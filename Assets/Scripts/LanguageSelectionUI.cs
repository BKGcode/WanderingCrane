using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using System.Collections.Generic;

public class LanguageSelectionUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private LanguageManager languageManager;
    [SerializeField] private GameObject languageButtonPrefab;
    [SerializeField] private Transform buttonsContainer;
    [SerializeField] private GameObject loadingIndicator;

    private Dictionary<string, string> languageCodeToName = new Dictionary<string, string>
    {
        {"en", "English"},
        {"es", "Español"},
        {"fr", "Français"},
        {"de", "Deutsch"},
        {"it", "Italiano"}
        // Añade más idiomas según sea necesario
    };

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

        List<string> supportedLanguages = languageManager.GetSupportedLanguages();

        foreach (var languageCode in supportedLanguages)
        {
            GameObject buttonObj = Instantiate(languageButtonPrefab, buttonsContainer);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

            if (button != null && buttonText != null)
            {
                string languageName = languageCodeToName.TryGetValue(languageCode, out string name) ? name : languageCode;
                buttonText.text = languageName;
                button.onClick.AddListener(() => ChangeLanguageAsync(languageCode));
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
                string buttonLanguageCode = GetLanguageCodeFromName(buttonText.text);
                bool isCurrentLanguage = buttonLanguageCode == currentLanguageCode;
                button.interactable = !isCurrentLanguage;
            }
        }
    }

    private string GetLanguageCodeFromName(string languageName)
    {
        foreach (var pair in languageCodeToName)
        {
            if (pair.Value == languageName)
            {
                return pair.Key;
            }
        }
        return languageName; // Fallback to using the name as the code
    }
}