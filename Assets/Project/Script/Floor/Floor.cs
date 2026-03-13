
using NSJ_Enemy;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Floor : MonoBehaviour
{
    // 적을 모아둘 리스트
    [SerializeField] private List<Enemys> enemysGroup;

    public event UnityAction OnFloorCleared;
    private int _clearedGroupCount;

    // 스테이지 스타트
    public void StartFloor()
    {
        _clearedGroupCount = 0;
        foreach (var enemys in enemysGroup)
        {
            enemys.OnAllEnemiesDead += OnEnemysGroupCleared;
            enemys.Resume();
        }
    }

    private void OnEnemysGroupCleared()
    {
        _clearedGroupCount++;
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
