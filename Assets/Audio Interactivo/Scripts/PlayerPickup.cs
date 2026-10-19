using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickup : MonoBehaviour
{
    [Header("Configuración del Raycast")]
    [SerializeField] private float reachDistance = 2.5f;
    [SerializeField] private LayerMask interactionMask = ~0;
    [SerializeField] private Camera playerCamera; // Referencia a la cámara para apuntar

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

    private Rigidbody heldRb;
    private Collider heldCollider;

    private void Awake()
    {
        // Si no se asignó la cámara a mano, la busca en los hijos
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
        // El rayo sale desde la posición de la cámara y en la dirección hacia donde estás mirando
        Vector3 rayOrigin = playerCamera != null ? playerCamera.transform.position : transform.position;
        Vector3 rayDirection = playerCamera != null ? playerCamera.transform.forward : transform.forward;

        Ray ray = new Ray(rayOrigin, rayDirection);

        if (Physics.Raycast(ray, out RaycastHit hit, reachDistance, interactionMask))
        {
            // 1. ¿Mirando a la hornalla?
            StoveController targetStove = hit.collider.GetComponentInParent<StoveController>();
            if (targetStove != null)
            {
                targetStove.InteractWithStove(this);
                return;
            }

            // 2. ¿Mirando a la sartén?
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

        // 3. Soltar si tenemos algo
        if (heldRb != null)
        {
            Drop();
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