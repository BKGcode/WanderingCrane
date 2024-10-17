using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image speakerAvatarImage;
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(OnNextButtonClicked);
        }
        else
        {
            Debug.LogWarning("NextButton no está asignado en DialogueUI.");
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
        }
        else
        {
            Debug.LogWarning("CloseButton no está asignado en DialogueUI.");
        }

        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnCloseButtonClicked();
        }
    }

    private void OnNextButtonClicked()
    {
        Debug.Log("Botón 'Next' clicado.");
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.AdvanceDialogue();
        }
        else
        {
            Debug.LogError("DialogueManager.Instance es null. No se puede avanzar el diálogo.");
        }
    }

    private void OnCloseButtonClicked()
    {
        Debug.Log("Cerrando diálogo.");
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.EndDialogue();
        }
        else
        {
            Debug.LogError("DialogueManager.Instance es null. No se puede cerrar el diálogo.");
        }
    }

    public void ShowDialogue()
    {
        gameObject.SetActive(true);
        Debug.Log("Diálogo UI mostrado.");
    }

    public void HideDialogue()
    {
        gameObject.SetActive(false);
        Debug.Log("Diálogo UI ocultado.");
    }

    public void UpdateDialogue(Sprite avatar, string speakerName, string text)
    {
        if (speakerAvatarImage != null)
            speakerAvatarImage.sprite = avatar;
        else
            Debug.LogWarning("speakerAvatarImage no está asignado en DialogueUI.");

        if (speakerNameText != null)
            speakerNameText.text = speakerName;
        else
            Debug.LogWarning("speakerNameText no está asignado en DialogueUI.");

        if (dialogueText != null)
            dialogueText.text = text;
        else
            Debug.LogWarning("dialogueText no está asignado en DialogueUI.");

        Debug.Log($"Diálogo UI actualizado: {speakerName} dice: {text}");
    }
}