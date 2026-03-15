
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
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
        public float IntervalDistance {  get => _intervalDistance; set { _intervalDistance = value; }}
        [SerializeField] private float _intervalDistance;

        // Floor가 구독 → 이 그룹의 적이 전원 사망하면 클리어 카운트 증가
        public event UnityAction OnAllEnemiesDead;

        // false로 시작하는 이유:
        // 층이 생성된 직후에는 아직 "플레이 중"이 아님
        // Floor.StartFloor() → Resume()이 호출될 때 비로소 이동 시작
        // 덕분에 전환 연출 중 적이 움직이지 않음
        [SerializeField] private bool _canMove = false;

        // 사망한 적 수 카운터 (전원 사망 감지용)
        private int _deadCount = 0;
        Rigidbody2D _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            // 인스펙터에 미리 배치된 적(pre-placed)에 대한 사망 이벤트 구독
            // AddEnemy()를 통해 추가된 적은 AddEnemy() 안에서 따로 구독함
            // 지금은 필요없을듯
            //foreach (var enemy in _enemies)
            //    enemy.OnDie += OnEnemyDied;

            ControlEnemyInterval();
            InitEnemys();

            Manager.Event.OnPlayerHit += HitPlayerAfter;
            Manager.Event.OnPlayerDied += DiedPlayerAfter;
        }

        private void OnDestroy()
        {
            if (Manager.Event != null)
            {
                Manager.Event.OnPlayerHit -= HitPlayerAfter;
                Manager.Event.OnPlayerDied -= DiedPlayerAfter;
            }
        }

        private void Update()
        {
            Move();
        }

        // 적 추가
        public void AddEnemy(Enemy enemy)
        {
            // 동적으로 추가되는 적도 사망 이벤트 구독
            enemy.OnDie += OnEnemyDied;
            _enemies.Add(enemy);
            ControlEnemyInterval();
            InitEnemys();
        }

        // Floor.StartFloor()에서 호출 → 층 시작 시 이동 허용
        // _deadCount 리셋: 같은 인스턴스가 재사용될 경우 이전 카운트가 남아 오작동 방지
        public void Resume()
        {
            _deadCount = 0;
            _canMove = true;
        }

        // 적 한 명이 죽을 때마다 카운트
        // _enemies.Count에 도달하면 이 그룹의 전원 사망으로 판단
        private void OnEnemyDied()
        {
            _deadCount++;
            if (_deadCount >= _enemies.Count)
                OnAllEnemiesDead?.Invoke();
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
        private void DiedPlayerAfter()
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

        public void SetMoveSpeed(float moveSpeed)
        {
            _moveSpeed = moveSpeed;
            _rb.bodyType = _moveSpeed > 0 ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;
        }
    }
}
