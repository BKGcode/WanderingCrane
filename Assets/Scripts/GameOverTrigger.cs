// Assets/Scripts/Player/GameOverTrigger.cs
using UnityEngine;

public class GameOverTrigger : MonoBehaviour
{
    public float fallThreshold = -10f;
    private BallController ballController;
    private UIManager uiManager;

    void Start()
    {
        ballController = FindObjectOfType<BallController>();
        uiManager = UIManager.Instance;
    }

    void Update()
    {
        if (ballController != null && ballController.transform.position.y < fallThreshold)
        {
            uiManager.ShowGameOver("¡Game Over!");
            // Opcional: Detener el movimiento de la pelota
            ballController.enabled = false;
            Rigidbody rb = ballController.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}
