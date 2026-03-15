using NSJ_Enemy;
using System.Collections;
using UnityEngine;

namespace NSJ_Player
{
    public class DefenceBehaviour : MonoBehaviour
    {
        [SerializeField] private Player _player;

        [Header("Overlap")]
        [SerializeField] private Vector2 _overlapOffset = new Vector2(0.5f, 0.5f);
        [SerializeField] private Vector2 _overlapSize = new Vector2(0.5f, 1f);

        [Header("KnockBack")]
        [SerializeField] private float _knockBackForce = 5f;
        [SerializeField] private float _moveBackDuration = 0.5f;


        private bool _canDefend = true;
        // 층 전환 중에는 방어 불가 — 화면 밖에 있는 상태에서 방어가 작동하면 위치 복귀 로직이 충돌함
        private bool _isTransitioning = false;

        private void Awake()
        {
            if (_player == null)
                _player = GetComponentInParent<Player>();
        }

        private void Start()
        {
            if (Manager.Event == null) return;
            Manager.Event.OnStageTransitionStart += OnTransitionStart;
            Manager.Event.OnStageTransitionEnd += OnTransitionEnd;
        }

        private void OnDestroy()
        {
            if (Manager.Event == null) return;
            Manager.Event.OnStageTransitionStart -= OnTransitionStart;
            Manager.Event.OnStageTransitionEnd -= OnTransitionEnd;
        }

        private void OnTransitionStart() => _isTransitioning = true;
        private void OnTransitionEnd() => _isTransitioning = false;


        public void Defence()
        {
            // _isTransitioning: 층 전환 연출 중에는 입력 차단
            if (_canDefend == false || _isTransitioning) return;
            Vector2 center = (Vector2)transform.position + _overlapOffset;
            Collider2D[] hits = Physics2D.OverlapBoxAll(center, _overlapSize, 0f, Layer.EnemyGroup);

            foreach (Collider2D hit in hits)
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy == null || !enemy.CanHit) continue;

                Enemys enemys = enemy.GetComponentInParent<Enemys>();
                enemys?.KnockBack(_knockBackForce, _moveBackDuration);
                MoveBack();
            }


        }


        public void MoveBack()
        {
            StartCoroutine(MoveBackCoroutine());
        }
        private IEnumerator MoveBackCoroutine()
        {
            _canDefend = false;
            Manager.Event?.OnDefenceStartInvoke();

            Vector3 startPos = _player.transform.position;
            float elapsed = 0f;

            while (elapsed < _moveBackDuration)
            {
                elapsed += Time.deltaTime;
                _player.transform.position = Vector3.Lerp(startPos, _player.InitialPosition.position, elapsed / _moveBackDuration);
                yield return null;
            }

            _player.transform.position = _player.InitialPosition.position;
            _canDefend = true;
            Manager.Event?.OnDefenceEndInvoke();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Vector2 center = (Vector2)transform.position + _overlapOffset;
            Gizmos.DrawWireCube(center, _overlapSize);
        }
    }
}
