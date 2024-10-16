using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Proporciona una interfaz para que el jugador pueda seleccionar y cambiar el idioma del juego en tiempo real.
/// </summary>
public class LanguageSelectionUI : MonoBehaviour
{
    [Header("Referencias")]

    /// <summary>
    /// Referencia al LanguageManager.
    /// </summary>
    [Tooltip("Referencia al LanguageManager.")]
    [SerializeField] private LanguageManager languageManager;

    /// <summary>
    /// Prefab del botón de idioma.
    /// </summary>
    [Tooltip("Prefab del botón de idioma.")]
    [SerializeField] private GameObject languageButtonPrefab;

    /// <summary>
    /// Contenedor para los botones de idioma.
    /// </summary>
    [Tooltip("Contenedor para los botones de idioma.")]
    [SerializeField] private Transform buttonsContainer;

    private void Start()
    {
        PopulateLanguageButtons();
    }

    /// <summary>
    /// Crea y configura los botones para cada idioma soportado.
    /// </summary>
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
                button.onClick.AddListener(() => ChangeLanguage(language.LanguageCode));
            }
            else
            {
                Debug.LogWarning("Prefab del botón de idioma no tiene los componentes necesarios.");
            }
        }
    }

    /// <summary>
    /// Cambia el idioma del juego al seleccionado.
    /// </summary>
    /// <param name="languageCode">Código del idioma a establecer.</param>
    private void ChangeLanguage(string languageCode)
    {
        languageManager.SetLanguage(languageCode);
    }
}
