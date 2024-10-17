using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue Settings")]
    [SerializeField] private NPCDialogueData npcDialogueData;
    [SerializeField] private PlayerData playerData;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            Debug.Log($"Player ha entrado en el trigger de {npcDialogueData.npcName}");
            if (DialogueManager.Instance != null)
            {
                if (npcDialogueData != null && playerData != null)
                {
                    if (DialogueManager.Instance.Initialize(playerData, npcDialogueData))
                    {
                        hasTriggered = true;
                    }
                }
                else
                {
                    Debug.LogError("npcDialogueData o playerData no están asignados en DialogueTrigger.");
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
        if (npcDialogueData == null)
        {
            Debug.LogWarning("npcDialogueData no está asignado en DialogueTrigger.");
        }
        if (playerData == null)
        {
            Debug.LogWarning("playerData no está asignado en DialogueTrigger.");
        }
    }
}