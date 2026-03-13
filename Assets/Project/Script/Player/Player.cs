using System.Collections;
using UnityEngine;

namespace NSJ_Player
{
    public class Player : MonoBehaviour
    {
        public Transform InitialPosition => _initialPosition;
        [SerializeField] private Transform _initialPosition;
        [SerializeField] private int _health = 3;
        [SerializeField] private float _attackPower;
        public float AttackPower => _attackPower;

        [Header("Stage Transition")]
        [SerializeField] private float _offScreenDistance = 20f;
        [SerializeField] private float _transitionMoveDuration = 0.5f;

        public bool IsCollide => _isCollide;
        private bool _isCollide = false;

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

        private void OnTransitionStart() => StartCoroutine(MoveOffScreenCoroutine());

        private void OnTransitionEnd() => transform.position = _initialPosition.position;

        private IEnumerator MoveOffScreenCoroutine()
        {
            Vector3 start = transform.position;
            Vector3 target = start + Vector3.up * _offScreenDistance;
            float elapsed = 0f;
            while (elapsed < _transitionMoveDuration)
            {
                elapsed += Time.deltaTime;
                transform.position = Vector3.Lerp(start, target, elapsed / _transitionMoveDuration);
                yield return null;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag(Tag.Enemy))
            {
                _isCollide = true;
            }
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag(Tag.Enemy))
            {
                _isCollide = false;
            }
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag(Tag.Boundary))
            {
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
            _health--;

            if (_health <= 0)
            {
                Die();

                // 임시로 사용, 나중에 Die 이벤트로 변경
                Manager.Event?.OnPlayerHitInvoke();

                return;
            }

            // 이벤트 발생
            Manager.Event?.OnPlayerHitInvoke();
        }



        private void Die()
        {
            Debug.Log("죽음");
        }


    }
}
