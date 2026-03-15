using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace NSJ_Player
{
    public class Player : MonoBehaviour
    {
        public Transform InitialPosition => _initialPosition;
        [SerializeField] private Transform _initialPosition;
        [SerializeField] private int _health = 3;
        public int Health { get => _health; set {  _health = value; OnHealthChange?.Invoke(value); }   }
        public event UnityAction<int> OnHealthChange;

        public float AttackPower {
            get
            {
                float power = _attackPower;
                if (UserDataManager.Instance.CurEquipment != null && UserDataManager.Instance.CurEquipment.Data != null)
                {
                    power += UserDataManager.Instance.CurEquipment.Data.BasicDamage;
                }
                return power;
            }
        }

        [SerializeField] private float _attackPower;

        [Header("Screen Positions")]
        [SerializeField] private float _initViewportX       =  0.35f;
        [SerializeField] private float _leftEntryViewportX  = -0.15f;
        [SerializeField] private float _rightExitViewportX  =  1.15f;

        [Header("Stage Transition")]
        // 화면 밖 이동 / 왼쪽 등장 각각에 걸리는 시간(초)
        [SerializeField] private float _transitionMoveDuration = 0.5f;
        [SerializeField] private Transform _rightEntryPos;
        [SerializeField] private Transform _leftEntryPos;
        public bool IsCollide => _isCollide;
        private bool _isCollide = false;
        // 전환 중 왼쪽 Boundary를 통과할 때 피격 판정이 발생하는 버그 방지용
        public bool IsTransitioning => _isTransitioning;
        private bool _isTransitioning = false;


        public Rigidbody2D Rb => _rb;
        private Rigidbody2D _rb;

        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            AdjustPositionsToScreen();
        }

        private void Start()
        {
            transform.position = _initialPosition.position;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag(Tag.Enemy) ||
                collision.gameObject.CompareTag(Tag.Chest) ||
                collision.gameObject.CompareTag(Tag.Boss))
            {
                // Chest도 대쉬를 막는 실제 물리 오브젝트 — 상자 앞에서 대쉬 정지
                _isCollide = true;
            }
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag(Tag.Enemy) ||
                collision.gameObject.CompareTag(Tag.Chest) ||
                collision.gameObject.CompareTag(Tag.Boss))
            {
                _isCollide = false;
            }
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag(Tag.Boundary))
            {
                // 전환 중 왼쪽에서 등장할 때 Boundary를 통과하므로 판정 무시
                if (_isTransitioning) return;
                _isCollide = true;
            }

        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag(Tag.Boundary))
            {
                _isCollide = false;
            }
        }
        public void TakeDamage()
        {
            // 데미지 판정
            if( _isTransitioning) return;

            Health--;

            if (Health <= 0)
            {
                Die();
                return;
            }

            // 이벤트 발생
            Manager.Event?.OnPlayerHitInvoke();
        }



        private void Die()
        {
            Manager.Event?.OnPlayerDiedInvoke();

            gameObject.SetActive(false);
        }

        // FloorManager가 yield return으로 대기하며 순서를 제어하므로
        // Player는 이벤트를 직접 구독하지 않음 — 이동 코루틴만 public으로 제공

        // [전환 1단계] 오른쪽 화면 밖으로 이동
        // FloorManager에서 yield return StartCoroutine(_player.MoveOffScreenCoroutine()) 로 호출
        public IEnumerator MoveOffScreenCoroutine()
        {
            Vector3 start = transform.position;
            Vector3 target = _rightEntryPos.position;
            float elapsed = 0f;
            while (elapsed < _transitionMoveDuration)
            {
                elapsed += Time.deltaTime;
                transform.position = Vector3.Lerp(start, target, elapsed / _transitionMoveDuration);
                yield return null;
            }
        }

        // [전환 3단계] 왼쪽 화면 밖에서 InitialPosition으로 등장
        // 플로어 하강이 완료된 뒤 호출 → 새 층에 플레이어가 나타나는 연출
        public IEnumerator MoveFromLeftCoroutine()
        {
            // 이동 중 왼쪽 Boundary 트리거를 통과하므로 피격 판정 무시
            _isTransitioning = true;

            Vector3 target = _initialPosition.position;
            // InitialPosition 기준 왼쪽으로 _offScreenDistance 만큼 떨어진 곳에서 시작
            Vector3 start = _leftEntryPos.position;
            transform.position = start;

            float elapsed = 0f;
            while (elapsed < _transitionMoveDuration)
            {
                elapsed += Time.deltaTime;
                transform.position = Vector3.MoveTowards(start, target, elapsed / _transitionMoveDuration);
                yield return null;
            }
            transform.position = target;

            _rb.linearVelocity = Vector3.zero;

            _isTransitioning = false;
        }
        private void AdjustPositionsToScreen()
        {
            Camera cam = Camera.main;
            float depth = Mathf.Abs(cam.transform.position.z);

            float initW = cam.ViewportToWorldPoint(new Vector3(_initViewportX, 0f, depth)).x;
            float leftW = cam.ViewportToWorldPoint(new Vector3(_leftEntryViewportX, 0f, depth)).x;
            float rightW = cam.ViewportToWorldPoint(new Vector3(_rightExitViewportX, 0f, depth)).x;

            _initialPosition.position = new Vector3(initW, _initialPosition.position.y, 0f);
            _leftEntryPos.position = new Vector3(leftW, _leftEntryPos.position.y, 0f);
            _rightEntryPos.position = new Vector3(rightW, _rightEntryPos.position.y, 0f);
        }
    }

}
