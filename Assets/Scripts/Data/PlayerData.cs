using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "PlayerData", menuName = "DialogueSystem/PlayerData")]
public class PlayerData : ScriptableObject
{
    public string playerName;
    public Sprite avatar;
}
