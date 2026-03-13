using UnityEngine;

namespace NSJ_Player
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private Transform _InitialPosition;

        // 적과 맞대면판정
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag(Tag.Enemy))
            {
                Debug.Log("적과 충돌");
            }
        }

        private void Start()
        {
            GlobalEventManager.GlobalEvent.OnPlayerHit += MoveBack;
        }

        // 데미지 판정

        // 처음 위치로 이동

        private void MoveBack()
        {
            // 처음 위치로 이동하는 로직
            transform.position = Vector3.Lerp(transform.position, _InitialPosition.position, 0.5f);
        }
    }
}