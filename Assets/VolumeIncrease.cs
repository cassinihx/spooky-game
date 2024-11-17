using UnityEngine;
using System.Collections;

public class VolumeIncrease : MonoBehaviour
{
    public float maxVolume = 1.0f;
    public float duration = 60.0f;

    private AudioSource _audioSource;
    private float _initialVolume;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            return;
        }

        _initialVolume = _audioSource.volume;
        StartCoroutine(IncreaseVolume());
    }

    private IEnumerator IncreaseVolume()
    {
        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            _audioSource.volume = Mathf.Lerp(_initialVolume, maxVolume, elapsedTime / duration);
            yield return null;
        }

        _audioSource.volume = maxVolume;
    }
}
