// Assets/ScriptableObjects/PlayerSettings.cs
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "ScriptableObjects/PlayerSettings", order = 1)]
public class PlayerSettings : ScriptableObject
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 7f;

    [Header("Combat Settings")]
    public float attackCooldown = 1f;
    public int attackDamage = 10;

    [Header("Health Settings")]
    public int maxHealth = 100;
}
