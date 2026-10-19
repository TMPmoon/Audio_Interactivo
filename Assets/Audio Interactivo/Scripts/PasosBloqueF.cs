using UnityEngine;

public class PasosBloqueF : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private float stepInterval = 0.5f;

    private float stepTimer = 0f;
    private int lastClipIndex = -1;

    private void Update()
    {
        // 1. Detectar si el jugador se está desplazando horizontalmente en el suelo
        Vector3 horizontalVelocity = new Vector3(controller.velocity.x, 0f, controller.velocity.z);
        bool isMoving = horizontalVelocity.magnitude > 0.2f && controller.isGrounded;

        if (isMoving)
        {
            stepTimer += Time.deltaTime;
            if (stepTimer >= stepInterval)
            {
                PlayVariedFootstep();
                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = stepInterval; // Listo para sonar en cuanto vuelva a dar un paso
        }
    }

    private void PlayVariedFootstep()
    {
        if (footstepClips == null || footstepClips.Length == 0) return;

        // 2. Lógica condicional: seleccionar un clip aleatorio sin repetir el anterior
        int newIndex;
        do
        {
            newIndex = Random.Range(0, footstepClips.Length);
        } while (newIndex == lastClipIndex && footstepClips.Length > 1);

        lastClipIndex = newIndex;

        // 3. Modificación en tiempo real: asignación de clip y variación sutil de pitch
        audioSource.clip = footstepClips[newIndex];
        audioSource.pitch = Random.Range(0.92f, 1.08f);
        audioSource.Play();
    }
}
