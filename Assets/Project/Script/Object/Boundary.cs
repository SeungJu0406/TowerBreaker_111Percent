using UnityEngine;

public class Boundary : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Tag.Player))
        {
            GlobalEventManager.GlobalEvent.OnPlayerHitInvoke();
        }
    }
}
