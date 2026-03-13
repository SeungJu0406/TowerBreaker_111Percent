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
        // 화면 밖 이동 / 왼쪽 등장 각각에 걸리는 시간(초)
        [SerializeField] private float _transitionMoveDuration = 0.5f;

        public bool IsCollide => _isCollide;
        private bool _isCollide = false;
        // 전환 중 왼쪽 Boundary를 통과할 때 피격 판정이 발생하는 버그 방지용
        public bool IsTransitioning => _isTransitioning;
        private bool _isTransitioning = false;

        // FloorManager가 yield return으로 대기하며 순서를 제어하므로
        // Player는 이벤트를 직접 구독하지 않음 — 이동 코루틴만 public으로 제공

        // [전환 1단계] 오른쪽 화면 밖으로 이동
        // FloorManager에서 yield return StartCoroutine(_player.MoveOffScreenCoroutine()) 로 호출
        public IEnumerator MoveOffScreenCoroutine()
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

        // [전환 3단계] 왼쪽 화면 밖에서 InitialPosition으로 등장
        // 플로어 하강이 완료된 뒤 호출 → 새 층에 플레이어가 나타나는 연출
        public IEnumerator MoveFromLeftCoroutine()
        {
            // 이동 중 왼쪽 Boundary 트리거를 통과하므로 피격 판정 무시
            _isTransitioning = true;

            Vector3 target = _initialPosition.position;
            // InitialPosition 기준 왼쪽으로 _offScreenDistance 만큼 떨어진 곳에서 시작
            Vector3 start = new Vector3(target.x - _offScreenDistance, target.y, target.z);
            transform.position = start;

            float elapsed = 0f;
            while (elapsed < _transitionMoveDuration)
            {
                elapsed += Time.deltaTime;
                transform.position = Vector3.Lerp(start, target, elapsed / _transitionMoveDuration);
                yield return null;
            }
            transform.position = target;

            _isTransitioning = false;
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
