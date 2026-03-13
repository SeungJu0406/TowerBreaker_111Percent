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
        // 화면 밖으로 나가는 거리 (카메라 범위 밖이 되도록 충분히 크게 설정)
        [SerializeField] private float _offScreenDistance = 20f;
        // 화면 밖 이동에 걸리는 시간(초) — 층 하강 연출(_transitionDuration)보다 짧게 설정 권장
        [SerializeField] private float _transitionMoveDuration = 0.5f;

        public bool IsCollide => _isCollide;
        private bool _isCollide = false;

        private void Start()
        {
            if (Manager.Event == null) return;
            // 전환 시작 → 화면 밖으로 이동 / 전환 완료 → InitialPosition으로 복귀
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

        // 전환이 완료되면 위치를 스냅으로 복귀
        // 코루틴 없이 즉시 이동하는 이유:
        // 층 이동 연출이 끝난 시점에서 플레이어가 갑자기 나타나는 연출이 자연스러움
        // (카메라 밖에서 갑자기 등장 = 시작 위치에서 뿅 나오는 느낌)
        private void OnTransitionEnd() => transform.position = _initialPosition.position;

        // 층 전환 연출 중 플레이어를 카메라 오른쪽 밖으로 이동
        private IEnumerator MoveOffScreenCoroutine()
        {
            Vector3 start = transform.position;
            Vector3 target = start + Vector3.right * _offScreenDistance;
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
