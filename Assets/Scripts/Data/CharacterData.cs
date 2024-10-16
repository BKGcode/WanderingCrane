using UnityEngine;

/// <summary>
/// ScriptableObject que almacena la información de un personaje en el sistema de diálogos.
/// </summary>
[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Dialogue System/Character Data", order = 1)]
public class CharacterData : ScriptableObject
{
    [Header("Información del Personaje")]

    /// <summary>
    /// Nombre del personaje.
    /// </summary>
    [Tooltip("Nombre del personaje.")]
    [SerializeField] private string characterName;

    /// <summary>
    /// Retrato del personaje.
    /// </summary>
    [Tooltip("Retrato del personaje.")]
    [SerializeField] private Sprite portrait;

    /// <summary>
    /// Sonido opcional que se reproducirá cuando el personaje salude o inicie el diálogo.
    /// </summary>
    [Tooltip("Sonido opcional que se reproducirá cuando el personaje salude o inicie el diálogo.")]
    [SerializeField] private AudioClip greetingSound;

    /// <summary>
    /// Propiedad pública para acceder al nombre del personaje.
    /// </summary>
    public string CharacterName => characterName;

    /// <summary>
    /// Propiedad pública para acceder al retrato del personaje.
    /// </summary>
    public Sprite Portrait => portrait;

    /// <summary>
    /// Propiedad pública para acceder al sonido de saludo del personaje.
    /// </summary>
    public AudioClip GreetingSound => greetingSound;
}
