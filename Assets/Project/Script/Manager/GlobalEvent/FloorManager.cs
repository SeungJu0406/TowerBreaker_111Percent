using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    [SerializeField] private List<FloorData> _floorData;
    [SerializeField] private Floor _floorPrefab;

    [Header("Transition")]
    [SerializeField] private float _floorHeight = 10f;
    [SerializeField] private float _transitionDuration = 1f;

    private Floor _currentFloor;
    private int _currentFloorIndex;
    private Dictionary<int, Floor> _activeFloors = new Dictionary<int, Floor>();

    void Awake()
    {
        Manager.SetFloor(this);
    }

    void Start()
    {
        // 현재 층 + 앞 3층 생성
        int preloadCount = Mathf.Min(3, _floorData.Count - 1);
        for (int i = 0; i <= preloadCount; i++)
            SpawnFloor(i);

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

    private void SpawnFloor(int index)
    {
        Floor floor = Instantiate(_floorPrefab, new Vector3(0, index * _floorHeight, 0), Quaternion.identity);
        floor.CreateEnemy(GetFloorData(index));
        _activeFloors[index] = floor;
    }

    private void OnCurrentFloorCleared()
    {
        _currentFloor.OnFloorCleared -= OnCurrentFloorCleared;
        Manager.Event?.OnStageClearInvoke();
        StartCoroutine(TransitionCoroutine());
    }

    private IEnumerator TransitionCoroutine()
    {
        Manager.Event?.OnStageTransitionStartInvoke();

        // 모든 활성 플로어 시작 위치 캡처
        var floorMoves = new List<(Floor floor, Vector3 startPos)>();
        foreach (var kv in _activeFloors)
            floorMoves.Add((kv.Value, kv.Value.transform.position));

        // 플로어들 아래로 이동
        float elapsed = 0f;
        while (elapsed < _transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _transitionDuration;
            foreach (var (floor, startPos) in floorMoves)
                floor.transform.position = Vector3.Lerp(startPos, startPos + Vector3.down * _floorHeight, t);
            yield return null;
        }

        // 최종 위치 고정
        foreach (var (floor, startPos) in floorMoves)
            floor.transform.position = startPos + Vector3.down * _floorHeight;

        _currentFloorIndex++;

        // 다음 층 세팅
        _currentFloor = _activeFloors[_currentFloorIndex];
        _currentFloor.OnFloorCleared += OnCurrentFloorCleared;
        _currentFloor.StartFloor();

        CreateFloor();
        DestroyFloor();

        Manager.Event?.OnStageTransitionEndInvoke();
    }

    // 현재 층으로부터 +3층 생성
    private void CreateFloor()
    {
        for (int i = _currentFloorIndex + 1; i <= _currentFloorIndex + 3; i++)
        {
            if (i >= _floorData.Count || _activeFloors.ContainsKey(i)) continue;
            SpawnFloor(i);
        }
    }

    // 현재 층으로부터 -2층 삭제
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
