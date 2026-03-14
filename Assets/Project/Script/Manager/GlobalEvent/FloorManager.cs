using System.Collections;
using System.Collections.Generic;
using NSJ_Player;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using Utility;

public class FloorManager : MonoBehaviour
{
    [SerializeField] private List<FloorData> _floorData;
    [SerializeField] private Floor _floorPrefab;
    // 플레이어 이동 코루틴을 직접 yield 하기 위해 참조 필요
    [SerializeField] private Player _player;

    [Header("Transition")]
    [SerializeField] private Vector3 _initPos = new Vector3(0, 0, 0);
    // 플로어 간 수직 간격 (Y축 기준, 위로 쌓임)
    [SerializeField] private float _floorHeight = 10f;
    // 층 하강 연출에 걸리는 시간(초)
    [SerializeField] private float _transitionDuration = 1f;

   [SerializeField] private Floor _currentFloor;
    private int _currentFloorIndex;

    // Key = 플로어 인덱스, Value = 실제 Floor 오브젝트
    // List 대신 Dictionary를 쓴 이유:
    // 층 번호로 즉시 접근 + 중간 층 삭제가 필요해서 인덱스 기반 관리가 편함
    private Dictionary<int, Floor> _activeFloors = new Dictionary<int, Floor>();

    void Awake()
    {
        Manager.SetFloor(this);

       _player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    void Start()
    {
        // 시작 시 현재 층(0) + 최대 앞 3층까지 미리 생성
        // 이유: 전환 연출 도중 다음 층이 이미 화면에 보여야 하므로 사전 생성 필요
        int preloadCount = Mathf.Min(3, _floorData.Count - 1);
        for (int i = 0; i <= preloadCount; i++)
            SpawnFloor(i);

        // 0번 플로어를 현재 층으로 설정하고 시작
        _currentFloorIndex = 0;
        _currentFloor = _activeFloors[0];
        _currentFloor.OnFloorCleared += OnCurrentFloorCleared;
        _currentFloor.StartFloor();
    }

    void OnDestroy()
    {
        if (Manager.Floor == this)
            Manager.SetFloor(null);
    }

    // 플로어 프리팹을 생성하고 적을 배치
    // 위치: index * _floorHeight → 현재 층 위로 한 칸씩 올라가는 구조
    private void SpawnFloor(int floorIndex)
    {
        int y = floorIndex - _currentFloorIndex;

        Floor floor = Instantiate(_floorPrefab, new Vector3(_initPos.x, _initPos.y + y * _floorHeight, _initPos.z), Quaternion.identity);
        floor.CreateEnemy(GetFloorData(floorIndex));
        floor.CreateChest(GetFloorData(floorIndex));
        _activeFloors[floorIndex] = floor;
    }

    // 현재 플로어의 모든 적이 죽었을 때 Floor에서 이 콜백 호출
    private void OnCurrentFloorCleared()
    {
        // 이벤트 중복 구독 방지 (다음 전환 때 다시 등록하므로 여기서 해제)
        _currentFloor.OnFloorCleared -= OnCurrentFloorCleared;
        Manager.Event?.OnStageClearInvoke();

        // 마지막 층인지 확인
        // 다음 인덱스가 _floorData 범위를 벗어나면 더 이상 올라갈 층이 없음
        if (_currentFloorIndex + 1 >= _floorData.Count)
        {
            // 최종 클리어 — 전환 없이 종료
            // 나중에 게임 클리어 연출/UI/씬 전환으로 교체 예정
            Debug.Log("최종 클리어!");
            return;
        }

        StartCoroutine(TransitionCoroutine());
    }

    // ─── 전환 순서 ─────────────────────────────────────────────────────
    // 1. OnStageTransitionStart → Behaviour 입력 차단
    // 2. 플레이어가 위쪽 화면 밖으로 이동 (완료까지 대기)
    // 3. 모든 활성 플로어가 아래로 하강 (완료까지 대기)
    // 4. 다음 층 활성화, 앞 층 추가 생성, 낙오 층 삭제
    // 5. 플레이어가 왼쪽 화면 밖 → InitialPosition으로 등장 (완료까지 대기)
    // 6. OnStageTransitionEnd → 입력 허용
    // ────────────────────────────────────────────────────────────────────
    private IEnumerator TransitionCoroutine()
    {
        // 입력 차단 — 이 시점부터 플레이어가 복귀할 때까지 Behaviour들은 동작하지 않음
        Manager.Event?.OnStageTransitionStartInvoke();

        // [1단계] 플레이어 먼저 화면 위로 퇴장
        // yield return으로 완료를 기다린 후 플로어를 내림
        yield return StartCoroutine(_player.MoveOffScreenCoroutine());

        // [2단계] 플레이어가 사라진 후 플로어 하강 연출 시작
        // 코루틴 실행 중 _activeFloors가 변경되면 안 되므로
        // 시작 시점의 위치를 리스트로 캡처해 둠
        var floorMoves = new List<(Floor floor, Vector3 startPos)>();
        foreach (var kv in _activeFloors)
            floorMoves.Add((kv.Value, kv.Value.transform.position));

        // Lerp로 _transitionDuration 동안 모든 플로어를 아래로 이동
        float elapsed = 0f;
        while (elapsed < _transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _transitionDuration;
            foreach (var (floor, startPos) in floorMoves)
                floor.transform.position = Vector3.Lerp(startPos, startPos + Vector3.down * _floorHeight, t);
            yield return null;
        }

        // Lerp는 부동소수점 오차가 있으므로 완료 후 정확한 위치로 고정
        foreach (var (floor, startPos) in floorMoves)
            floor.transform.position = startPos + Vector3.down * _floorHeight;

        // [3단계] 플로어 하강 완료 → 다음 층 활성화
        _currentFloorIndex++;
        _currentFloor = _activeFloors[_currentFloorIndex];
        _currentFloor.OnFloorCleared += OnCurrentFloorCleared;

        DestroyFloor();
        CreateFloor();

        // [4단계] 플레이어 왼쪽에서 등장
        // 새 층이 세팅된 뒤에 플레이어가 나타나므로 적과 타이밍이 맞음
        yield return StartCoroutine(_player.MoveFromLeftCoroutine());

        yield return 0.5f.Second(); // 잠깐 대기 (선택 사항, 임시)   

        _currentFloor.StartFloor();

        // 모든 연출 완료 → 입력 허용
        Manager.Event?.OnStageTransitionEndInvoke();
    }

    // 현재 층으로부터 +3층까지 미리 생성
    // 이미 생성된 층은 ContainsKey로 중복 방지
    private void CreateFloor()
    {
        for (int i = _currentFloorIndex + 1; i <= _currentFloorIndex + 3; i++)
        {
            if (i >= _floorData.Count || _activeFloors.ContainsKey(i)) continue;
            SpawnFloor(i);
        }
    }

    // 현재 층 기준 2층 이상 뒤에 있는 플로어 삭제
    // 이유: 더 이상 보이지 않는 층을 메모리에 계속 유지할 필요가 없음
    private void DestroyFloor()
    {
        var toDestroy = new List<int>();
        foreach (var key in _activeFloors.Keys)
        {
            if (key < _currentFloorIndex - 1)
                toDestroy.Add(key);
        }
        foreach (int k in toDestroy)
        {
            Destroy(_activeFloors[k].gameObject);
            _activeFloors.Remove(k);
        }
    }

    public FloorData GetFloorData(int floorIndex)
    {
        if (floorIndex < 0 || floorIndex >= _floorData.Count)
        {
            Debug.LogError($"Floor index {floorIndex} is out of range.");
            return null;
        }
        return _floorData[floorIndex];
    }
}
