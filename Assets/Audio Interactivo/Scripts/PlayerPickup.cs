using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickup : MonoBehaviour
{
    [Header("Configuración del Raycast")]
    [SerializeField] private float reachDistance = 2.5f;
    [SerializeField] private LayerMask interactionMask = ~0;

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
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, reachDistance, interactionMask))
        {
            // 1. ¿Estamos mirando a la hornalla?
            StoveController targetStove = hit.collider.GetComponentInParent<StoveController>();
            if (targetStove != null)
            {
                targetStove.InteractWithStove(this);
                return;
            }

            // 2. ¿Estamos mirando a la sartén (Grabbable) y no tenemos nada en mano?
            if (heldRb == null && hit.collider.CompareTag("Grabbable"))
            {
                // Condición: solo se puede levantar si la hornalla ya está encendida
                if (stove != null && !stove.IsLit)
                {
                    Debug.Log("<color=red>[AVISO] Primero tenés que prender la hornalla.</color>");
                    return;
                }

                PickupObject(hit);
                return;
            }
        }

        // 3. Si tenemos algo en la mano y apretamos 'E' al aire/piso, lo soltamos
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