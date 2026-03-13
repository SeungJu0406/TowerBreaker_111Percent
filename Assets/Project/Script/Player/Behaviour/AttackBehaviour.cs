using NSJ_Enemy;
using UnityEngine;

namespace NSJ_Player
{
    public class AttackBehaviour : MonoBehaviour
    {
        [SerializeField] private Player _player;
        [SerializeField] private KeyCode _attackKey = KeyCode.C;

        [Header("Overlap")]
        [SerializeField] private Vector2 _overlapOffset = new Vector2(0.5f, 0.5f);
        [SerializeField] private Vector2 _overlapSize = new Vector2(0.5f, 1f);
        [SerializeField] private LayerMask _enemyLayer;


        private bool _canAttack = true;

        private void Awake()
        {
            if (_player == null)
                _player = GetComponentInParent<Player>();
        }

        private void Update()
        {
            if (_canAttack == false) return;

            if (Input.GetKeyDown(_attackKey))
            {
                Attack();
            }
        }
        private void Attack()
        {
            Vector2 center = (Vector2)transform.position + _overlapOffset;
            Collider2D[] hits = Physics2D.OverlapBoxAll(center, _overlapSize, 0f, _enemyLayer);

            foreach (Collider2D hit in hits)
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy == null || !enemy.CanHit) continue;

                ((IHitable)enemy).TakeDamage(_player.AttackPower);
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