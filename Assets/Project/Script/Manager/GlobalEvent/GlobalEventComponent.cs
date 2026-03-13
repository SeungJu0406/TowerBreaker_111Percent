using UnityEngine;
using UnityEngine.Events;

public class GlobalEventComponent : MonoBehaviour
{

    public event UnityAction OnPlayerHit;
    public event UnityAction OnDefenceStart;
    public event UnityAction OnDefenceEnd;

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

    public void OnDefenceStartInvoke()
    {
        OnDefenceStart?.Invoke();
    }

    public void OnDefenceEndInvoke()
    {
        OnDefenceEnd?.Invoke();
    }
}
