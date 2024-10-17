using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class LanguageSelectionUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private LanguageManager languageManager;
    [SerializeField] private GameObject languageButtonPrefab;
    [SerializeField] private Transform buttonsContainer;
    [SerializeField] private GameObject loadingIndicator;

    private void Start()
    {
        PopulateLanguageButtons();
        languageManager.OnLanguageChanged += UpdateButtonStates;
    }

    private void OnDestroy()
    {
        languageManager.OnLanguageChanged -= UpdateButtonStates;
    }

    private void PopulateLanguageButtons()
    {
        if (languageManager == null || languageButtonPrefab == null || buttonsContainer == null)
        {
            Debug.LogError("Faltan referencias en LanguageSelectionUI.");
            return;
        }

        foreach (var language in languageManager.GetSupportedLanguages())
        {
            GameObject buttonObj = Instantiate(languageButtonPrefab, buttonsContainer);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

            if (button != null && buttonText != null)
            {
                buttonText.text = language.LanguageName;
                button.onClick.AddListener(() => ChangeLanguageAsync(language.LanguageCode));
            }
            else
            {
                Debug.LogWarning("Prefab del botón de idioma no tiene los componentes necesarios.");
            }
        }

        UpdateButtonStates(languageManager.CurrentLanguageCode);
    }

    private async void ChangeLanguageAsync(string languageCode)
    {
        if (loadingIndicator != null)
            loadingIndicator.SetActive(true);

        await languageManager.SetLanguageAsync(languageCode);

        if (loadingIndicator != null)
            loadingIndicator.SetActive(false);
    }

    private void UpdateButtonStates(string currentLanguageCode)
    {
        foreach (Transform child in buttonsContainer)
        {
            Button button = child.GetComponent<Button>();
            TextMeshProUGUI buttonText = child.GetComponentInChildren<TextMeshProUGUI>();

            if (button != null && buttonText != null)
            {
                bool isCurrentLanguage = buttonText.text == languageManager.GetSupportedLanguages().Find(lang => lang.LanguageCode == currentLanguageCode)?.LanguageName;
                button.interactable = !isCurrentLanguage;
            }
        }
    }
}