using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickup : MonoBehaviour
{
    [Header("Configuración del Raycast")]
    [SerializeField] private float reachDistance = 2.5f;
    [SerializeField] private LayerMask interactionMask = ~0;
    [SerializeField] private Camera playerCamera;

    [Header("Punto de Agarre")]
    [SerializeField] private Transform holdPoint;
    [SerializeField] private float smoothSpeed = 18f;

    [Header("Referencia de la Hornalla")]
    [SerializeField] private StoveController stove;

    [Header("Audio (Bloque C - SFX)")]
    [SerializeField] private AudioSource pickupAudioSource;
    [SerializeField] private AudioClip pickupClip;
    [SerializeField] private AudioClip dropClip;
    [SerializeField] private float minPitch = 0.92f;
    [SerializeField] private float maxPitch = 1.08f;

    [Header("Evento Puerta (Knock)")]
    [Tooltip("AudioSource ubicado en la puerta de entrada para que suene en 3D")]
    [SerializeField] private AudioSource doorAudioSource;
    [Tooltip("Clip del sonido de tocar la puerta (Toc Toc)")]
    [SerializeField] private AudioClip doorKnockClip;
    [SerializeField] private float doorKnockDelay = 2.0f;

    [Header("Diálogo Post Toc-Toc (Bloque E - Voz)")]
    [Tooltip("AudioSource ruteado al grupo Voz del Mixer")]
    [SerializeField] private AudioSource voiceAudioSource;
    [Tooltip("Clip donde el personaje reacciona al toc-toc (ej: '¿Quién será?')")]
    [SerializeField] private AudioClip reactionToKnockClip;
    [SerializeField] private float voiceDelayAfterKnock = 1.2f;

    [Header("Activador del Trigger de la Puerta")]
    [Tooltip("El collider que espera al jugador en la puerta")]
    [SerializeField] private DoorCheckTrigger doorCheckTrigger;

    private Rigidbody heldRb;
    private Collider heldCollider;
    private bool knockTriggered = false;

    private void Awake()
    {
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
        }
    }

    private void Update()
    {
        bool interactPressed = (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
                               || Input.GetKeyDown(KeyCode.E);

        if (interactPressed)
        {
            HandleInteraction();
        }
    }

    private void FixedUpdate()
    {
        if (heldRb != null && holdPoint != null)
        {
            heldRb.MovePosition(Vector3.Lerp(heldRb.position, holdPoint.position, smoothSpeed * Time.fixedDeltaTime));
            heldRb.MoveRotation(Quaternion.Lerp(heldRb.rotation, holdPoint.rotation, smoothSpeed * Time.fixedDeltaTime));
        }
    }

    private void HandleInteraction()
    {
        Vector3 rayOrigin = playerCamera != null ? playerCamera.transform.position : transform.position;
        Vector3 rayDirection = playerCamera != null ? playerCamera.transform.forward : transform.forward;

        Ray ray = new Ray(rayOrigin, rayDirection);

        if (Physics.Raycast(ray, out RaycastHit hit, reachDistance, interactionMask))
        {
            // 1. Hornalla
            StoveController targetStove = hit.collider.GetComponentInParent<StoveController>();
            if (targetStove != null)
            {
                bool hadPanInHand = (heldRb != null);

                targetStove.InteractWithStove(this);

                // Si colocamos la sartén, arranca la secuencia del toc-toc
                if (hadPanInHand && targetStove.HasPan && !knockTriggered)
                {
                    knockTriggered = true;
                    StartCoroutine(TriggerDoorKnockSequence());
                }
                return;
            }

            // 2. Puerta
            Door door = hit.collider.GetComponentInParent<Door>();
            if (door != null)
            {
                door.ToggleDoor();
                return;
            }

            // 3. Sartén
            if (heldRb == null && hit.collider.CompareTag("Grabbable"))
            {
                if (stove != null && !stove.IsLit)
                {
                    Debug.Log("<color=red>[AVISO] Primero tenés que prender la hornalla.</color>");
                    return;
                }

                PickupObject(hit);
                return;
            }
        }

        // 4. Soltar
        if (heldRb != null)
        {
            Drop();
        }
    }

    private IEnumerator TriggerDoorKnockSequence()
    {
        yield return new WaitForSeconds(doorKnockDelay);

        // Suena el golpe a la puerta
        if (doorAudioSource != null && doorKnockClip != null)
        {
            doorAudioSource.pitch = Random.Range(0.95f, 1.05f);
            doorAudioSource.PlayOneShot(doorKnockClip);
            Debug.Log("<color=yellow>[PUERTA] Alguien tocó la puerta de entrada...</color>");
        }

        // Habilita el collider de la puerta para que reaccione al ir
        if (doorCheckTrigger != null)
        {
            doorCheckTrigger.EnableTrigger();
        }

        // Pausa breve y entra el diálogo del personaje
        yield return new WaitForSeconds(voiceDelayAfterKnock);

        if (voiceAudioSource != null && reactionToKnockClip != null)
        {
            voiceAudioSource.PlayOneShot(reactionToKnockClip);
            Debug.Log("<color=cyan>[VOZ] Diálogo al escuchar la puerta ('¿Quién será?').</color>");
        }
    }

    private void PickupObject(RaycastHit hit)
    {
        heldRb = hit.collider.attachedRigidbody;
        if (heldRb == null) return;

        heldCollider = hit.collider;
        heldRb.isKinematic = true;
        heldRb.useGravity = false;
        heldRb.transform.SetParent(null);

        CharacterController cc = GetComponentInParent<CharacterController>();
        if (cc != null && heldCollider != null)
        {
            Physics.IgnoreCollision(heldCollider, cc, true);
        }

        PlayAudio(pickupClip);
    }

    public void Drop()
    {
        if (heldRb == null) return;

        heldRb.isKinematic = false;
        heldRb.useGravity = true;

        CharacterController cc = GetComponentInParent<CharacterController>();
        if (cc != null && heldCollider != null)
        {
            Physics.IgnoreCollision(heldCollider, cc, false);
        }

        PlayAudio(dropClip);

        heldRb = null;
        heldCollider = null;
    }

    public void ClearHeldObject()
    {
        CharacterController cc = GetComponentInParent<CharacterController>();
        if (cc != null && heldCollider != null)
        {
            Physics.IgnoreCollision(heldCollider, cc, false);
        }
        heldRb = null;
        heldCollider = null;
    }

    private void PlayAudio(AudioClip clip)
    {
        if (pickupAudioSource == null || clip == null) return;
        pickupAudioSource.pitch = Random.Range(minPitch, maxPitch);
        pickupAudioSource.PlayOneShot(clip);
    }

    public Rigidbody GetHeldObject() => heldRb;
}