using UnityEngine;

public class FireEscapeTrigger : MonoBehaviour
{
    [Header("Audio Diálogo (Bloque E - Voz)")]
    [Tooltip("AudioSource para la voz")]
    [SerializeField] private AudioSource voiceAudioSource;
    [Tooltip("Clip que indica escapar por la puerta principal")]
    [SerializeField] private AudioClip escapeDialogueClip;

    [Header("Objetivo")]
    [Tooltip("Referencia al script de la puerta de escape")]
    [SerializeField] private DoorEscapeTrigger doorEscape;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        // Detecta al jugador
        if (other.CompareTag("Player") || other.GetComponent<CharacterController>() != null)
        {
            triggered = true;

            // Reproduce la voz
            if (voiceAudioSource != null && escapeDialogueClip != null)
            {
                voiceAudioSource.PlayOneShot(escapeDialogueClip);
            }

            // Habilita la interacción con la puerta de salida
            if (doorEscape != null)
            {
                doorEscape.EnableEscape();
            }

            Debug.Log("<color=red>[FUEGO] ¡Incendio detectado! Hay que salir por la puerta principal.</color>");
        }
    }
}