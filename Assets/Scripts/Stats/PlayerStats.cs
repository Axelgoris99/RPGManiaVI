using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
	This component is derived from CharacterStats. It adds two things:
		- Gaining modifiers when equipping items
		- Restarting the game when dying
*/

public class PlayerStats : CharacterStats
{
    public int expPoint;
    public int gold;

    public int swordProficiency;
    public int bowProficiency;
    public int staffProficiency;
    public int shieldProficiency;


    // Use this for initialization
    public override void Start()
    {
        base.Start();
        EquipmentManager.instance.onEquipmentChanged += OnEquipmentChanged;
    }

    void OnEquipmentChanged(Equipment newItem, Equipment oldItem)
    {
        if (newItem != null)
        {
            maxHealth.AddModifier(newItem.maxHealth);
            abilityPoints.AddModifier(newItem.abilityPoints);
            strenght.AddModifier(newItem.strenght);
            intelligence.AddModifier(newItem.intelligence);
            constitution.AddModifier(newItem.constitution);
            mind.AddModifier(newItem.mind);
            dexterity.AddModifier(newItem.dexterity);
            luck.AddModifier(newItem.luck);
            attackRange.AddModifier(newItem.attackRange);
            movementRange.AddModifier(newItem.movementRange);

        }

        if (oldItem != null)
        {
            maxHealth.RemoveModifier(oldItem.maxHealth);
            abilityPoints.RemoveModifier(oldItem.abilityPoints);
            strenght.RemoveModifier(oldItem.strenght);
            intelligence.RemoveModifier(oldItem.intelligence);
            constitution.RemoveModifier(oldItem.constitution);
            mind.RemoveModifier(oldItem.mind);
            dexterity.RemoveModifier(oldItem.dexterity);
            luck.RemoveModifier(oldItem.luck);
            attackRange.RemoveModifier(oldItem.attackRange);
            movementRange.RemoveModifier(oldItem.movementRange);
        }

    }



}