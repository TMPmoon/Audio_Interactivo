using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickup : MonoBehaviour
{
    [Header("Configuración del Raycast")]
    [Tooltip("Distancia máxima para alcanzar el objeto")]
    [SerializeField] private float reachDistance = 2.5f;
    [Tooltip("Capas de objetos que se pueden agarrar")]
    [SerializeField] private LayerMask pickupMask = ~0;

    [Header("Punto de Agarre")]
    [Tooltip("Transform vacío hijo de la cámara donde se sostiene el objeto")]
    [SerializeField] private Transform holdPoint;
    [Tooltip("Velocidad de suavizado para seguir el punto de agarre")]
    [SerializeField] private float smoothSpeed = 15f;

    [Header("Audio (Bloque C)")]
    [SerializeField] private AudioSource pickupAudioSource;
    [SerializeField] private AudioClip pickupClip;
    [SerializeField] private AudioClip dropClip;
    [SerializeField] private float minPitch = 0.92f;
    [SerializeField] private float maxPitch = 1.08f;

    private Rigidbody heldRb;
    private Collider heldCollider;

    private void Update()
    {
        // Detecta la tecla 'E' (compatible con New Input System y Legacy)
        bool interactPressed = (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
                               || Input.GetKeyDown(KeyCode.E);

        if (interactPressed)
        {
            if (heldRb == null)
            {
                TryPickup();
            }
            else
            {
                Drop();
            }
        }
    }

    private void FixedUpdate()
    {
        // Traslada el objeto suavemente hacia el punto de la mano
        if (heldRb != null && holdPoint != null)
        {
            Vector3 targetPosition = holdPoint.position;
            heldRb.MovePosition(Vector3.Lerp(heldRb.position, targetPosition, smoothSpeed * Time.fixedDeltaTime));
            heldRb.MoveRotation(Quaternion.Lerp(heldRb.rotation, holdPoint.rotation, smoothSpeed * Time.fixedDeltaTime));
        }
    }

    private void TryPickup()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, reachDistance, pickupMask))
        {
            if (hit.collider.CompareTag("Grabbable"))
            {
                Rigidbody rb = hit.collider.attachedRigidbody;
                if (rb != null)
                {
                    heldRb = rb;
                    heldCollider = hit.collider;

                    heldRb.useGravity = false;
                    heldRb.linearDamping = 10f; // Evita que oscile descontroladamente
                    heldRb.angularDamping = 10f;

                    // Ignora colisiones temporales contra el CharacterController para no empujar la cámara
                    CharacterController cc = GetComponentInParent<CharacterController>();
                    if (cc != null && heldCollider != null)
                    {
                        Physics.IgnoreCollision(heldCollider, cc, true);
                    }

                    PlayVariedSound(pickupClip);
                }
            }
        }
    }

    public void Drop()
    {
        if (heldRb == null) return;

        heldRb.useGravity = true;
        heldRb.linearDamping = 0.05f;
        heldRb.angularDamping = 0.05f;

        CharacterController cc = GetComponentInParent<CharacterController>();
        if (cc != null && heldCollider != null)
        {
            Physics.IgnoreCollision(heldCollider, cc, false);
        }

        PlayVariedSound(dropClip);

        heldRb = null;
        heldCollider = null;
    }

    private void PlayVariedSound(AudioClip clip)
    {
        if (pickupAudioSource == null || clip == null) return;
        pickupAudioSource.pitch = Random.Range(minPitch, maxPitch);
        pickupAudioSource.PlayOneShot(clip);
    }

    public Rigidbody GetHeldObject() => heldRb;
}