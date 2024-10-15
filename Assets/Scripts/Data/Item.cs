// Assets/Scripts/Inventory/Item.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "ScriptableObjects/Item", order = 2)]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public bool isStackable;
    public int maxStack = 99;

    // Puedes agregar más propiedades según las necesidades del juego
}
