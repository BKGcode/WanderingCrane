using UnityEngine;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private DialogueUI dialogueUI;

    [Header("Cooldown Settings")]
    [SerializeField] private float cooldownDuration = 2f;

    [Header("Localization")]
    [SerializeField] private LanguageManager languageManager;

    private PlayerData playerData;
    private NPCDialogueData currentNPCDialogue;
    private int currentLineIndex = 0;
    private bool isDialogueActive = false;
    private bool isCooldown = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (dialogueUI == null)
        {
            dialogueUI = FindObjectOfType<DialogueUI>();
            if (dialogueUI == null)
            {
                Debug.LogError("No se encontró DialogueUI en la escena. Asegúrate de que existe y tiene el componente DialogueUI.");
            }
        }

        if (languageManager == null)
        {
            languageManager = FindObjectOfType<LanguageManager>();
            if (languageManager == null)
            {
                Debug.LogError("No se encontró LanguageManager en la escena. Asegúrate de que existe y tiene el componente LanguageManager.");
            }
        }
    }

    public bool Initialize(PlayerData player, NPCDialogueData npcDialogue)
    {
        if (isDialogueActive)
        {
            Debug.LogWarning("Ya hay un diálogo activo.");
            return false;
        }

        if (isCooldown)
        {
            Debug.LogWarning("En cooldown. No se puede iniciar un nuevo diálogo.");
            return false;
        }

        playerData = player;
        currentNPCDialogue = npcDialogue;
        currentLineIndex = 0;
        isDialogueActive = true;
        Debug.Log($"Iniciando diálogo con {currentNPCDialogue.npcName}");

        if (dialogueUI != null)
        {
            dialogueUI.gameObject.SetActive(true);
            dialogueUI.ShowDialogue();
            DisplayCurrentLine();
            PauseGame();
            return true;
        }
        else
        {
            Debug.LogError("DialogueUI es null. No se puede mostrar el diálogo.");
            return false;
        }
    }

    private void DisplayCurrentLine()
    {
        if (currentLineIndex >= currentNPCDialogue.dialogueLines.Count)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = currentNPCDialogue.dialogueLines[currentLineIndex];
        Sprite avatar = line.speaker == Speaker.Player ? playerData.avatar : currentNPCDialogue.avatar;
        string speakerName = line.speaker == Speaker.Player ? playerData.playerName : currentNPCDialogue.npcName;
        string localizedText = languageManager.GetLocalizedText(currentNPCDialogue.csvFileName, line.textKey);
        Debug.Log($"Mostrando línea {currentLineIndex + 1}: {speakerName} dice: {localizedText}");
        dialogueUI.UpdateDialogue(avatar, speakerName, localizedText);
    }

    public void AdvanceDialogue()
    {
        if (!isDialogueActive)
        {
            Debug.LogWarning("No hay un diálogo activo para avanzar.");
            return;
        }

        currentLineIndex++;
        Debug.Log($"Avanzando al diálogo {currentLineIndex + 1}");
        DisplayCurrentLine();
    }

    public void EndDialogue()
    {
        isDialogueActive = false;
        Debug.Log("Diálogo finalizado.");
        if (dialogueUI != null)
        {
            dialogueUI.HideDialogue();
            dialogueUI.gameObject.SetActive(false);
        }
        ResumeGame();
        StartCoroutine(StartCooldown());
    }

    private IEnumerator StartCooldown()
    {
        isCooldown = true;
        Debug.Log($"Cooldown iniciado por {cooldownDuration} segundos.");
        yield return new WaitForSecondsRealtime(cooldownDuration);
        isCooldown = false;
        Debug.Log("Cooldown finalizado.");
    }

    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        Debug.Log("Juego pausado.");
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
        Debug.Log("Juego reanudado.");
    }
}