using NSJ_Player;
using UnityEngine;

public class Boundary : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(Tag.Player))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            player.TakeDamage();
        }
    }
}
