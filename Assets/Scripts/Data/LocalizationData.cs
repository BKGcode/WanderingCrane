using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

[CreateAssetMenu(fileName = "LocalizationData", menuName = "Localization/Localization Data")]
public class LocalizationData : ScriptableObject
{
    [System.Serializable]
    public class LocalizationEntry
    {
        public string Key;
        public string TranslatedText;
    }

    [System.Serializable]
    public class Language
    {
        public string LanguageCode;
        public string LanguageName;
        public List<LocalizationEntry> LocalizationEntries = new List<LocalizationEntry>();

        public async Task InitializeAsync()
        {
            // Simula una carga asíncrona
            await Task.Delay(100);
        }
    }

    public List<Language> SupportedLanguages = new List<Language>();
    public Language DefaultLanguage;

    public void AddLocalizationEntry(string languageCode, string key, string translatedText)
    {
        var language = SupportedLanguages.Find(l => l.LanguageCode == languageCode);
        if (language != null)
        {
            var entry = language.LocalizationEntries.Find(e => e.Key == key);
            if (entry != null)
            {
                entry.TranslatedText = translatedText;
            }
            else
            {
                language.LocalizationEntries.Add(new LocalizationEntry { Key = key, TranslatedText = translatedText });
            }
        }
        else
        {
            Debug.LogWarning($"El idioma con código '{languageCode}' no está soportado.");
        }
    }

    public string GetLocalizedText(string languageCode, string key)
    {
        var language = SupportedLanguages.Find(l => l.LanguageCode == languageCode);
        if (language != null)
        {
            var entry = language.LocalizationEntries.Find(e => e.Key == key);
            if (entry != null)
            {
                return entry.TranslatedText;
            }
        }
        return key;
    }
}