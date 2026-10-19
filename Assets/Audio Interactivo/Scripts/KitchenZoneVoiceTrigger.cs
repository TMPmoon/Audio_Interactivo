using UnityEngine;

public class KitchenZoneVoiceTrigger : MonoBehaviour
{
    [Header("Audio (Bloque E - Voz)")]
    [SerializeField] private AudioSource voiceAudioSource;
    [SerializeField] private AudioClip mustIgniteStoveClip; // 'Tengo que prender la hornalla'

    [Header("Condición")]
    [SerializeField] private StoveController stove;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        // Si la hornalla ya fue encendida, no decimos la línea
        if (stove != null && stove.IsLit) return;

        if (other.CompareTag("Player") || other.GetComponent<CharacterController>() != null)
        {
            triggered = true;

            if (voiceAudioSource != null && mustIgniteStoveClip != null)
            {
                voiceAudioSource.PlayOneShot(mustIgniteStoveClip);
            }

            Debug.Log("<color=cyan>[VOZ] 'Tengo que prender la hornalla...'</color>");
        }
    }
}