
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

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

        private bool _canMove = true;
        Rigidbody2D _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            ControlEnemyInterval();
            InitEnemys();

            Manager.Event.OnPlayerHit += HitPlayerAfter;
        }

        private void OnDestroy()
        {
            if (Manager.Event != null)
                Manager.Event.OnPlayerHit -= HitPlayerAfter;
        }

        private void Update()
        {
            Move();
        }

        // 적 추가
        public void AddEnemy(Enemy enemy)
        {
            _enemies.Add(enemy);
            ControlEnemyInterval();
            InitEnemys();
        }

        // 부모 이동 제어
        private void Move()
        {
            if (_canMove == false) return;

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

                enemy.SetNeighbor(prev, next);
            }
        }

        private void HitPlayerAfter()
        {
            Stop();
        }


        private void Stop()
        {
            _canMove = false;
        }
        // 몹 뒤로 밀림 현상
        public void KnockBack(float knockBackForce, float duration)
        {
            _canMove = true;
            StartCoroutine(MoveBackCoroutine(knockBackForce, duration));
        }

        IEnumerator MoveBackCoroutine(float knockBackForce, float duration)
        {

            float saveMoveSpeed = _moveSpeed;
            // 이동속도 0
            SetMoveSpeed(0);
            _rb.bodyType = RigidbodyType2D.Dynamic;
            // Rigidbody2D의 AddForce로 뒤로 밀림
            Vector2 dir = _direction == Direction.Left ? Vector2.right : Vector2.left;
            _rb.AddForce(dir * knockBackForce, ForceMode2D.Impulse);

            yield return duration.Second();
            // velocity 초기화
            _rb.linearVelocity = Vector2.zero;
            // 이동속도 원래대로
            SetMoveSpeed(saveMoveSpeed);
        }

        private void SetMoveSpeed(float moveSpeed)
        {
            _moveSpeed = moveSpeed;
            _rb.bodyType = _moveSpeed > 0 ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;
        }
    }
}