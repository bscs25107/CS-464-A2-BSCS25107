using UnityEngine;

public class Camerashake : MonoBehaviour
{
    public float maxOffset = 0.5f, decay = 1.5f;
    float trauma;                        
    Vector3 rest;

    void Awake() => rest = transform.localPosition;
    public void AddTrauma(float amount) =>
        trauma = Mathf.Clamp01(trauma + amount);

    void LateUpdate()
    {
        float shake = trauma * trauma;   
        float t = Time.unscaledTime * 25f;
        float x = Mathf.PerlinNoise(t, 0f) * 2f - 1f;
        float y = Mathf.PerlinNoise(0f, t) * 2f - 1f;
        Vector3 offset = new Vector3(x, y) * (maxOffset * shake);
        transform.localPosition = rest + offset;
        trauma -= decay * Time.unscaledDeltaTime;
        if (trauma < 0f) trauma = 0f;
    }
}
