using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component to be attached to all Weapon prefabs. The Weapon prefab works together with the WeaponData
/// ScriptableObjects to manage and run the behaviours of all weapons in the game.
/// </summary>
public abstract class Weapon : Item
{
    [System.Serializable]
    public struct Stats
    {
        public string name, description;

        [Header("Visuals")]
        public Projectile projectilePrefab; // If attached, a projectile will spawn every time the weapon cools down.
        public Aura auraPrefab; // If attached, an aura will spawn when weapon is equipped.
        public ParticleSystem hitEffect;
        public Rect spawnVariance;

        [Header("Values")]
        public float lifespan; // If 0, it will last forever.
        public float damage, damageVariance, area, speed, cooldown, projectileInterval, knockback;
        public int number, piercing, maxInstances;

        // Allows us to use the + operator to add 2 Stats together.
        // Very important later when we want to increase our weapon stats.
        public static Stats operator +(Stats s1, Stats s2)
        {
            Stats result = new Stats();
            result.name = s2.name ?? s1.name;
            result.description = s2.description ?? s1.description;
            result.projectilePrefab = s2.projectilePrefab ?? s1.projectilePrefab;
            result.auraPrefab = s2.auraPrefab ?? s1.auraPrefab;
            result.hitEffect = s2.hitEffect == null ? s1.hitEffect : s2.hitEffect;
            result.spawnVariance = s2.spawnVariance;
            result.lifespan = s1.lifespan + s2.lifespan;
            result.damage = s1.damage + s2.damage;
            result.damageVariance = s1.damageVariance + s2.damageVariance;
            result.area = s1.area + s2.area;
            result.speed = s1.speed + s2.speed;
            result.cooldown = s1.cooldown + s2.cooldown;
            result.number = s1.number + s2.number;
            result.piercing = s1.piercing + s2.piercing;
            result.projectileInterval = s1.projectileInterval + s2.projectileInterval;
            result.knockback = s1.knockback + s2.knockback;
            return result;
        }

        // Get damage dealt.
        public float GetDamage()
        {
            return damage + Random.Range(0, damageVariance);
        }
    }

    protected Stats currentStats;

    public WeaponData data;

    protected float currentCooldown;

    protected PlayerMovement movement; // Reference to the player's movement.

    // For dynamically created weapons, call initialise to set everything up.
    public virtual void Initialise(WeaponData data)
    {
        base.Initialise(data);
        this.data = data;
        currentStats = data.baseStats;
        movement = GetComponentInParent<PlayerMovement>();
        ActivateCooldown();
    }

    protected virtual void Update()
    {
        currentCooldown -= Time.deltaTime;
        if (currentCooldown <= 0f) //Once the cooldown becomes 0, attack
        {
            Attack(currentStats.number + owner.Stats.amount);
        }
    }

    // Levels up the weapon by 1, and calculates the corresponding stats.
    public override bool DoLevelUp()
    {
        base.DoLevelUp();

        // Prevent level up if we are already at max level.
        if (!CanLevelUp())
        {
            Debug.LogWarning(string.Format("Cannot level up {0} to Level {1}, max level of {2} already reached.", name, currentLevel, data.maxLevel));
            return false;
        }

        // Otherwise, add stats of the next level to our weapon.
        currentStats += data.GetLevelData(++currentLevel);
        return true;
    }

    // Lets us check whether this weapon can attack at this current moment.
    public virtual bool CanAttack()
    {
        return currentCooldown <= 0;
    }

    // Performs an attack with the weapon.
    // Returns true if the attack was successful.
    // This doesn't do anything. We have to override this at the child class to add a behaviour.
    protected virtual bool Attack(int attackCount = 1)
    {
        if (CanAttack())
        {
            ActivateCooldown();
            return true;
        }
        return false;
    }

    // Gets the amount of damage that the weapon is supposed to deal.
    // Factoring in the weapon's stats (including damage variance),
    // as well as the character's Might stat.
    public virtual float GetDamage()
    {
        return currentStats.GetDamage() * owner.Stats.might;
    }

    // Get the area, including modifications from the player's stats.
    public virtual float GetArea()
    {
        return currentStats.area * owner.Stats.area;
    }

    // For retrieving the weapon's stats.
    public virtual Stats GetStats() { return currentStats; }

    // Refreshes the cooldown of the weapon.
    // If <strict> is true, refreshes only when currentCooldown < 0.
    public virtual bool ActivateCooldown(bool strict = false)
    {
        // When <strict> is enabled and the cooldown is not yet finished,
        // do not refresh the cooldown.
        if(strict && currentCooldown > 0) return false;

        // Calculate what the cooldown is going to be, factoring in the cooldown
        // reduction stat in the player character.
        float actualCooldown = currentStats.cooldown * Owner.Stats.cooldown;

        // Limit the maximum cooldown to the actual cooldown, so we cannot increase
        // the cooldown above the cooldown stat if we accidentally call this function
        // multiple times.
        currentCooldown = Mathf.Min(actualCooldown, currentCooldown + actualCooldown);
        return true;
    }
}