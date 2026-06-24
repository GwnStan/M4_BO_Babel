using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFlicker : MonoBehaviour
{
    [Header("Intensity Settings")]
    public float minIntensity = 0.5f;
    public float maxIntensity = 1.5f;

    [Header("Flicker Speed")]
    public float flickerSpeed = 0.1f;

    private Light pointLight;
    private float targetIntensity;

    void Start()
    {
        pointLight = GetComponent<Light>();
        targetIntensity = pointLight.intensity;
    }

    void Update()
    {
        // Pick a new random target intensity occasionally
        if (Random.value < flickerSpeed)
        {
            targetIntensity = Random.Range(minIntensity, maxIntensity);
        }

        // Smoothly move toward the target intensity
        pointLight.intensity = Mathf.Lerp(pointLight.intensity, targetIntensity, Time.deltaTime * 10f);
    }
}
