using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ScriptableObject que almacena las traducciones para diferentes idiomas.
/// Cada idioma está representado por un diccionario que mapea claves de localización a textos traducidos.
/// </summary>
[CreateAssetMenu(fileName = "NewLocalizationData", menuName = "Dialogue System/Localization Data", order = 3)]
public class LocalizationData : ScriptableObject
{
    [Header("Configuración de Idiomas")]

    /// <summary>
    /// Lista de idiomas soportados en el juego.
    /// </summary>
    [Tooltip("Lista de idiomas soportados en el juego.")]
    [SerializeField] private List<Language> supportedLanguages = new List<Language>();

    /// <summary>
    /// Idioma por defecto que se usará si no se especifica otro.
    /// </summary>
    [Tooltip("Idioma por defecto que se usará si no se especifica otro.")]
    [SerializeField] private Language defaultLanguage;

    /// <summary>
    /// Propiedad pública para acceder a los idiomas soportados.
    /// </summary>
    public List<Language> SupportedLanguages => supportedLanguages;

    /// <summary>
    /// Propiedad pública para acceder al idioma por defecto.
    /// </summary>
    public Language DefaultLanguage => defaultLanguage;

    /// <summary>
    /// Clase que representa un idioma y sus traducciones asociadas.
    /// </summary>
    [System.Serializable]
    public class Language
    {
        [Header("Información del Idioma")]

        /// <summary>
        /// Nombre del idioma (por ejemplo, "Español", "English").
        /// </summary>
        [Tooltip("Nombre del idioma (por ejemplo, 'Español', 'English').")]
        [SerializeField] private string languageName;

        /// <summary>
        /// Código del idioma (por ejemplo, "es", "en").
        /// </summary>
        [Tooltip("Código del idioma (por ejemplo, 'es', 'en').")]
        [SerializeField] private string languageCode;

        /// <summary>
        /// Lista que mapea claves de localización a textos traducidos.
        /// </summary>
        [Tooltip("Lista que mapea claves de localización a textos traducidos.")]
        [SerializeField] private List<LocalizationEntry> localizationEntries = new List<LocalizationEntry>();

        /// <summary>
        /// Propiedad pública para acceder al nombre del idioma.
        /// </summary>
        public string LanguageName => languageName;

        /// <summary>
        /// Propiedad pública para acceder al código del idioma.
        /// </summary>
        public string LanguageCode => languageCode;

        /// <summary>
        /// Propiedad pública para acceder a las entradas de localización.
        /// </summary>
        public List<LocalizationEntry> LocalizationEntries => localizationEntries;

        /// <summary>
        /// Diccionario interno para acceder rápidamente a las traducciones mediante claves.
        /// </summary>
        private Dictionary<string, string> localizationDictionary;

        /// <summary>
        /// Inicializa el diccionario de localización para este idioma.
        /// </summary>
        public void Initialize()
        {
            localizationDictionary = new Dictionary<string, string>();
            foreach (var entry in localizationEntries)
            {
                if (!localizationDictionary.ContainsKey(entry.Key))
                {
                    localizationDictionary.Add(entry.Key, entry.TranslatedText);
                }
                else
                {
                    Debug.LogWarning($"Clave duplicada '{entry.Key}' en el idioma '{languageName}'.");
                }
            }
        }

        /// <summary>
        /// Obtiene el texto traducido para una clave específica.
        /// </summary>
        /// <param name="key">Clave de localización.</param>
        /// <returns>Texto traducido si existe; de lo contrario, devuelve la clave original.</returns>
        public string GetTranslation(string key)
        {
            if (localizationDictionary == null)
            {
                Initialize();
            }

            if (localizationDictionary.TryGetValue(key, out string translatedText))
            {
                return translatedText;
            }
            else
            {
                Debug.LogWarning($"No se encontró la traducción para la clave '{key}' en el idioma '{languageName}'.");
                return key; // Retorna la clave original si no se encuentra la traducción
            }
        }
    }

    /// <summary>
    /// Clase que representa una entrada de localización con una clave y su texto traducido.
    /// </summary>
    [System.Serializable]
    public class LocalizationEntry
    {
        [Header("Entrada de Localización")]

        /// <summary>
        /// Clave única para identificar el texto a traducir.
        /// </summary>
        [Tooltip("Clave única para identificar el texto a traducir.")]
        [SerializeField] private string key;

        /// <summary>
        /// Texto traducido correspondiente a la clave.
        /// </summary>
        [Tooltip("Texto traducido correspondiente a la clave.")]
        [SerializeField] private string translatedText;

        /// <summary>
        /// Propiedad pública para acceder a la clave.
        /// </summary>
        public string Key => key;

        /// <summary>
        /// Propiedad pública para acceder al texto traducido.
        /// </summary>
        public string TranslatedText => translatedText;
    }

    /// <summary>
    /// Método de validación para asegurar que los idiomas y las entradas de localización están correctamente configurados.
    /// </summary>
    private void OnValidate()
    {
        if (supportedLanguages == null || supportedLanguages.Count == 0)
        {
            Debug.LogWarning($"LocalizationData '{name}' no tiene idiomas soportados definidos.");
            return;
        }

        foreach (var language in supportedLanguages)
        {
            if (string.IsNullOrEmpty(language.LanguageName))
            {
                Debug.LogWarning($"Un idioma en '{name}' no tiene asignado un nombre.");
            }

            if (string.IsNullOrEmpty(language.LanguageCode))
            {
                Debug.LogWarning($"El idioma '{language.LanguageName}' en '{name}' no tiene asignado un código.");
            }

            foreach (var entry in language.LocalizationEntries)
            {
                if (string.IsNullOrEmpty(entry.Key))
                {
                    Debug.LogWarning($"Una entrada de localización en el idioma '{language.LanguageName}' no tiene asignada una clave.");
                }

                if (string.IsNullOrEmpty(entry.TranslatedText))
                {
                    Debug.LogWarning($"La entrada de localización con clave '{entry.Key}' en el idioma '{language.LanguageName}' no tiene asignado un texto traducido.");
                }
            }
        }

        if (defaultLanguage == null)
        {
            Debug.LogWarning($"LocalizationData '{name}' no tiene asignado un idioma por defecto.");
        }
        else
        {
            bool defaultExists = false;
            foreach (var lang in supportedLanguages)
            {
                if (lang == defaultLanguage)
                {
                    defaultExists = true;
                    break;
                }
            }

            if (!defaultExists)
            {
                Debug.LogWarning($"El idioma por defecto '{defaultLanguage.LanguageName}' no está en la lista de idiomas soportados en '{name}'.");
            }
        }
    }
}
