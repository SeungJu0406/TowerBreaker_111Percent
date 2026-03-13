using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{

    public event UnityAction OnPlayerHit;
    public event UnityAction OnDefenceStart;
    public event UnityAction OnDefenceEnd;
    public event UnityAction OnStageClear;
    public event UnityAction OnStageTransitionStart;
    public event UnityAction OnStageTransitionEnd;

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

    public void OnStageClearInvoke() => OnStageClear?.Invoke();
    public void OnStageTransitionStartInvoke() => OnStageTransitionStart?.Invoke();
    public void OnStageTransitionEndInvoke() => OnStageTransitionEnd?.Invoke();
}
