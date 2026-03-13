using System.Collections;
using UnityEngine;

namespace NSJ_Player
{
    public class Player : MonoBehaviour
    {
        public Transform InitialPosition => _initialPosition;
        [SerializeField] private Transform _initialPosition;
        [SerializeField] private int _health = 3;
        public float AttackPower;


        public bool IsCollide => _isCollide;  
        private bool _isCollide = false;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag(Tag.Enemy) ||
                collision.gameObject.CompareTag(Tag.Boundary))
            {
                _isCollide = true;
            }
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag(Tag.Enemy) ||
                collision.gameObject.CompareTag(Tag.Boundary))
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
                GlobalEventManager.GlobalEvent?.OnPlayerHitInvoke();

                return;
            }

            // 이벤트 발생
            GlobalEventManager.GlobalEvent?.OnPlayerHitInvoke();
        }



        private void Die()
        {
            Debug.Log("죽음");
        }


    }
}
