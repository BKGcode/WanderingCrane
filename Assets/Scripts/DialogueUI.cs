using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Gestiona los elementos de la UI del diálogo, incluyendo TextMeshPro para el texto y el nombre, y el retrato del hablante.
/// </summary>
public class DialogueUI : MonoBehaviour
{
    [Header("Elementos de la UI")]

    /// <summary>
    /// Texto para mostrar el nombre del hablante.
    /// </summary>
    [Tooltip("Texto para mostrar el nombre del hablante.")]
    [SerializeField] private TextMeshProUGUI speakerNameText;

    /// <summary>
    /// Texto para mostrar el contenido del diálogo.
    /// </summary>
    [Tooltip("Texto para mostrar el contenido del diálogo.")]
    [SerializeField] private TextMeshProUGUI dialogueText;

    /// <summary>
    /// Imagen para mostrar el retrato del hablante.
    /// </summary>
    [Tooltip("Imagen para mostrar el retrato del hablante.")]
    [SerializeField] private Image speakerPortraitImage;

    /// <summary>
    /// Botón para avanzar al siguiente diálogo.
    /// </summary>
    [Tooltip("Botón para avanzar al siguiente diálogo.")]
    [SerializeField] private Button nextButton;

    /// <summary>
    /// Evento que se dispara cuando se presiona el botón para avanzar el diálogo.
    /// </summary>
    public delegate void NextDialogueAction();
    public event NextDialogueAction OnNextDialogue;

    private void Awake()
    {
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(() =>
            {
                OnNextDialogue?.Invoke();
            });
        }
        else
        {
            Debug.LogWarning("NextButton no está asignado en DialogueUI.");
        }

        gameObject.SetActive(false); // Ocultar la UI al inicio
    }

    /// <summary>
    /// Muestra el diálogo con la información proporcionada.
    /// </summary>
    /// <param name="speakerName">Nombre del hablante.</param>
    /// <param name="portrait">Retrato del hablante.</param>
    /// <param name="text">Texto del diálogo.</param>
    /// <param name="dialogueSound">Sonido opcional para reproducir al mostrar el diálogo.</param>
    public void DisplayDialogue(string speakerName, Sprite portrait, string text, AudioClip dialogueSound = null)
    {
        if (speakerNameText != null)
            speakerNameText.text = speakerName;

        if (speakerPortraitImage != null)
            speakerPortraitImage.sprite = portrait;

        if (dialogueText != null)
            dialogueText.text = text;

        gameObject.SetActive(true);

        if (dialogueSound != null)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.PlayOneShot(dialogueSound);
            }
            else
            {
                Debug.LogWarning("No hay AudioSource en el objeto de DialogueUI para reproducir el sonido del diálogo.");
            }
        }
    }

    /// <summary>
    /// Oculta la UI del diálogo.
    /// </summary>
    public void HideDialogue()
    {
        gameObject.SetActive(false);
    }
}
