using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Referencia a la bisagra")]
    [Tooltip("Arrastrá acá a PivotPuerta (el objeto hijo)")]
    [SerializeField] private Transform hingePoint;

    [Header("Configuración de Apertura")]
    [Tooltip("Grados que se abre (ej: 90 o -90)")]
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float duration = 1.0f;

    [Header("Configuración de Audio")]
    [Tooltip("El componente AudioSource que reproducirá los sonidos")]
    [SerializeField] private AudioSource doorAudioSource;
    [Tooltip("Sonido al abrir la puerta")]
    [SerializeField] private AudioClip openSound;
    [Tooltip("Sonido al cerrar la puerta")]
    [SerializeField] private AudioClip closeSound;

    private bool isOpen = false;
    private bool isAnimating = false;
    private Coroutine currentDoorCoroutine;

    public void ToggleDoor()
    {
        if (isAnimating) return; // Evita solapar movimientos mientras gira

        isOpen = !isOpen;

        // Reproducir el sonido correspondiente
        if (doorAudioSource != null)
        {
            if (isOpen && openSound != null)
            {
                doorAudioSource.PlayOneShot(openSound);
            }
            else if (!isOpen && closeSound != null)
            {
                doorAudioSource.PlayOneShot(closeSound);
            }
        }

        if (currentDoorCoroutine != null)
        {
            StopCoroutine(currentDoorCoroutine);
        }

        float targetDeltaAngle = isOpen ? openAngle : -openAngle;
        currentDoorCoroutine = StartCoroutine(AnimateDoor(targetDeltaAngle, duration));
    }

    private IEnumerator AnimateDoor(float totalAngle, float time)
    {
        isAnimating = true;

        float elapsed = 0f;
        float previousAngle = 0f;

        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / time);

            // Curva suave de entrada y salida (equivalente a EaseInOut)
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            float currentStepAngle = Mathf.Lerp(0f, totalAngle, smoothT);

            // Corrección del salto de línea que había en tu script original
            float delta = currentStepAngle - previousAngle;
            previousAngle = currentStepAngle;

            // Rota la puerta usando como eje de giro la posición de PivotPuerta
            if (hingePoint != null)
            {
                transform.RotateAround(hingePoint.position, Vector3.up, delta);
            }
            else
            {
                transform.Rotate(Vector3.up, delta, Space.Self);
            }

            yield return null;
        }

        isAnimating = false;
    }
}