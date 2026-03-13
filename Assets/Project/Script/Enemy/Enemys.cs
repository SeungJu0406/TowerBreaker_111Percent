
using System.Collections.Generic;
using UnityEngine;

namespace NSJ_Enemy
{
    public enum Direction
    {
        Left,
        Right,
    }

    public class Enemys : MonoBehaviour
    {
        [SerializeField] private List<Enemy> _enemies;

        [SerializeField] private Direction _direction;
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _intervalDistance;


        private void Start()
        {
            ControlEnemyInterval();
            InitEnemys();
        }

        private void Update()
        {
            Move();

            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    TestTakeDamage();
            //}
        }

        // 부모 이동 제어
        private void Move()
        {
            // 이동 방향에 따른 이동
            Vector2 moveDirection = _direction == Direction.Left ? Vector2.left : Vector2.right;
            transform.Translate(moveDirection * _moveSpeed * Time.deltaTime);
        }

        // 자식 간격 제어

        private void ControlEnemyInterval()
        {
            if (_enemies.Count <= 0) return;

            // 자식 간격 제어 로직
            // 0번 인덱스의 몹을 본인 위치로 이동
            Enemy enemy0 = _enemies[0];

            // 이후 인덱스의 몹들을 일정한 간격을 두고 뒤쪽으로 배치
            Vector2 intervalVector = _direction == Direction.Left ? Vector2.right : Vector2.left;

            for (int i = 1; i < _enemies.Count; i++)
            {
                Enemy enemy = _enemies[i];
                enemy.transform.position = enemy0.transform.position + (Vector3)(intervalVector * _intervalDistance * i);
            }
        }

        // 리스트 관리

        private void InitEnemys()
        {
            for (int i = 0; i < _enemies.Count; i++)
            {
                Enemy enemy = _enemies[i];
         
                if (i == 0)
                {
                    enemy.SetCanHit(true);
                }
                else
                {
                    enemy.SetCanHit(false);
                }

                Enemy prev = i > 0 ? _enemies[i - 1] : null;
                Enemy next = i < _enemies.Count - 1 ? _enemies[i + 1] : null;

                enemy.SetNeighbor(prev,next);
            }
        }


        //private void TestTakeDamage()
        //{
        //    for (int i = 0; i < _enemies.Count; i++)
        //    {
        //        Enemy enemy = _enemies[i];
        //        if (enemy.CanHit == true)
        //        {
        //            enemy.TakeDamage(10f);
        //            break;
        //        }
        //    }
        //}
    }
}