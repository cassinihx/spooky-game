using System.Collections;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    private Light _light;

    [Tooltip("Minimum time the light stays on or off in seconds.")]
    public float minTime = 0.05f;

    [Tooltip("Maximum time the light stays on or off in seconds.")]
    public float maxTime = 0.2f;

    private void Start()
    {
        _light = GetComponent<Light>();
        if (_light == null)
        {
            Debug.LogError("LightFlicker script requires a Light component.");
            return;
        }
        
        // Start the flickering coroutine
        StartCoroutine(Flicker());
    }

    private IEnumerator Flicker()
    {
        while (true)
        {
            // Randomly choose whether the light should be on or off
            _light.enabled = !_light.enabled;

            // Wait for a random duration between minTime and maxTime
            float waitTime = Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(waitTime);
        }
    }
}
