// DialogueUI.cs
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
        nextButton.onClick.AddListener(OnNextButtonClicked);
        closeButton.onClick.AddListener(OnCloseButtonClicked);
        gameObject.SetActive(false);
    }

    private void OnNextButtonClicked()
    {
        DialogueManager.Instance.AdvanceDialogue();
    }

    private void OnCloseButtonClicked()
    {
        DialogueManager.Instance.EndDialogue();
    }

    public void UpdateDialogue(Sprite avatar, string speakerName, string text)
    {
        speakerAvatarImage.sprite = avatar;
        speakerNameText.text = speakerName;
        dialogueText.text = text;
    }

    public void HideDialogue()
    {
        gameObject.SetActive(false);
    }
}
