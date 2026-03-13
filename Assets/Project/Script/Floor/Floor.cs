
using NSJ_Enemy;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Floor : MonoBehaviour
{
    // 이 층에 속한 Enemys 그룹 리스트 (프리팹에 미리 배치된 오브젝트들)
    [SerializeField] private List<Enemys> enemysGroup;

    // FloorManager가 구독 → 이 층의 모든 그룹이 클리어되면 전환 코루틴 시작
    public event UnityAction OnFloorCleared;

    // 몇 개의 Enemys 그룹이 클리어됐는지 카운팅
    private int _clearedGroupCount;

    // FloorManager.Start()에서 최초 1회, 이후 전환 완료 시마다 호출
    public void StartFloor()
    {
        _clearedGroupCount = 0;

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
    // 모든 그룹이 클리어되면 OnFloorCleared 이벤트 발생
    private void OnEnemysGroupCleared()
    {
        _clearedGroupCount++;

        // enemysGroup.Count > 0 체크: 빈 플로어에서 즉시 발동하는 오작동 방지
        if (_clearedGroupCount >= enemysGroup.Count && enemysGroup.Count > 0)
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
            for (int i = 0; i < enemies.Count; i++)
            {
                // 적 생성 로직
                Enemy newEnemy = Instantiate(enemies[i]);
                // 생성된 적을 enemysGroup 리스트에 추가
                enemysGroup[enemyGroupCount].AddEnemy(newEnemy);
            }
            enemyGroupCount++;
        }
    }
}
