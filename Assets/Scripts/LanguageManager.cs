using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Gestiona el idioma actual del juego, carga las traducciones y permite cambios dinámicos de idioma.
/// </summary>
public class LanguageManager : MonoBehaviour
{
    [Header("Referencias")]

    /// <summary>
    /// Datos de localización que contienen las traducciones.
    /// </summary>
    [Tooltip("Datos de localización que contienen las traducciones.")]
    [SerializeField] private LocalizationData localizationData;

    [Header("Configuración Inicial")]

    /// <summary>
    /// Código del idioma por defecto al iniciar el juego.
    /// </summary>
    [Tooltip("Código del idioma por defecto al iniciar el juego.")]
    [SerializeField] private string initialLanguageCode = "en";

    /// <summary>
    /// Diccionario para almacenar las traducciones actuales.
    /// </summary>
    private Dictionary<string, string> currentTranslations = new Dictionary<string, string>();

    /// <summary>
    /// Código del idioma actualmente activo.
    /// </summary>
    public string CurrentLanguageCode { get; private set; }

    private void Awake()
    {
        if (localizationData == null)
        {
            Debug.LogError("LocalizationData no está asignado en LanguageManager.");
            return;
        }

        // Inicializar con el idioma por defecto o el especificado
        string startLanguage = !string.IsNullOrEmpty(initialLanguageCode) ? initialLanguageCode : localizationData.DefaultLanguage.LanguageCode;
        SetLanguage(startLanguage);
    }

    /// <summary>
    /// Cambia el idioma actual del juego.
    /// </summary>
    /// <param name="languageCode">Código del idioma a establecer.</param>
    public void SetLanguage(string languageCode)
    {
        var language = localizationData.SupportedLanguages.Find(lang => lang.LanguageCode == languageCode);
        if (language != null)
        {
            CurrentLanguageCode = languageCode;
            language.Initialize();

            currentTranslations.Clear();
            foreach (var entry in language.LocalizationEntries) // 'LocalizationEntries' ahora es público
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

            // Notificar a otros sistemas que el idioma ha cambiado
            EventManager.TriggerEvent("OnLanguageChanged");
        }
        else
        {
            Debug.LogWarning($"El idioma con código '{languageCode}' no está soportado.");
        }
    }

    /// <summary>
    /// Obtiene el texto traducido para una clave específica.
    /// </summary>
    /// <param name="key">Clave de localización.</param>
    /// <returns>Texto traducido si existe; de lo contrario, retorna la clave original.</returns>
    public string GetLocalizedText(string key)
    {
        if (currentTranslations.TryGetValue(key, out string translatedText))
        {
            return translatedText;
        }
        else
        {
            Debug.LogWarning($"No se encontró la traducción para la clave '{key}' en el idioma '{CurrentLanguageCode}'.");
            return key; // Retorna la clave original si no se encuentra la traducción
        }
    }

    /// <summary>
    /// Obtiene la lista de idiomas soportados.
    /// </summary>
    /// <returns>Lista de idiomas soportados.</returns>
    public List<LocalizationData.Language> GetSupportedLanguages()
    {
        return localizationData.SupportedLanguages;
    }
}
