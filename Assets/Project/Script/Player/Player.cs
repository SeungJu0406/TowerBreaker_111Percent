using UnityEngine;

namespace NSJ_Player
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private Transform _InitialPosition;
        [SerializeField] private int _health = 3;



        public void TakeDamage()
        {
            // 데미지 판정
            _health--;

            if (_health <= 0)
            {
                Die();
            }

            // 이벤트 발생
            GlobalEventManager.GlobalEvent.OnPlayerHitInvoke();

            // 처음 위치로 이동
            MoveBack();
        }

        private void Die()
        {
            Debug.Log("죽음");
        }

        private void MoveBack()
        {
            // 처음 위치로 이동하는 로직
            transform.position = Vector3.Lerp(transform.position, _InitialPosition.position, 0.5f);
        }
    }
}