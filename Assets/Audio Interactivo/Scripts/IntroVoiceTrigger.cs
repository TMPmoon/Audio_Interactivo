using UnityEngine;

public class IntroVoiceTrigger : MonoBehaviour
{
    [Header("Audio (Bloque E - Voz)")]
    [SerializeField] private AudioSource voiceAudioSource;
    [SerializeField] private AudioClip milanesaVoiceClip;

    private bool triggered = false;

    private void Start()
    {
        // Si arranca directamente parado en la puerta, se reproduce al iniciar
        TriggerVoice();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<CharacterController>() != null)
        {
            TriggerVoice();
        }
    }

    private void TriggerVoice()
    {
        if (triggered) return;
        triggered = true;

        if (voiceAudioSource != null && milanesaVoiceClip != null)
        {
            voiceAudioSource.PlayOneShot(milanesaVoiceClip);
            Debug.Log("<color=cyan>[VOZ] 'Qué ganas de una milanesa...'</color>");
        }
    }
}