using System.Collections;
using UnityEngine;
public class HitStop : MonoBehaviour
{
    public static HitStop Instance;
    void Awake() => Instance = this;

    public void Freeze(float seconds) =>
        StartCoroutine(FreezeFor(seconds));

    IEnumerator FreezeFor(float seconds)
    {
        Time.timeScale = 0f;              // the world stops
        yield return new WaitForSecondsRealtime(seconds);
        Time.timeScale = 1f;              // and resumes
    }
}
