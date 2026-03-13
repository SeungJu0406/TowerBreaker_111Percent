using NSJ_Enemy;
using UnityEngine;

namespace NSJ_Player
{
    public class DefenceBehaviour : MonoBehaviour
    {
        [SerializeField] private Player _player;
        [SerializeField] private KeyCode _defenceKey = KeyCode.Space;

        [Header("Overlap")]
        [SerializeField] private Vector2 _overlapOffset;
        [SerializeField] private Vector2 _overlapSize;
        [SerializeField] private LayerMask _enemyLayer;

        [Header("KnockBack")]
        [SerializeField] private float _knockBackForce = 5f;


        private void Update()
        {
            if (Input.GetKeyDown(_defenceKey))
            {
                Defence();
            }
        }

        private void Defence()
        {
            Vector2 center = (Vector2)transform.position + _overlapOffset;
            Collider2D[] hits = Physics2D.OverlapBoxAll(center, _overlapSize, 0f, _enemyLayer);

            foreach (Collider2D hit in hits)
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy == null || !enemy.CanHit) continue;

                Enemys enemys = enemy.GetComponentInParent<Enemys>();
                enemys?.KnockBack(_knockBackForce);
                break;
            }

            _player.MoveBack();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Vector2 center = (Vector2)transform.position + _overlapOffset;
            Gizmos.DrawWireCube(center, _overlapSize);
        }
    }
}
