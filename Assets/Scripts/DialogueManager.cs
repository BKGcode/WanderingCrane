// DialogueManager.cs
using UnityEngine;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private DialogueUI dialogueUI;

    [Header("Cooldown Settings")]
    [SerializeField] private float cooldownDuration = 2f;

    [Header("Language Settings")]
    [SerializeField] private string initialLanguageCode = "en";

    private PlayerData playerData;
    private DialogueData currentDialogue;
    private int currentLineIndex = 0;
    private bool isDialogueActive = false;
    private bool isCooldown = false;
    private string currentLanguageCode;

    private void Awake()
    {
        // Implementación del patrón Singleton
        if (Instance == null)
        {
            Instance = this;
            // Opcional: DontDestroyOnLoad(gameObject);
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
                Debug.LogError("No se encontró DialogueUI en la escena.");
            }
        }

        currentLanguageCode = initialLanguageCode;
    }

    public bool Initialize(PlayerData player, DialogueData dialogue)
    {
        if (isDialogueActive || isCooldown)
        {
            return false;
        }

        playerData = player;
        currentDialogue = dialogue;
        currentLineIndex = 0;
        isDialogueActive = true;

        dialogueUI.gameObject.SetActive(true);
        DisplayCurrentLine();
        PauseGame();

        return true;
    }

    private void DisplayCurrentLine()
    {
        if (currentLineIndex >= currentDialogue.dialogueLines.Count)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = currentDialogue.dialogueLines[currentLineIndex];
        Sprite avatar = line.speaker == Speaker.Player ? playerData.avatar : currentDialogue.npcAvatar;
        string speakerName = line.speaker == Speaker.Player ? playerData.playerName : currentDialogue.npcName;
        string localizedText = GetLocalizedText(line);

        dialogueUI.UpdateDialogue(avatar, speakerName, localizedText);
    }

    private string GetLocalizedText(DialogueLine line)
    {
        LocalizedText localizedText = line.localizedTexts.Find(t => t.languageCode == currentLanguageCode);
        if (localizedText != null)
        {
            return localizedText.text;
        }
        else
        {
            Debug.LogWarning($"No se encontró texto para el idioma '{currentLanguageCode}' en la línea de diálogo.");
            return "[Texto no disponible]";
        }
    }

    public void AdvanceDialogue()
    {
        if (!isDialogueActive)
            return;

        currentLineIndex++;
        DisplayCurrentLine();
    }

    public void EndDialogue()
    {
        isDialogueActive = false;
        dialogueUI.HideDialogue();
        dialogueUI.gameObject.SetActive(false);
        ResumeGame();
        StartCoroutine(StartCooldown());
    }

    private IEnumerator StartCooldown()
    {
        isCooldown = true;
        yield return new WaitForSecondsRealtime(cooldownDuration);
        isCooldown = false;
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    public void SetLanguage(string languageCode)
    {
        currentLanguageCode = languageCode;
    }

    public string GetCurrentLanguage()
    {
        return currentLanguageCode;
    }
}