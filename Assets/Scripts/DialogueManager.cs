// DialogueManager.cs
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private DialogueUI dialogueUI;

    [Header("Cooldown Settings")]
    [SerializeField] private float cooldownDuration = 2f;

    [Header("Localization")]
    [SerializeField] private LocalizationManager localizationManager;

    private PlayerData playerData;
    private DialogueData currentDialogue;
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
                Debug.LogError("No se encontró DialogueUI en la escena.");
            }
        }

        if (localizationManager == null)
        {
            localizationManager = FindObjectOfType<LocalizationManager>();
            if (localizationManager == null)
            {
                Debug.LogError("No se encontró LocalizationManager en la escena.");
            }
        }
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
        string localizedText = localizationManager.GetLocalizedText(line.textKey);

        dialogueUI.UpdateDialogue(avatar, speakerName, localizedText);
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

    private System.Collections.IEnumerator StartCooldown()
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
}
