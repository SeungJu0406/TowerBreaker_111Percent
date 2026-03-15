using System.Collections;
using UnityEngine;
using Utility;

public class HitStop : SingleTon<HitStop>
{
    protected override void InitAwake() { }

    public void Do(float duration, float shakeStrength = 0.1f)
    {
        StopAllCoroutines();
        StartCoroutine(HitStopRoutine(duration, shakeStrength));
    }

    private IEnumerator HitStopRoutine(float duration, float shakeStrength)
    {
        Time.timeScale = 0f;

        Camera cam = Camera.main;
        Vector3 originalPos = cam.transform.position;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float strength = shakeStrength * (1f - t);
            cam.transform.position = originalPos + (Vector3)(Random.insideUnitCircle * strength);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        cam.transform.position = originalPos;
        Time.timeScale = 1f;
    }
}
