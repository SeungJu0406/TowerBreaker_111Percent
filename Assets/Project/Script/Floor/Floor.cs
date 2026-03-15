
using NSJ_Enemy;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Floor : MonoBehaviour
{
    // 이 층에 속한 Enemys 그룹 리스트
    [SerializeField] private List<Enemys> enemysGroup;
    [SerializeField] private Enemys _enemysPrefab;
    [SerializeField] private Transform _InitialEnemyPos;

    // 이 층에 배치된 상자 리스트
    // 상자가 있는 방은 적 클리어 + 상자 전부 개봉까지 완료해야 다음 층으로 진입
    [SerializeField]private ChestObject _chestObjectPrefab;
    [SerializeField] private Transform _chestPos;

    [Header("Screen Positions")]
    [SerializeField] private float _enemyStartViewportX = 0.9f;
    [SerializeField] private float _chestStartViewPortX = 0.7f;
    private ChestObject _chest;
    
    // FloorManager가 구독 → 이 층의 모든 조건(적 + 상자)이 충족되면 전환 시작
    public event UnityAction OnFloorCleared;

    private int _clearedGroupCount;
    private int _openedChestCount;

    // FloorManager.Start()에서 최초 1회, 이후 전환 완료 시마다 호출
    public void StartFloor()
    {
        _clearedGroupCount = 0;
        _openedChestCount = 0;

        foreach (var enemys in enemysGroup)
        {
            // 그룹이 전부 죽으면 OnEnemysGroupCleared 호출되도록 구독
            enemys.OnAllEnemiesDead += OnEnemysGroupCleared;

            // Enemys는 기본적으로 _canMove = false 상태
            // StartFloor()가 불릴 때 비로소 이동 허용 → 전환 중에는 이동 안 함
            enemys.Resume();
        }
    }

    // 하나의 Enemys 그룹이 전멸할 때마다 호출
    private void OnEnemysGroupCleared()
    {
        _clearedGroupCount++;

        // 아직 남은 그룹이 있으면 대기
        if (_clearedGroupCount < enemysGroup.Count) return;

        // 모든 적 그룹 클리어 완료
        // 상자가 없으면 바로 층 클리어, 있으면 상자 잠금 해제 후 대기
        if (_chest == null)
        {
            OnFloorCleared?.Invoke();
        }
        else
        {
            UnlockChests();
        }
    }

    // 모든 적 사망 후 상자 잠금 해제
    // 이 시점부터 플레이어가 상자를 공격할 수 있음
    private void UnlockChests()
    {
        if (_chest == null) return;
        _chest.Unlock();
        _chest.OnOpened += OnChestOpened;
    }

    // 상자 하나가 열릴 때마다 호출
    private void OnChestOpened()
    {
        _openedChestCount++;

        // 활성 상자 수 기준으로 체크 (null이거나 이미 비활성화된 것 제외)
        int activeChestCount = 0;

        if (_chest != null) activeChestCount++;


        if (_openedChestCount >= activeChestCount)
            OnFloorCleared?.Invoke();
    }

    // 적 생성
    public void CreateEnemy(FloorData floorData)
    {
        // floorData의 EnemyGroup을 이용하여 적 생성
        int enemyGroupCount = 0;
        foreach (var enemyGroup in floorData.EnemysGroup)
        {
            List<Enemy> enemies = enemyGroup.Enemies;
            Enemys newEnemys = Instantiate(_enemysPrefab, transform);

            newEnemys.SetMoveSpeed(enemyGroup.EnemysSpeed);
            newEnemys.IntervalDistance = enemyGroup.EnemyInterval;

            enemysGroup.Add(newEnemys);
            // 위치 설정 — viewport 기반 X로 오른쪽 화면 끝 기준 스폰
            float depth  = Mathf.Abs(Camera.main.transform.position.z);
            float spawnX = Camera.main.ViewportToWorldPoint(new Vector3(_enemyStartViewportX, 0f, depth)).x;
            _InitialEnemyPos.position = new Vector3(spawnX, _InitialEnemyPos.position.y, _InitialEnemyPos.position.z);

            newEnemys.transform.position = _InitialEnemyPos.position;
            for (int i = 0; i < enemies.Count; i++)
            {
                // 적 생성 로직
                Enemy newEnemy = Instantiate(enemies[i], newEnemys.transform);
                // 생성된 적을 enemysGroup 리스트에 추가
                newEnemys.AddEnemy(newEnemy);
            }
            enemyGroupCount++;
        }
    }

    // 상자 생성

    public void CreateChest(FloorData floorData)
    {
        if (floorData.IsExistChest)
        {
            float depth = Mathf.Abs(Camera.main.transform.position.z);
            float spawnX = Camera.main.ViewportToWorldPoint(new Vector3(_chestStartViewPortX, 0f, depth)).x;
            _chestPos.position = new Vector3(spawnX, _chestPos.position.y, _chestPos.position.z);


            _chest = Instantiate(_chestObjectPrefab, transform);
            _chest.transform.position = _chestPos.position;
        }
    }
}
