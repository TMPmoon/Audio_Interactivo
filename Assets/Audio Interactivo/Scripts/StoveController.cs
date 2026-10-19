using UnityEngine;

public class StoveController : MonoBehaviour
{
    [Header("Punto de Encastre")]
    [Tooltip("Punto vacío donde se acomoda la sartén")]
    [SerializeField] private Transform panSnapPoint;

    [Header("Audio Puntual (Bloque C - SFX)")]
    [SerializeField] private AudioSource stoveSfxAudioSource;
    [SerializeField] private AudioClip igniteStoveClip; // Chispa / encendido
    [SerializeField] private AudioClip placePanClip;    // Apoyar sartén

    [Header("Audio Continuo (Bloque D - 3D Loop)")]
    [Tooltip("AudioSource con el fuego de la hornalla en loop")]
    [SerializeField] private AudioSource smallFireLoopSource;

    [Header("Audio Diálogos (Bloque E - Voz)")]
    [Tooltip("AudioSource ruteado al grupo Voz del Mixer")]
    [SerializeField] private AudioSource voiceAudioSource;
    [SerializeField] private AudioClip placePanVoiceClip; // 'Ahora sí, a poner la sartén'

    private bool isLit = false;
    private bool hasPan = false;

    public bool IsLit => isLit;
    public bool HasPan => hasPan;

    public void InteractWithStove(PlayerPickup playerPickup)
    {
        // 1. Si está apagada, la encendemos
        if (!isLit)
        {
            isLit = true;

            // SFX de chispa / prender
            if (stoveSfxAudioSource != null && igniteStoveClip != null)
            {
                stoveSfxAudioSource.PlayOneShot(igniteStoveClip);
            }

            // Arrancar el fuego constante en loop
            if (smallFireLoopSource != null)
            {
                smallFireLoopSource.Play();
            }

            // Voz: "Ahora sí, a poner la sartén"
            if (voiceAudioSource != null && placePanVoiceClip != null)
            {
                voiceAudioSource.PlayOneShot(placePanVoiceClip);
            }

            Debug.Log("<color=yellow>[HORNALLA] Encendida. Ahora sí, a poner la sartén.</color>");
            return;
        }

        // 2. Si ya está prendida y traemos la sartén, la colocamos
        if (isLit && !hasPan && playerPickup != null && playerPickup.GetHeldObject() != null)
        {
            Rigidbody pan = playerPickup.GetHeldObject();
            playerPickup.ClearHeldObject();

            pan.isKinematic = true;
            pan.useGravity = false;

            Transform target = panSnapPoint != null ? panSnapPoint : transform;
            pan.transform.position = target.position;
            pan.transform.rotation = target.rotation;
            pan.transform.SetParent(target);

            hasPan = true;

            // SFX de apoyar sartén
            if (stoveSfxAudioSource != null && placePanClip != null)
            {
                stoveSfxAudioSource.PlayOneShot(placePanClip);
            }

            Debug.Log("<color=orange>[HORNALLA] Sartén colocada en el fuego caliente.</color>");
        }
    }

    // Para cuando descubras el incendio más adelante
    public void ExtinguishSmallFire()
    {
        if (smallFireLoopSource != null && smallFireLoopSource.isPlaying)
        {
            smallFireLoopSource.Stop();
        }
    }
}