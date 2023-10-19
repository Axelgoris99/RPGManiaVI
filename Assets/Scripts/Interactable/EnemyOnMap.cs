using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This makes our enemy interactable. */

[RequireComponent(typeof(CharacterStats))]
public class EnemyOnMap : Interactable
{

    CharacterStats stats;

    public override void Start()
    {
        base.Start();
        focusColor = Color.red;
        stats = GetComponent<CharacterStats>();
        stats.OnHealthReachedZero += Die;
    }

    // When we interact with the enemy: We attack it.
    public override void Interact()
    {
        print("Interact");
        FightManager combatManager = FightManager.Instance;
    }

    void Die()
    {
        Destroy(gameObject);
    }

}