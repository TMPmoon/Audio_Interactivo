using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class SimpleCameraController : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 3.5f;

    [Header("Sensibilidad del Mouse")]
    public float mouseSensitivity = 0.15f; // Ajustá este valor a tu gusto

    [Header("Input Actions")]
    public InputAction moveAction;
    public InputAction lookAction;

    private float rotationX = 0.0f;
    private float rotationY = 0.0f;
    private CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        // Bloquea el cursor en el centro de la pantalla para control FPS fluido
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Inicializa las rotaciones con los ángulos actuales de la cámara
        Vector3 currentAngles = transform.localEulerAngles;
        rotationX = currentAngles.x;
        rotationY = currentAngles.y;
    }

    private void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();
    }

    void Update()
    {
        // 1. Mirar con el mouse (Horizontal y Vertical)
        Vector2 lookInput = lookAction.ReadValue<Vector2>();

        rotationY += lookInput.x * mouseSensitivity; // Giro izquierda / derecha
        rotationX -= lookInput.y * mouseSensitivity; // Mirar arriba / abajo
        rotationX = Mathf.Clamp(rotationX, -85f, 85f); // Límite para no dar la vuelta completa

        transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);

        // 2. Movimiento WASD alineado a la vista
        Vector2 moveInput = moveAction.ReadValue<Vector2>();

        // Direcciones en el plano horizontal (sin volar)
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = (forward * moveInput.y + right * moveInput.x).normalized;
        Vector3 motion = moveDirection * moveSpeed;

        // Gravedad suave para mantener la cámara apoyada en el piso
        motion.y = -9.81f;

        characterController.Move(motion * Time.deltaTime);

        // Escape para liberar el cursor si necesitás tocar algo en el editor
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