using UnityEngine;

public class DoorCheckTrigger : MonoBehaviour
{
    [Header("Audio Diálogo (Bloque E - Voz)")]
    [Tooltip("AudioSource ruteado al grupo Voz del Mixer")]
    [SerializeField] private AudioSource voiceAudioSource;
    [Tooltip("Clip donde dice que no hay nadie y tiene que volver (ej: 'No hay nadie... ¿y ese olor?')")]
    [SerializeField] private AudioClip nobodyAtDoorClip;

    [Header("Incendio (Bloque D - 3D y Visual)")]
    [Tooltip("Arrastrá acá el prefab/GameObject Fire001")]
    [SerializeField] private GameObject fire001Prefab;
    [Tooltip("AudioSource del fuego grande ubicado en Fire001")]
    [SerializeField] private AudioSource bigFireAudioSource;

    [Header("Referencia de la Hornalla")]
    [Tooltip("Para apagar el fueguito chico de la hornalla")]
    [SerializeField] private StoveController stove;

    private bool canTrigger = false;
    private bool alreadyTriggered = false;

    // Llamado desde PlayerPickup tras escuchar el Toc-Toc
    public void EnableTrigger()
    {
        canTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canTrigger || alreadyTriggered) return;

        // Detecta al jugador
        if (other.CompareTag("Player") || other.GetComponent<CharacterController>() != null || other.GetComponentInParent<CharacterController>() != null)
        {
            alreadyTriggered = true;

            // 1. Diálogo de la puerta ("No hay nadie... ¿qué es ese ruido?")
            if (voiceAudioSource != null && nobodyAtDoorClip != null)
            {
                voiceAudioSource.PlayOneShot(nobodyAtDoorClip);
                Debug.Log("<color=cyan>[VOZ PUERTA] 'No hay nadie... ¿y ese olor?'</color>");
            }

            // 2. Apagar el fuego suave de la hornalla
            if (stove != null)
            {
                stove.ExtinguishSmallFire();
            }

            // 3. Activar el prefab Fire001
            if (fire001Prefab != null)
            {
                fire001Prefab.SetActive(true);
            }

            // 4. Reproducir el AudioSource del fuego
            if (bigFireAudioSource != null)
            {
                bigFireAudioSource.Play();
                Debug.Log("<color=red>[FUEGO] Fire001 encendido y sonando a lo lejos.</color>");
            }
        }
    }
}