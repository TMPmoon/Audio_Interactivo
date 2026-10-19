using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class SimpleCameraController : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 3.5f;

    [Header("Sensibilidad del Mouse")]
    public float mouseSensitivity = 0.15f;

    [Header("Referencias Opcionales")]
    [Tooltip("Si tenés la cámara como hija, asignala acá. Si el script está en la misma cámara, dejalo vacío.")]
    [SerializeField] private Transform cameraTransform;

    [Header("Input Actions")]
    public InputAction moveAction;
    public InputAction lookAction;

    private float pitch = 0.0f; // Rotación vertical (cabeza)
    private float yaw = 0.0f;   // Rotación horizontal (cuerpo)
    private CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        if (cameraTransform == null)
        {
            cameraTransform = transform;
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector3 currentAngles = transform.localEulerAngles;
        yaw = currentAngles.y;
        pitch = currentAngles.x;
    }

    private void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();
    }

    private void Update()
    {
        // 1. Lectura del Mouse
        Vector2 lookInput = lookAction.ReadValue<Vector2>();

        yaw += lookInput.x * mouseSensitivity;
        pitch -= lookInput.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -80f, 80f); // Límite vertical

        // Aplicamos la rotación:
        // Si la cámara es el mismo objeto, rotamos en su sitio manteniendo el eje vertical puro
        transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);

        // 2. Movimiento WASD puro en el plano horizontal XZ
        Vector2 moveInput = moveAction.ReadValue<Vector2>();

        // Usamos solo el ángulo horizontal (yaw) para que mirar al piso no te frene ni te mueva hacia abajo
        Vector3 forward = Quaternion.Euler(0f, yaw, 0f) * Vector3.forward;
        Vector3 right = Quaternion.Euler(0f, yaw, 0f) * Vector3.right;

        Vector3 moveDirection = (forward * moveInput.y + right * moveInput.x).normalized;
        Vector3 motion = moveDirection * moveSpeed;

        // Gravedad adecuada: suave si toca el piso, caída real si está en el aire
        if (characterController.isGrounded)
        {
            motion.y = -2f; // Fuerza mínima constante para mantenerlo pegado al piso
        }
        else
        {
            motion.y = -9.81f;
        }

        characterController.Move(motion * Time.deltaTime);

        // Escape para liberar mouse en el editor
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
    }
}