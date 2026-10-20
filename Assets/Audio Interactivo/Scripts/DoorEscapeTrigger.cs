using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoorEscapeTrigger : MonoBehaviour
{
    [Header("Ajustes del Fade Out")]
    [Tooltip("Duración en segundos del fundido a negro")]
    [SerializeField] private float fadeDuration = 2.0f;
    [Tooltip("Color del fundido")]
    [SerializeField] private Color fadeColor = Color.black;

    [Header("Audio Opcional (SFX)")]
    [Tooltip("Sonido al abrir la puerta o escapar")]
    [SerializeField] private AudioSource doorAudioSource;
    [SerializeField] private AudioClip doorOpenClip;

    private bool canEscape = false;
    private bool isPlayerAtDoor = false;
    private bool fadeStarted = false;
    private float currentAlpha = 0f;
    private Texture2D fadeTexture;

    private void Awake()
    {
        // Crea una textura 1x1 lisa para dibujar el fade en pantalla
        fadeTexture = new Texture2D(1, 1);
        fadeTexture.SetPixel(0, 0, Color.white);
        fadeTexture.Apply();
    }

    public void EnableEscape()
    {
        canEscape = true;
        Debug.Log("<color=green>[PUERTA] La salida principal ya está lista para usarse.</color>");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<CharacterController>() != null)
        {
            isPlayerAtDoor = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<CharacterController>() != null)
        {
            isPlayerAtDoor = false;
        }
    }

    private void Update()
    {
        if (!canEscape || !isPlayerAtDoor || fadeStarted) return;

        // Comprobación de tecla 'E' (New Input System y Legacy)
        bool pressedE = (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
                        || Input.GetKeyDown(KeyCode.E);

        if (pressedE)
        {
            fadeStarted = true;

            if (doorAudioSource != null && doorOpenClip != null)
            {
                doorAudioSource.PlayOneShot(doorOpenClip);
            }

            StartCoroutine(DoFadeOut());
        }
    }

    private IEnumerator DoFadeOut()
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            currentAlpha = Mathf.Clamp01(timer / fadeDuration);
            yield return null;
        }

        currentAlpha = 1f;
        Debug.Log("<color=cyan>[JUEGO] Fade Out completado. Nivel/Escape finalizado.</color>");

        // Aquí puedes cargar la siguiente escena si lo necesitas:
        // UnityEngine.SceneManagement.SceneManager.LoadScene("NombreEscena");
    }

    private void OnGUI()
    {
        // Si el alpha es mayor a 0, dibuja el rectángulo sobre toda la pantalla
        if (currentAlpha > 0f)
        {
            GUI.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, currentAlpha);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeTexture);
        }
    }
}