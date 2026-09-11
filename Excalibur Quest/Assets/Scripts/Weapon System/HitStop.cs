using UnityEngine;
using System.Collections;

public class HitStop : MonoBehaviour
{
    public static HitStop Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void Play(float duration)
    {
        StartCoroutine(HitStopRoutine(duration));
    }

    private IEnumerator HitStopRoutine(float duration)
    {
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f;
    }
}
