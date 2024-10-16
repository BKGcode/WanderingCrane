using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Detecta la interacción del jugador y activa un diálogo específico.
/// </summary>
public class DialogueTrigger : MonoBehaviour
{
    [Header("Referencias")]

    /// <summary>
    /// Datos de diálogo que se van a activar.
    /// </summary>
    [Tooltip("Datos de diálogo que se van a activar.")]
    [SerializeField] private DialogueData dialogueData;

    /// <summary>
    /// Distancia mínima para que el jugador pueda interactuar.
    /// </summary>
    [Tooltip("Distancia mínima para que el jugador pueda interactuar.")]
    [SerializeField] private float interactionDistance = 3f;

    /// <summary>
    /// Tecla para interactuar (por ejemplo, 'E').
    /// </summary>
    [Tooltip("Tecla para interactuar (por ejemplo, 'E').")]
    [SerializeField] private KeyCode interactionKey = KeyCode.E;

    /// <summary>
    /// Mensaje de indicación para el jugador.
    /// </summary>
    [Tooltip("Mensaje de indicación para el jugador.")]
    [SerializeField] private string interactionPrompt = "Presiona 'E' para hablar";

    [Header("Componentes UI")]

    /// <summary>
    /// Referencia al texto de indicación de interacción.
    /// </summary>
    [Tooltip("Referencia al texto de indicación de interacción.")]
    [SerializeField] private Text promptText;

    private Transform playerTransform;
    private bool isPlayerInRange = false;

    private void Start()
    {
        // Asumimos que el jugador tiene la etiqueta "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("No se encontró un objeto con la etiqueta 'Player' en la escena.");
        }

        // Ocultar el mensaje de interacción al inicio
        if (promptText != null)
        {
            promptText.text = "";
        }
    }

    private void Update()
    {
        if (playerTransform == null || dialogueData == null)
            return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        if (distance <= interactionDistance)
        {
            if (!isPlayerInRange)
            {
                isPlayerInRange = true;
                ShowPrompt();
            }

            if (Input.GetKeyDown(interactionKey))
            {
                TriggerDialogue();
            }
        }
        else
        {
            if (isPlayerInRange)
            {
                isPlayerInRange = false;
                HidePrompt();
            }
        }
    }

    /// <summary>
    /// Muestra el mensaje de indicación para interactuar.
    /// </summary>
    private void ShowPrompt()
    {
        if (promptText != null)
        {
            promptText.text = interactionPrompt;
        }
    }

    /// <summary>
    /// Oculta el mensaje de indicación de interacción.
    /// </summary>
    private void HidePrompt()
    {
        if (promptText != null)
        {
            promptText.text = "";
        }
    }

    /// <summary>
    /// Activa el diálogo asociado.
    /// </summary>
    private void TriggerDialogue()
    {
        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
        if (dialogueManager != null)
        {
            dialogueManager.SetCurrentDialogue(dialogueData);
            dialogueManager.StartDialogue();
        }
        else
        {
            Debug.LogError("No se encontró una instancia de DialogueManager en la escena.");
        }
    }
}
