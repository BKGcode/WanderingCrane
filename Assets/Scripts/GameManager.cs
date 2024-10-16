using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Referencias")]
    [SerializeField] private Ball ball;
    [SerializeField] private GameObject gameOverPopup;
    [SerializeField] private AudioSource audioSource;

    [Header("Configuración")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private Vector3 ballStartPosition = Vector3.zero;
    [SerializeField] private AudioClip gameOverSound;

    private float elapsedTime = 0f;
    private int coins = 0;
    private bool isGameOver = false;

    public bool IsGameOver => isGameOver;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void Start()
    {
        LoadGame();
    }

    private void Update()
    {
        if (!isGameOver)
        {
            elapsedTime += Time.deltaTime;
        }
    }

    public void AddCoin()
    {
        coins++;
        SaveGame();
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Time.timeScale = 0f;
        
        if (gameOverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(gameOverSound);
        }
        
        if (gameOverPopup != null)
        {
            gameOverPopup.SetActive(true);
        }

        SaveGame();
    }

    public void ResetGame()
    {
        elapsedTime = 0f;
        coins = 0;
        isGameOver = false;

        if (ball != null)
        {
            ball.ResetBall(ballStartPosition);
        }

        if (gameOverPopup != null)
        {
            gameOverPopup.SetActive(false);
        }

        Time.timeScale = 1f;
        SaveGame();
    }

    public void ExitToMainMenu()
    {
        SaveGame();
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void SaveGame()
    {
        if (ball != null)
        {
            GameData data = new GameData
            {
                time = elapsedTime,
                coins = coins,
                ballPosition = ball.transform.position,
                ballVelocity = ball.GetComponent<Rigidbody2D>().velocity
            };
            SaveSystem.SaveGame(data);
        }
        else
        {
            Debug.LogWarning("No se puede guardar el juego porque la referencia a la pelota es nula.");
        }
    }

    public void LoadGame()
    {
        GameData data = SaveSystem.LoadGame();
        if (data != null)
        {
            elapsedTime = data.time;
            coins = data.coins;
            if (ball != null)
            {
                ball.LoadBallData(data.ballPosition, data.ballVelocity);
            }
            else
            {
                Debug.LogWarning("No se puede cargar la posición de la pelota porque la referencia es nula.");
            }
        }
        else
        {
            ResetGame();
        }
    }

    public void PauseGame()
    {
        if (!isGameOver)
        {
            Time.timeScale = 0f;
            // Aquí puedes activar un menú de pausa si lo tienes
        }
    }

    public void ResumeGame()
    {
        if (!isGameOver)
        {
            Time.timeScale = 1f;
            // Aquí puedes desactivar el menú de pausa si lo tienes
        }
    }

    public int GetCoins()
    {
        return coins;
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    public void SetBallStartPosition(Vector3 position)
    {
        ballStartPosition = position;
    }

    public Vector3 GetBallStartPosition()
    {
        return ballStartPosition;
    }
}