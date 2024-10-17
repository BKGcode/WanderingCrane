// DialogueTrigger.cs
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue Settings")]
    [SerializeField] private DialogueData dialogueData;
    [SerializeField] private PlayerData playerData;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            Debug.Log($"El jugador ha entrado en el trigger de {dialogueData.npcName}");
            if (DialogueManager.Instance != null)
            {
                if (dialogueData != null && playerData != null)
                {
                    if (DialogueManager.Instance.Initialize(playerData, dialogueData))
                    {
                        hasTriggered = true;
                    }
                }
                else
                {
                    Debug.LogError("DialogueData o PlayerData no están asignados en DialogueTrigger.");
                }
            }
            else
            {
                Debug.LogError("DialogueManager.Instance es null. Asegúrate de que DialogueManager esté presente en la escena.");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            hasTriggered = false;
        }
    }

    private void OnValidate()
    {
        if (dialogueData == null)
        {
            Debug.LogWarning("DialogueData no está asignado en DialogueTrigger.");
        }
        if (playerData == null)
        {
            Debug.LogWarning("PlayerData no está asignado en DialogueTrigger.");
        }
    }
}
