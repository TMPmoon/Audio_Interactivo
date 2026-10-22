using UnityEngine;
using UnityEngine.Audio;

public class SnapshotTransition : MonoBehaviour
{
    [Header("Configuración del Mixer")]
    public AudioMixerSnapshot tensionSnapshot;

    [Header("Condición")]
    public GameObject objetoFuego; // Arrastrá el fuego acá

    [Header("Parámetros")]
    public float transitionTime = 2.0f;
    private bool hasTriggered = false;

    private void Update()
    {
        // Si ya se disparó la música, no hace nada más
        if (hasTriggered) return;

        // Comprueba si el fuego existe y se activó en la escena
        if (objetoFuego != null && objetoFuego.activeInHierarchy)
        {
            hasTriggered = true;
            tensionSnapshot.TransitionTo(transitionTime);
            Debug.Log("<color=red>[MÚSICA] Fuego encendido detectado: Transición a música de tensión.</color>");
        }
    }
}