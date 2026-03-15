using NSJ_Enemy;
using UnityEngine;

namespace NSJ_Player
{
    public class AttackBehaviour : MonoBehaviour
    {
        [SerializeField] private Player _player;

        [Header("Overlap")]
        [SerializeField] private Vector2 _overlapOffset = new Vector2(0.5f, 0.5f);
        [SerializeField] private Vector2 _overlapSize = new Vector2(0.5f, 1f);
        // 상자는 Enemy와 별도 레이어 — 인스펙터에서 Chest 레이어 지정
        [SerializeField] private LayerMask _chestLayer;


        private bool _canAttack = true;
        // 층 전환 중에는 공격 불가 — 플레이어가 화면 밖에 있으므로 입력을 막음
        private bool _isTransitioning = false;

        private IBattle _battle;

        private void Awake()
        {
            if (_player == null)
            {
                _player = GetComponentInParent<Player>();
                _battle = _player.GetComponent<IBattle>();
            }
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

        public void Attack()
        {
            // _isTransitioning: 층 전환 연출 중에는 입력 차단
            if (_canAttack == false || _isTransitioning) return;

            Vector2 center = (Vector2)transform.position + _overlapOffset;
            Collider2D[] hits = Physics2D.OverlapBoxAll(center, _overlapSize, 0f, Layer.EnemyGroup);

            foreach (Collider2D hit in hits)
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy == null || !enemy.CanHit) continue;

                // 배틀시스템으로 변경
                _battle.AttackTarget(enemy, _player.AttackPower);
            }

            // 상자 공격 — 적과 별도 레이어로 검출
            // 상자는 적이 모두 죽은 뒤 Unlock()이 호출돼야만 TryHit()이 true를 반환
            Collider2D[] chestHits = Physics2D.OverlapBoxAll(center, _overlapSize, 0f, _chestLayer);
            foreach (Collider2D hit in chestHits)
            {
                ChestObject chest = hit.GetComponent<ChestObject>();
                if (chest == null) continue;

                _battle.AttackTarget(chest, _player.AttackPower);
                break;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Vector2 center = (Vector2)transform.position + _overlapOffset;
            Gizmos.DrawWireCube(center, _overlapSize);
        }
    }
}