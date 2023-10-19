using UnityEngine;

/* Contains all the stats for a character. */

public class CharacterStats : MonoBehaviour
{
    public string characterName;
    public int level;

    public Stat maxHealth;          // Maximum amount of health
    public int currentHealth { get; protected set; }    // Current amount of health

    public Stat abilityPoints; // Determines how many actions can be done in one turn
    public Stat strenght; // Affects damages inflicted with physical attacks
    public Stat intelligence; // Affects damages inflicted with magical attacks
    public Stat constitution; // Affects dmgs taken from physical attacks
    public Stat mind; // Affects dmgs taken from magic
    public Stat dexterity; // affects your speed (turn rates), precision and evasion, as well as bow range
    public Stat luck; // Affects god intervention/level/critical rate

    public Stat attackRange; // Should always be one?
    public Stat movementRange; // Should always be 3 ?
    public int currentRange = 3;

    private int statPoints;

    public event System.Action OnHealthReachedZero;

    public virtual void Awake()
    {
        currentHealth = maxHealth.GetValue();
    }

    // Start with max HP.
    public virtual void Start()
    {
        maxHealth.baseValue = constitution.GetValue() * 3 + mind.GetValue() * 3;
    }

    // Damage the character
    public void TakeDamage(int damage)
    {
        // Subtract the armor value - Make sure damage doesn't go below 0.
        damage -= constitution.GetValue();
        damage = Mathf.Clamp(damage, 0, int.MaxValue);

        // Subtract damage from health
        currentHealth -= damage;
        Debug.Log(transform.name + " takes " + damage + " damage.");

        // If we hit 0. Die.
        if (currentHealth <= 0)
        {
            if (OnHealthReachedZero != null)
            {
                OnHealthReachedZero();
            }
        }
    }

    // Heal the character.
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth.GetValue());
    }



}