using UnityEngine;
using UnityEngine.Events;

public class GlobalEventComponent : MonoBehaviour
{

    public event UnityAction OnPlayerHit;

    void Awake()
    {
        GlobalEventManager.SetGlobalEvent(this);
    }

    void OnDestroy()
    {
        if (GlobalEventManager.GlobalEvent == this)
        {
            GlobalEventManager.SetGlobalEvent(null);
        }
    }

    public void OnPlayerHitInvoke()
    {
        OnPlayerHit?.Invoke();
    }
}
