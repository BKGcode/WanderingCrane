using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Referencia")]
    [SerializeField] private Transform target;

    [Header("Configuración de Seguimiento")]
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

    private void LateUpdate()
    {
        if (target == null)
            return;

        SeguirPelota();
    }

    private void SeguirPelota()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        
        transform.position = smoothedPosition;
    }

    // Método para cargar la posición de la cámara
    public void CargarPosicion(Vector3 posicionPelota)
    {
        transform.position = posicionPelota + offset;
    }
}