using UnityEngine;
using UnityEngine.Audio;

public class SnapshotTransition : MonoBehaviour
{
    [Header("Configuración del Mixer")]
    public AudioMixerSnapshot tensionSnapshot;

    [Header("Condición")]
    public GameObject objetoFuego; 

    [Header("Parámetros")]
    public float transitionTime = 2.0f;
    private bool hasTriggered = false;

    private void Update()
    {
        
        if (hasTriggered) return;

        
        if (objetoFuego != null && objetoFuego.activeInHierarchy)
        {
            hasTriggered = true;
            tensionSnapshot.TransitionTo(transitionTime);
            Debug.Log("<color=red>[MÚSICA] Fuego encendido detectado: Transición a música de tensión.</color>");
        }
    }
}