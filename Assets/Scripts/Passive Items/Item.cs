using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for both the Passive and the Weapon classes. It is primarily intended
/// to handle weapon evolution, as we want both weapons and passives to be evolve-able.
/// </summary>
public abstract class Item : MonoBehaviour
{
    public int currentLevel = 1, maxLevel = 1;
    protected ItemData.Evolution[] evolutionData;
    protected PlayerInventory inventory;
    protected PlayerStats owner;

    public virtual void Initialise(ItemData data)
    {
        maxLevel = data.maxLevel;

        // Store the evolution data as we have to track whether
        // all the catalysts are in the inventory so we can evolve.
        evolutionData = data.evolutionData;

        // We have to find a better way to reference the player inventory
        // in future, as this is inefficient.
        inventory = FindObjectOfType<PlayerInventory>();
        owner = FindObjectOfType<PlayerStats>();
    }

    // Call this function to get all the evolutions that the weapon
    // can currently evolve to.
    public virtual ItemData.Evolution[] CanEvolve()
    {
        List<ItemData.Evolution> possibleEvolutions = new List<ItemData.Evolution>();

        // Check each listed evolution and whether it is in
        // the inventory.
        foreach(ItemData.Evolution e in evolutionData)
        {
            if (CanEvolve(e)) possibleEvolutions.Add(e);
        }


        return possibleEvolutions.ToArray();
    }

    // Checks if a specific evolution is possible.
    public virtual bool CanEvolve(ItemData.Evolution evolution, int levelUpAmount = 1)
    {
        // Cannot evolve if the item hasn't reached the level to evolve.
        if (evolution.evolutionLevel > currentLevel + levelUpAmount)
        {
            Debug.LogWarning(string.Format("Evolution failed. Current level {0}, evolution level {1}", currentLevel, evolution.evolutionLevel));
            return false;
        }

        // Checks to see if all the catalysts are in the inventory.
        foreach (ItemData.Evolution.Config c in evolution.catalysts)
        {
            Item item = inventory.Get(c.itemType);
            if (!item || item.currentLevel < c.level)
            {
                Debug.LogWarning(string.Format("Evolution failed. Missing {0}", c.itemType.name));
                return false;
            }
        }

        return true;
    }

    // AttemptEvolution will spawn a new weapon for the character, and remove all
    // the weapons that are supposed to be consumed.
    public virtual bool AttemptEvolution(ItemData.Evolution evolutionData, int levelUpAmount = 1)
    {
        if (!CanEvolve(evolutionData, levelUpAmount))
            return false;
        
        // Should we consume passives / weapons?
        bool consumePassives = (evolutionData.consumes & ItemData.Evolution.Consumption.passives) > 0;
        bool consumeWeapons = (evolutionData.consumes & ItemData.Evolution.Consumption.weapons) > 0;
        
        // Loop through all the catalysts and check if we should consume them.
        foreach(ItemData.Evolution.Config c in evolutionData.catalysts)
        {
            if (c.itemType is PassiveData && consumePassives) inventory.Remove(c.itemType, true);
            if (c.itemType is WeaponData && consumeWeapons) inventory.Remove(c.itemType, true);
        }

        // Should we consume ourselves as well?
        if (this is Passive && consumePassives) inventory.Remove((this as Passive).data, true);
        else if (this is Weapon && consumeWeapons) inventory.Remove((this as Weapon).data, true);

        // Add the new weapon onto our inventory.
        inventory.Add(evolutionData.outcome.itemType);

        return true;
    }

    public virtual bool CanLevelUp()
    {
        return currentLevel <= maxLevel;
    }

    // Whenever an item levels up, attempt to make it evolve.
    public virtual bool DoLevelUp()
    {
        if (evolutionData == null) return true;

        // Tries to evolve into every listed evolution of this weapon,
        // if the weapon's evolution condition is levelling up.
        foreach (ItemData.Evolution e in evolutionData)
        {
            if(e.condition == ItemData.Evolution.Condition.auto)
                AttemptEvolution(e);
        }
        return true;
    }

    // What effects you receive on equipping an item.
    public virtual void OnEquip() { }

    // What effects are removed on unequipping an item.
    public virtual void OnUnequip() { }
}