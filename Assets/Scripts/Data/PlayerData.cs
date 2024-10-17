// PlayerData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "DialogueSystem/PlayerData")]
public class PlayerData : ScriptableObject
{
    public string playerName;
    public Sprite avatar;
}
