using System.Collections;
using UnityEngine;

public class DynamicMusicTransition : MonoBehaviour
{
    [Header("Configuración de Audio")]
    public AudioSource calmMusic;
    public AudioSource tenseMusic;

    [Header("Parámetros de Transición")]
    public float fadeDuration = 2.0f; // Tiempo en segundos que dura el cambio
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        // Verifica que quien cruza la puerta sea el jugador
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            StartCoroutine(CrossfadeMusic());
        }
    }

    private IEnumerator CrossfadeMusic()
    {
        float currentTime = 0;

        // Aseguramos que la música de tensión empiece con volumen 0
        tenseMusic.volume = 0f;
        if (!tenseMusic.isPlaying)
        {
            tenseMusic.Play();
        }

        // Fundido cruzado gradual
        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            float lerpFactor = currentTime / fadeDuration;

            calmMusic.volume = Mathf.Lerp(1f, 0f, lerpFactor);
            tenseMusic.volume = Mathf.Lerp(0f, 1f, lerpFactor);

            yield return null;
        }

        // Nos aseguramos de apagar la música calmada al terminar
        calmMusic.volume = 0f;
        calmMusic.Stop();
        tenseMusic.volume = 1f;
    }
}