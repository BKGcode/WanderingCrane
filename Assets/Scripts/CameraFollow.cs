using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Referencia")]
    [SerializeField] private Transform target;        // Referencia al Transform de la pelota

    [Header("Configuración de Seguimiento")]
    [SerializeField] private float smoothSpeed = 0.125f; // Velocidad de suavizado para el seguimiento
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f); // Desplazamiento de la cámara respecto a la pelota

    private void LateUpdate()
    {
        if (target == null)
            return;

        // Lógica de seguimiento
        SeguirPelota();
    }

    private void SeguirPelota()
    {
        // Posición objetivo con el offset
        Vector3 desiredPosition = target.position + offset;
        // Posición suavizada
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        
        transform.position = smoothedPosition;
    }
}
