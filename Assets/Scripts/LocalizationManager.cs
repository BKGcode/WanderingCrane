// LocalizationManager.cs
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class LocalizationManager : MonoBehaviour
{
    [Header("Configuración Inicial")]
    [SerializeField] private string initialLanguageCode = "en";
    [SerializeField] private TextAsset localizationCSV;

    private Dictionary<string, string> currentTranslations = new Dictionary<string, string>();
    private Dictionary<string, Dictionary<string, string>> allTranslations = new Dictionary<string, Dictionary<string, string>>();

    public string CurrentLanguageCode { get; private set; }

    private void Awake()
    {
        LoadAllTranslations();
        string startLanguage = !string.IsNullOrEmpty(initialLanguageCode) ? initialLanguageCode : "en";
        SetLanguage(startLanguage);
    }

    private void LoadAllTranslations()
    {
        if (localizationCSV == null)
        {
            Debug.LogError("No se ha asignado el archivo CSV de localización.");
            return;
        }

        string[] lines = localizationCSV.text.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length < 2)
        {
            Debug.LogError("El archivo CSV de localización está vacío o mal formateado.");
            return;
        }

        string[] headers = lines[0].Split(',');
        for (int i = 1; i < headers.Length; i++)
        {
            string languageCode = headers[i].Trim();
            if (!allTranslations.ContainsKey(languageCode))
            {
                allTranslations[languageCode] = new Dictionary<string, string>();
            }
        }

        for (int i = 1; i < lines.Length; i++)
        {
            string[] fields = lines[i].Split(',');
            if (fields.Length < 2) continue;

            string key = fields[0].Trim();
            for (int j = 1; j < fields.Length; j++)
            {
                string languageCode = headers[j].Trim();
                string value = fields[j].Trim().Replace("\\n", "\n");
                allTranslations[languageCode][key] = value;
            }
        }
    }

    public void SetLanguage(string languageCode)
    {
        if (allTranslations.ContainsKey(languageCode))
        {
            CurrentLanguageCode = languageCode;
            currentTranslations = allTranslations[languageCode];
            // Notificar a los interesados que el idioma ha cambiado
        }
        else
        {
            Debug.LogWarning($"Idioma '{languageCode}' no encontrado. Se mantiene el idioma actual.");
        }
    }

    public string GetLocalizedText(string key)
    {
        if (currentTranslations.TryGetValue(key, out string translation))
        {
            return translation;
        }
        else
        {
            Debug.LogWarning($"Clave de localización '{key}' no encontrada en el idioma '{CurrentLanguageCode}'.");
            return key;
        }
    }

    public List<string> GetSupportedLanguages()
    {
        return new List<string>(allTranslations.Keys);
    }
}
