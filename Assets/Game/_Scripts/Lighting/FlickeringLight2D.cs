using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlickeringLight2D : MonoBehaviour
{
    [SerializeField] private Light2D targetLight;

    [Header("Flicker Settings")]
    [SerializeField] private float minIntensity = 0.7f;
    [SerializeField] private float maxIntensity = 1.3f;
    [SerializeField] private float flickerSpeed = 3f;

    private float randomOffset;

    private void Awake()
    {
        if (targetLight == null)
            targetLight = GetComponent<Light2D>();

        randomOffset = Random.Range(0f, 1000f);
    }

    private void Update()
    {
        if (targetLight == null)
            return;

        float noise = Mathf.PerlinNoise(
            randomOffset,
            Time.time * flickerSpeed
        );

        targetLight.intensity = Mathf.Lerp(
            minIntensity,
            maxIntensity,
            noise
        );
    }
}
