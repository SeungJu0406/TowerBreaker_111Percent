using System.Collections;
using UnityEngine;

namespace NSJ_Player
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private Transform _InitialPosition;
        [SerializeField] private int _health = 3;
        [SerializeField] private float _moveBackDuration = 0.5f;



        public void TakeDamage()
        {
            // 데미지 판정
            _health--;

            if (_health <= 0)
            {
                Die();
                return;
            }

            // 이벤트 발생
            GlobalEventManager.GlobalEvent?.OnPlayerHitInvoke();

            // 처음 위치로 이동
            StartCoroutine(MoveBackCoroutine());
        }

        private void Die()
        {
            Debug.Log("죽음");
        }

        private IEnumerator MoveBackCoroutine()
        {
            Vector3 startPos = transform.position;
            float elapsed = 0f;

            while (elapsed < _moveBackDuration)
            {
                elapsed += Time.deltaTime;
                transform.position = Vector3.Lerp(startPos, _InitialPosition.position, elapsed / _moveBackDuration);
                yield return null;
            }

            transform.position = _InitialPosition.position;
        }
    }
}
