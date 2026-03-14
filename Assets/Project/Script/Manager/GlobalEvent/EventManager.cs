using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{

    public event UnityAction OnPlayerHit;
    public event UnityAction OnPlayerDied;
    public event UnityAction OnDefenceStart;
    public event UnityAction OnDefenceEnd;

    // 층의 모든 적이 죽었을 때 (Floor → FloorManager → 여기서 전파)
    public event UnityAction OnStageClear;
    // 층 전환 연출 시작 — Behaviour들의 입력 차단 + 플레이어 화면 밖 이동 트리거
    public event UnityAction OnStageTransitionStart;
    // 층 전환 연출 완료 — 입력 허용 + 플레이어 초기 위치 복귀 트리거
    public event UnityAction OnStageTransitionEnd;

    public event UnityAction OnGameClear;

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

    public void OnPlayerDiedInvoke()
    {
        OnPlayerDied?.Invoke();
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

    public void OnGameClearInvoke() => OnGameClear?.Invoke();
}
