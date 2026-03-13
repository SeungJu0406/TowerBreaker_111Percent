using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{

    public event UnityAction OnPlayerHit;
    public event UnityAction OnDefenceStart;
    public event UnityAction OnDefenceEnd;

    void Awake()
    {
        Manager.SetEvent(this);
    }

    void OnDestroy()
    {
        if (Manager.Event == this)
        {
            Manager.SetEvent(null);
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
