
using NSJ_Enemy;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    // 적을 모아둘 리스트
    [SerializeField] private List<Enemys> enemysGroup;

    // 스테이지 스타트
    public void StartFloor()
    {
        // 스타트하면 적이 이제부터 움직일 수 있음
        // 각 Enemys 의 InitEnemy 를 호출하고 움직일 수 있도록 설정
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
