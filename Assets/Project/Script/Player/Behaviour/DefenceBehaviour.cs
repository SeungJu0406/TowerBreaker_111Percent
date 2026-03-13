using NSJ_Enemy;
using System.Collections;
using UnityEngine;

namespace NSJ_Player
{
    public class DefenceBehaviour : MonoBehaviour
    {
        [SerializeField] private Player _player;
        [SerializeField] private KeyCode _defenceKey = KeyCode.X;

        [Header("Overlap")]
        [SerializeField] private Vector2 _overlapOffset = new Vector2(0.5f, 0.5f);
        [SerializeField] private Vector2 _overlapSize = new Vector2(0.5f, 1f);
        [SerializeField] private LayerMask _enemyLayer;

        [Header("KnockBack")]
        [SerializeField] private float _knockBackForce = 5f;
        [SerializeField] private float _moveBackDuration = 0.5f;


        private bool _canDefend = true;

        private void Awake()
        {
            if (_player == null)
                _player = GetComponentInParent<Player>();
        }

        private void Update()
        {
            if (_canDefend == false) return;

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
                enemys?.KnockBack(_knockBackForce, _moveBackDuration);
                MoveBack();
                break;
            }


        }


        public void MoveBack()
        {
            StartCoroutine(MoveBackCoroutine());
        }
        private IEnumerator MoveBackCoroutine()
        {
            _canDefend = false;

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
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Vector2 center = (Vector2)transform.position + _overlapOffset;
            Gizmos.DrawWireCube(center, _overlapSize);
        }
    }
}
