using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class LanguageManager : MonoBehaviour
{
    [Header("Configuración Inicial")]
    [SerializeField] private string initialLanguageCode = "en";
    [SerializeField] private string localizationFolderPath = "Localization";

    private Dictionary<string, Dictionary<string, string>> currentTranslations = new Dictionary<string, Dictionary<string, string>>();
    public string CurrentLanguageCode { get; private set; }

    public delegate void LanguageChangedHandler(string newLanguageCode);
    public event LanguageChangedHandler OnLanguageChanged;

    private void Awake()
    {
        string startLanguage = !string.IsNullOrEmpty(initialLanguageCode) ? initialLanguageCode : "en";
        SetLanguageAsync(startLanguage);
    }

    public async Task SetLanguageAsync(string languageCode)
    {
        CurrentLanguageCode = languageCode;
        await LoadTranslationsAsync(languageCode);
        OnLanguageChanged?.Invoke(CurrentLanguageCode);
    }

    private async Task LoadTranslationsAsync(string languageCode)
    {
        currentTranslations.Clear();
        string folderPath = Path.Combine(Application.dataPath, localizationFolderPath);
        string[] csvFiles = Directory.GetFiles(folderPath, "*.csv");

        foreach (string filePath in csvFiles)
        {
            await LoadCSVFileAsync(filePath, languageCode);
        }
    }

    private async Task LoadCSVFileAsync(string filePath, string languageCode)
    {
        string fileName = Path.GetFileNameWithoutExtension(filePath);
        string[] lines = await File.ReadAllLinesAsync(filePath);
        string[] headers = lines[0].Split(',');
        int languageIndex = System.Array.IndexOf(headers, languageCode);

        if (languageIndex == -1)
        {
            Debug.LogWarning($"Language '{languageCode}' not found in file {fileName}");
            return;
        }

        currentTranslations[fileName] = new Dictionary<string, string>();

        for (int i = 1; i < lines.Length; i++)
        {
            string[] columns = lines[i].Split(',');
            if (columns.Length > languageIndex)
            {
                string key = columns[0];
                string value = columns[languageIndex];
                currentTranslations[fileName][key] = value;
            }
        }
    }

    public string GetLocalizedText(string fileName, string key)
    {
        if (currentTranslations.TryGetValue(fileName, out var fileTranslations))
        {
            if (fileTranslations.TryGetValue(key, out string translation))
            {
                return translation;
            }
        }
        Debug.LogWarning($"Translation not found for key '{key}' in file '{fileName}' for language '{CurrentLanguageCode}'");
        return key;
    }

    public List<string> GetSupportedLanguages()
    {
        List<string> languages = new List<string>();
        string filePath = Path.Combine(Application.dataPath, localizationFolderPath, "ui_texts.csv");
        if (File.Exists(filePath))
        {
            string[] headers = File.ReadAllLines(filePath)[0].Split(',');
            languages.AddRange(headers);
            languages.RemoveAt(0); // Remove the "Key" column
        }
        return languages;
    }
}