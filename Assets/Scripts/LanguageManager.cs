using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

public class LanguageManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private LocalizationData localizationData;

    [Header("Configuración Inicial")]
    [SerializeField] private string initialLanguageCode = "en";

    private Dictionary<string, string> currentTranslations = new Dictionary<string, string>();
    public string CurrentLanguageCode { get; private set; }

    public delegate void LanguageChangedHandler(string newLanguageCode);
    public event LanguageChangedHandler OnLanguageChanged;

    private void Awake()
    {
        if (localizationData == null)
        {
            Debug.LogError("LocalizationData no está asignado en LanguageManager.");
            return;
        }

        string startLanguage = !string.IsNullOrEmpty(initialLanguageCode) ? initialLanguageCode : localizationData.DefaultLanguage.LanguageCode;
        SetLanguageAsync(startLanguage);
    }

    public async Task SetLanguageAsync(string languageCode)
    {
        var language = localizationData.SupportedLanguages.Find(lang => lang.LanguageCode == languageCode);
        if (language != null)
        {
            CurrentLanguageCode = languageCode;
            await language.InitializeAsync();

            currentTranslations.Clear();
            foreach (var entry in language.LocalizationEntries)
            {
                if (!currentTranslations.ContainsKey(entry.Key))
                {
                    currentTranslations.Add(entry.Key, entry.TranslatedText);
                }
                else
                {
                    Debug.LogWarning($"Clave duplicada '{entry.Key}' en el idioma '{language.LanguageName}'.");
                }
            }

            OnLanguageChanged?.Invoke(CurrentLanguageCode);
        }
        else
        {
            Debug.LogWarning($"El idioma con código '{languageCode}' no está soportado.");
        }
    }

    public string GetLocalizedText(string key)
    {
        if (currentTranslations.TryGetValue(key, out string translatedText))
        {
            return translatedText;
        }
        else
        {
            Debug.LogWarning($"No se encontró la traducción para la clave '{key}' en el idioma '{CurrentLanguageCode}'.");
            return key;
        }
    }

    public List<LocalizationData.Language> GetSupportedLanguages()
    {
        return localizationData.SupportedLanguages;
    }
}