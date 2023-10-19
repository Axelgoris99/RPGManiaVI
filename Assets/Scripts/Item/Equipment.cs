using UnityEngine;

/* An Item that can be equipped to increase armor/damage. */

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Equipment")]
public class Equipment : Item
{

    public EquipmentSlot equipSlot;     // What slot to equip it in

    public int maxHealth;          // Maximum amount of health
    public int currentHealth { get; protected set; }    // Current amount of health

    public int abilityPoints; // Determines how many actions can be done in one turn
    public int strenght; // Affects damages inflicted with physical attacks
    public int intelligence; // Affects damages inflicted with magical attacks
    public int constitution; // Affects dmgs taken from physical attacks
    public int mind; // Affects dmgs taken from magic
    public int dexterity; // affects your speed (turn rates), precision and evasion, as well as bow range
    public int luck; // Affects god intervention/level/critical rate

    public int attackRange; // depends on the weapon
    public int movementRange; // added to the character movement
    
    public SkinnedMeshRenderer prefab;

    // Called when pressed in the inventory
    public override void Use()
    {
        EquipmentManager.instance.Equip(this);  // Equip
        RemoveFromInventory();  // Remove from inventory
    }

}

public enum EquipmentSlot { Head, Chest, Legs, Weapon, Shield, Feet }