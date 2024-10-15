// Assets/Scripts/Skills/Skill.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "ScriptableObjects/Skill", order = 3)]
public class Skill : ScriptableObject
{
    public string skillName;
    public Sprite icon;
    public float cooldown;
    public GameObject skillEffectPrefab;

    public void Activate(GameObject user)
    {
        // Implementa la lógica de activación de la habilidad
        if (skillEffectPrefab != null)
        {
            Instantiate(skillEffectPrefab, user.transform.position, Quaternion.identity);
        }
    }
}
