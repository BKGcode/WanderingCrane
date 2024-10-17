using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton para asegurar que solo hay una instancia de GameManager
    public static GameManager Instance { get; private set; }

    [Header("Game Data")]
    public GameData currentGameData;

    [Header("UI References")]
    [Tooltip("Referencia al UI de confirmación para reiniciar el juego.")]
    [SerializeField] private GameObject resetConfirmationUI;

    [Tooltip("Referencia al UI de carga.")]
    [SerializeField] private GameObject loadingUI;

    [Tooltip("Referencia al UI de diálogo.")]
    [SerializeField] private GameObject dialogueUI; // Añadido para el diálogo

    // Estado del juego
    private bool isGameOver = false;

    private void Awake()
    {
        // Implementación del patrón Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persistir entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Intentar cargar el juego al iniciar
        LoadGame();
    }

    /// <summary>
    /// Guarda el estado actual del juego.
    /// </summary>
    public void SaveGame()
    {
        if (currentGameData == null)
        {
            Debug.LogError("GameData está vacío. No se puede guardar el juego.");
            return;
        }

        try
        {
            SaveSystem.SaveGame(currentGameData);
            Debug.Log("Juego guardado correctamente.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error al guardar el juego: {ex.Message}");
        }
    }

    /// <summary>
    /// Carga el estado del juego desde el archivo de guardado.
    /// </summary>
    public void LoadGame()
    {
        try
        {
            GameData loadedData = SaveSystem.LoadGame();
            if (loadedData != null)
            {
                currentGameData = loadedData;
                ApplyGameData();
                Debug.Log("Juego cargado correctamente.");
            }
            else
            {
                Debug.LogWarning("No se encontró un archivo de guardado. Se iniciará un nuevo juego.");
                InitializeNewGame();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error al cargar el juego: {ex.Message}");
        }
    }

    /// <summary>
    /// Reinicia el juego, eliminando el archivo de guardado y reiniciando la escena.
    /// </summary>
    public void ResetGame()
    {
        // Mostrar UI de confirmación antes de reiniciar
        if (resetConfirmationUI != null)
        {
            resetConfirmationUI.SetActive(true);
        }
        else
        {
            Debug.LogError("Reset Confirmation UI no está asignado en GameManager.");
        }
    }

    /// <summary>
    /// Confirmación para reiniciar el juego.
    /// </summary>
    public void ConfirmResetGame()
    {
        try
        {
            SaveSystem.DeleteSaveFile();
            Debug.Log("Archivo de guardado eliminado.");
            InitializeNewGame();
            // Opcional: Reiniciar la escena actual
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error al reiniciar el juego: {ex.Message}");
        }
    }

    /// <summary>
    /// Cancela la operación de reinicio del juego.
    /// </summary>
    public void CancelResetGame()
    {
        if (resetConfirmationUI != null)
        {
            resetConfirmationUI.SetActive(false);
        }
        else
        {
            Debug.LogError("Reset Confirmation UI no está asignado en GameManager.");
        }
    }

    /// <summary>
    /// Inicializa un nuevo juego con valores predeterminados.
    /// </summary>
    private void InitializeNewGame()
    {
        currentGameData = new GameData
        {
            time = 0f,
            coins = 0,
            ballPosition = Vector3.zero,
            ballVelocity = Vector2.zero
            // Inicializa otros campos según sea necesario
        };
        ApplyGameData();
    }

    /// <summary>
    /// Aplica los datos del juego cargados a la escena y objetos relevantes.
    /// </summary>
    private void ApplyGameData()
    {
        // Ejemplo: Posicionar la pelota
        GameObject ball = GameObject.FindWithTag("Player");
        if (ball != null)
        {
            ball.transform.position = currentGameData.ballPosition;
            Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = currentGameData.ballVelocity;
            }
        }
        else
        {
            Debug.LogWarning("No se encontró un objeto con la tag 'Player' en la escena.");
        }

        // Aplicar otros campos de GameData según sea necesario
    }

    /// <summary>
    /// Actualiza el tiempo del juego. Debe ser llamado desde otros scripts que gestionan el tiempo.
    /// </summary>
    /// <param name="deltaTime">Tiempo transcurrido desde el último frame.</param>
    public void UpdateGameTime(float deltaTime)
    {
        if (currentGameData != null)
        {
            currentGameData.time += deltaTime;
        }
    }

    /// <summary>
    /// Añade una moneda al juego.
    /// </summary>
    public void AddCoin()
    {
        if (currentGameData != null)
        {
            currentGameData.coins += 1;
            SaveGame(); // Guardar automáticamente después de añadir una moneda
            Debug.Log($"Se ha añadido una moneda. Total monedas: {currentGameData.coins}");
        }
    }

    /// <summary>
    /// Propiedad que indica si el juego ha terminado.
    /// </summary>
    public bool IsGameOver
    {
        get { return isGameOver; }
    }

    /// <summary>
    /// Finaliza el juego, estableciendo el estado de Game Over y mostrando la UI correspondiente.
    /// </summary>
    public void GameOver()
    {
        if (!isGameOver)
        {
            isGameOver = true;
            Debug.Log("¡Game Over!");

            // Opcional: Mostrar UI de Game Over
            // Por ejemplo:
            // gameOverUIPanel.SetActive(true);
        }
    }

    /// <summary>
    /// Muestra la UI de carga.
    /// </summary>
    public void ShowLoadingUI()
    {
        if (loadingUI != null)
        {
            loadingUI.SetActive(true);
        }
        else
        {
            Debug.LogError("Loading UI no está asignado en GameManager.");
        }
    }

    /// <summary>
    /// Oculta la UI de carga.
    /// </summary>
    public void HideLoadingUI()
    {
        if (loadingUI != null)
        {
            loadingUI.SetActive(false);
        }
        else
        {
            Debug.LogError("Loading UI no está asignado en GameManager.");
        }
    }

    /// <summary>
    /// Maneja la finalización de la carga del juego.
    /// </summary>
    private void OnGameLoaded()
    {
        HideLoadingUI();
        Debug.Log("La carga del juego ha finalizado.");
    }

    /// <summary>
    /// Método para abrir el diálogo.
    /// </summary>
    public void OpenDialogue()
    {
        if (dialogueUI != null)
        {
            dialogueUI.SetActive(true); // Muestra el diálogo
        }
        else
        {
            Debug.LogError("Dialogue UI no está asignado en GameManager.");
        }
    }

    /// <summary>
    /// Método para cerrar el diálogo.
    /// </summary>
    public void CloseDialogue()
    {
        if (dialogueUI != null)
        {
            dialogueUI.SetActive(false); // Oculta el diálogo
        }
        else
        {
            Debug.LogError("Dialogue UI no está asignado en GameManager.");
        }
    }

    /// <summary>
    /// Método para cerrar la aplicación.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Cerrando el juego...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
