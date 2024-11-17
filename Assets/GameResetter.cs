using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameResetter : MonoBehaviour
{
    public AudioClip snoreClip;
    public AudioSource increasingSoundSource;
    public AudioSource ambientSoundSource;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to the player
        if (other.CompareTag("Player"))
        {
            StartCoroutine(PlaySnoreAndReset());
        }
    }

    private IEnumerator PlaySnoreAndReset()
    {
        // Stop the other sounds
        if (increasingSoundSource != null)
        {
            increasingSoundSource.Stop();
        }

        if (ambientSoundSource != null)
        {
            ambientSoundSource.Stop();
        }

        // Play the snore sound
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = snoreClip;
        audioSource.Play();

        // Wait for 2 seconds
        yield return new WaitForSeconds(4.0f);

        // Stop the snore sound (in case it is longer than 2 seconds)
        audioSource.Stop();

        // Reset the game
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
