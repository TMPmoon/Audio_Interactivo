using UnityEngine;

public class LivingZoneVoiceTrigger : MonoBehaviour
{
    [Header("Audio (Bloque E - Voz)")]
    [SerializeField] private AudioSource voiceAudioSource;
    [SerializeField] private AudioClip mustCrossChairClip; //'este perro...'

    private bool triggered = false;

   private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player") || other.GetComponent<CharacterController>() != null)
        {
            triggered = true;

            if (voiceAudioSource != null && mustCrossChairClip != null)
            {
                voiceAudioSource.PlayOneShot(mustCrossChairClip);
            }

            Debug.Log("<color=cyan>[VOZ] 'Tengo que prender la hornalla...'</color>");
        }

    }
}
