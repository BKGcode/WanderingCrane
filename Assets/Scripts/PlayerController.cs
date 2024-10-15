// Assets/Scripts/Player/PlayerController.cs
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    public PlayerSettings playerSettings;

    [Header("Events")]
    public UnityEvent OnJump;
    public UnityEvent OnRunStart;
    public UnityEvent OnRunStop;

    private Rigidbody rb;
    private bool isRunning = false;

    private float horizontalInput;
    private float verticalInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (playerSettings == null)
        {
            Debug.LogError("PlayerSettings no está asignado en el PlayerController.");
        }
    }

    private void Update()
    {
        HandleMovementInput();
        HandleRunInput();
        HandleJumpInput();
    }

    private void HandleMovementInput()
    {
        horizontalInput = Input.GetAxis("Horizontal"); // A/D o Flechas Izquierda/Derecha
        verticalInput = Input.GetAxis("Vertical"); // W/S o Flechas Arriba/Abajo

        Vector3 movement = new Vector3(horizontalInput, 0.0f, verticalInput).normalized;
        float currentSpeed = isRunning ? playerSettings.runSpeed : playerSettings.walkSpeed;

        Vector3 velocity = movement * currentSpeed;
        velocity.y = rb.velocity.y; // Preserva la velocidad vertical existente

        rb.velocity = velocity;
    }

    private void HandleRunInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isRunning = true;
            OnRunStart?.Invoke();
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isRunning = false;
            OnRunStop?.Invoke();
        }
    }

    private void HandleJumpInput()
    {
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.AddForce(Vector3.up * playerSettings.jumpForce, ForceMode.Impulse);
            OnJump?.Invoke();
        }
    }

    private bool IsGrounded()
    {
        // Verificación simple de si el jugador está en el suelo usando Raycast
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
}
