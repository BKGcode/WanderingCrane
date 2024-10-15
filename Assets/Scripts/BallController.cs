// Assets/Scripts/Player/BallController.cs
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class BallController : MonoBehaviour
{
    [Header("Settings")]
    public float maxForce = 100f;
    public float forceMultiplier = 10f; // Para ajustar la sensibilidad del input
    public float maxChargeTime = 2f; // Tiempo máximo para cargar el golpe

    [Header("Trajectory")]
    public Trajectory trajectory;
    public int trajectoryResolution = 30;
    public float timeStep = 0.1f;

    [Header("Events")]
    public UnityEvent OnHit;

    private Rigidbody rb;
    private bool isCharging = false;
    private float chargeStartTime;
    private Vector3 aimDirection;
    private float currentForce;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;
    }

    void Update()
    {
        HandleAiming();
        HandleCharging();
    }

    private void HandleAiming()
    {
        // Obtener la posición del ratón en la pantalla
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.WorldToScreenPoint(transform.position).z;
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);

        // Calcular la dirección desde la pelota hacia la posición del ratón en el mundo
        aimDirection = (worldMousePos - transform.position).normalized;

        // Mostrar la trayectoria predecida
        if (trajectory != null && isCharging)
        {
            trajectory.ShowTrajectory(transform.position, aimDirection, currentForce, trajectoryResolution, timeStep);
        }
    }

    private void HandleCharging()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isCharging = true;
            chargeStartTime = Time.time;
            currentForce = 0f;
        }

        if (Input.GetMouseButton(0) && isCharging)
        {
            float chargeTime = Time.time - chargeStartTime;
            currentForce = Mathf.Min(chargeTime / maxChargeTime * maxForce, maxForce);

            // Actualizar la UI de fuerza
            UIManager.Instance.SetForce(currentForce, maxForce);

            // Actualizar la trayectoria mientras se carga
            if (trajectory != null)
            {
                trajectory.ShowTrajectory(transform.position, aimDirection, currentForce, trajectoryResolution, timeStep);
            }
        }

        if (Input.GetMouseButtonUp(0) && isCharging)
        {
            isCharging = false;
            HitBall();
        }
    }

    private void HitBall()
    {
        Vector3 force = aimDirection * currentForce;
        rb.AddForce(force, ForceMode.Impulse);
        OnHit?.Invoke();

        // Resetear la trayectoria
        if (trajectory != null)
        {
            trajectory.HideTrajectory();
        }

        // Resetear la UI de fuerza
        UIManager.Instance.UpdateForceUI();

        // Resetear la fuerza
        currentForce = 0f;
    }
}
