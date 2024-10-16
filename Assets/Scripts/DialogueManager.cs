using UnityEngine;

/// <summary>
/// Controla el flujo de los diálogos, maneja la secuencia de entradas y comunica con la UI para actualizar el contenido mostrado.
/// </summary>
public class DialogueManager : MonoBehaviour
{
    [Header("Referencias")]

    /// <summary>
    /// Referencia a la UI del diálogo.
    /// </summary>
    [Tooltip("Referencia a la UI del diálogo.")]
    [SerializeField] private DialogueUI dialogueUI;

    /// <summary>
    /// Referencia al gestor de idiomas.
    /// </summary>
    [Tooltip("Referencia al gestor de idiomas.")]
    [SerializeField] private LanguageManager languageManager;

    /// <summary>
    /// Datos de diálogo que se van a mostrar.
    /// </summary>
    private DialogueData currentDialogue;

    /// <summary>
    /// Índice de la entrada de diálogo actual.
    /// </summary>
    private int currentEntryIndex = 0;

    /// <summary>
    /// Indica si un diálogo está activo.
    /// </summary>
    private bool isDialogueActive = false;

    private void OnEnable()
    {
        // Suscribirse al evento de cambio de idioma para actualizar el diálogo si está activo
        EventManager.StartListening("OnLanguageChanged", UpdateDialogueUI);
    }

    private void OnDisable()
    {
        // Desuscribirse del evento
        EventManager.StopListening("OnLanguageChanged", UpdateDialogueUI);
    }

    /// <summary>
    /// Establece el diálogo actual a mostrar.
    /// </summary>
    /// <param name="dialogue">Datos de diálogo que se van a mostrar.</param>
    public void SetCurrentDialogue(DialogueData dialogue)
    {
        currentDialogue = dialogue;
    }

    /// <summary>
    /// Inicia el diálogo.
    /// </summary>
    public void StartDialogue()
    {
        if (currentDialogue == null || currentDialogue.DialogueEntries.Count == 0)
        {
            Debug.LogWarning("No hay diálogo para iniciar.");
            return;
        }

        isDialogueActive = true;
        currentEntryIndex = 0;
        ShowCurrentEntry();
    }

    /// <summary>
    /// Avanza al siguiente diálogo.
    /// </summary>
    public void AdvanceDialogue()
    {
        if (!isDialogueActive)
            return;

        currentEntryIndex++;
        if (currentEntryIndex < currentDialogue.DialogueEntries.Count)
        {
            ShowCurrentEntry();
        }
        else
        {
            EndDialogue();
        }
    }

    /// <summary>
    /// Muestra la entrada de diálogo actual en la UI.
    /// </summary>
    private void ShowCurrentEntry()
    {
        if (currentEntryIndex >= currentDialogue.DialogueEntries.Count)
        {
            EndDialogue();
            return;
        }

        var entry = currentDialogue.DialogueEntries[currentEntryIndex];
        string localizedText = languageManager.GetLocalizedText(entry.LocalizedTextKey);

        dialogueUI.DisplayDialogue(entry.Speaker.CharacterName, entry.Speaker.Portrait, localizedText, entry.DialogueSound);
    }

    /// <summary>
    /// Finaliza el diálogo y oculta la UI.
    /// </summary>
    private void EndDialogue()
    {
        isDialogueActive = false;
        dialogueUI.HideDialogue();
    }

    /// <summary>
    /// Actualiza la UI del diálogo cuando cambia el idioma.
    /// </summary>
    private void UpdateDialogueUI()
    {
        if (!isDialogueActive)
            return;

        ShowCurrentEntry();
    }

    private void Start()
    {
        if (dialogueUI != null)
        {
            dialogueUI.OnNextDialogue += AdvanceDialogue;
        }
        else
        {
            Debug.LogError("DialogueUI no está asignado en DialogueManager.");
        }
    }

    private void OnDestroy()
    {
        if (dialogueUI != null)
        {
            dialogueUI.OnNextDialogue -= AdvanceDialogue;
        }
    }
}
