// Assets/Scripts/UI/UIManager.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Force UI")]
    public Slider forceSlider;
    public TextMeshProUGUI forceText;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;

    private BallController ballController;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Opcional: Hacer persistente entre escenas
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ballController = FindObjectOfType<BallController>();
        if (ballController != null)
        {
            ballController.OnHit.AddListener(UpdateForceUI);
        }
    }

    public void UpdateForceUI()
    {
        // Este método puede ser utilizado para resetear o actualizar la UI después de golpear
        if (forceSlider != null)
        {
            forceSlider.value = 0f;
        }

        if (forceText != null)
        {
            forceText.text = "Force: 0";
        }
    }

    public void SetForce(float force, float maxForce)
    {
        if (forceSlider != null)
        {
            forceSlider.value = force / maxForce;
        }

        if (forceText != null)
        {
            forceText.text = $"Force: {force:0}";
        }
    }

    public void ShowGameOver(string message)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (gameOverText != null)
            {
                gameOverText.text = message;
            }
        }
    }

    public void HideGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }
}
