using System.Collections;
using UnityEngine;
using Utility;

public class HitStop : SingleTon<HitStop>
{
    protected override void InitAwake() { }

    public void Do(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(HitStopRoutine(duration));
    }

    private IEnumerator HitStopRoutine(float duration)
    {
        Time.timeScale = 0f;
        yield return duration.RealSecond();
        Time.timeScale = 1f;
    }
}
