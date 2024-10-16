using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ScriptableObject que almacena las entradas de diálogo para un diálogo específico.
/// </summary>
[CreateAssetMenu(fileName = "NewDialogueData", menuName = "Dialogue System/Dialogue Data", order = 2)]
public class DialogueData : ScriptableObject
{
    [Header("Entradas de Diálogo")]

    /// <summary>
    /// Lista de entradas de diálogo en orden secuencial.
    /// </summary>
    [Tooltip("Lista de entradas de diálogo en orden secuencial.")]
    [SerializeField] private List<DialogueEntry> dialogueEntries = new List<DialogueEntry>();

    /// <summary>
    /// Propiedad pública para acceder a las entradas de diálogo.
    /// </summary>
    public List<DialogueEntry> DialogueEntries => dialogueEntries;

    /// <summary>
    /// Clase que representa una entrada de diálogo con un hablante, clave de localización y sonido opcional.
    /// </summary>
    [System.Serializable]
    public class DialogueEntry
    {
        [Header("Información de la Entrada de Diálogo")]

        /// <summary>
        /// Referencia al personaje que está hablando.
        /// </summary>
        [Tooltip("Referencia al personaje que está hablando.")]
        [SerializeField] private CharacterData speaker;

        /// <summary>
        /// Clave de localización para el texto del diálogo.
        /// </summary>
        [Tooltip("Clave de localización para el texto del diálogo.")]
        [SerializeField] private string localizedTextKey;

        /// <summary>
        /// Sonido opcional que se reproducirá al mostrar esta entrada de diálogo.
        /// </summary>
        [Tooltip("Sonido opcional que se reproducirá al mostrar esta entrada de diálogo.")]
        [SerializeField] private AudioClip dialogueSound;

        /// <summary>
        /// Propiedad pública para acceder al hablante.
        /// </summary>
        public CharacterData Speaker => speaker;

        /// <summary>
        /// Propiedad pública para acceder a la clave de localización.
        /// </summary>
        public string LocalizedTextKey => localizedTextKey;

        /// <summary>
        /// Propiedad pública para acceder al sonido del diálogo.
        /// </summary>
        public AudioClip DialogueSound => dialogueSound;
    }
}
