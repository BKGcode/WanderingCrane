using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New NPC Dialogue", menuName = "Dialogue/NPC Dialogue Data")]
public class NPCDialogueData : ScriptableObject
{
    public string npcName;
    public Sprite avatar;
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
}

[System.Serializable]
public class DialogueLine
{
    public Speaker speaker;
    public string textKey; // Clave para la localización en lugar de texto directo
}

public enum Speaker
{
    NPC,
    Player
}