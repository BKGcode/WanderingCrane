// DialogueData.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    public string npcName;
    public Sprite npcAvatar;
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
}

[System.Serializable]
public class DialogueLine
{
    public Speaker speaker;
    public List<LocalizedText> localizedTexts = new List<LocalizedText>();
}

[System.Serializable]
public class LocalizedText
{
    public string languageCode;
    [TextArea]
    public string text;
}

public enum Speaker
{
    NPC,
    Player
}
