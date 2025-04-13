using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingBibleWeapon : ProjectileWeapon
{
    public void SpawnRing(PlayerStats player = null)
    {
        // Only activate this if the player is present.
        if (player)
        {
            float angleOffset = 2 * Mathf.PI / Mathf.Max(1, currentStats.number + owner.Stats.amount); // 2 * Mathf.PI = 360. We are just calculating how far of an angle to space out the prefabs from each other on a circle's circumference.
            float currentAngle = 0;
            for (int i = 0; i < currentStats.number + owner.Stats.amount; i++)
            {
                // Convert the spawn angle onto a point on the circle's circumference and space it away relative to the player.
                Vector3 spawnPosition = player.transform.position + new Vector3(
                    GetArea() * Mathf.Cos(currentAngle),
                    GetArea() * Mathf.Sin(currentAngle)
                );

                // Spawn the book at the calculated position, and parents it to the owner so it follows them around.
                Projectile prefab = Instantiate(currentStats.projectilePrefab, spawnPosition, Quaternion.identity, owner.transform);

                prefab.owner = owner;
                currentAngle += angleOffset;
                prefab.weapon = this;
            }
        }
    }
    protected override bool Attack(int attackCount = 1)
    {
        // If no projectile prefab is assigned, leave a warning message.
        if (!currentStats.projectilePrefab)
        {
            Debug.LogWarning(string.Format("Projectile prefab has not been set for {0}", name));
            ActivateCooldown(true);
            return false;
        }

        // Can we attack?
        if (!CanAttack()) return false;

        SpawnRing(owner);

        ActivateCooldown(true);

        attackCount--;

        // Do we perform another attack?
        if (attackCount > 0)
        {
            currentAttackCount = attackCount;
            currentAttackInterval = ((WeaponData)data).baseStats.projectileInterval;
        }


        return true;
    }

    public override bool ActivateCooldown(bool strict = false)
    {
        // When 'strict' is enabled and the cooldown is not yet finished,
        // do not refresh the cooldown.
        if (strict && currentCooldown > 0) return false;

        // Calculate what the cooldown is going to be, factoring in the cooldown
        // reduction stat in the player character.
        // Cooldown is dependent on lifespan, so we add it.
        float actualCooldown = (currentStats.lifespan + currentStats.cooldown) * Owner.Stats.cooldown;

        // Limit the maximum cooldown to the actual cooldown, so we cannot increase
        // the cooldown above the cooldown stat if we accidentally call this function
        // multiple times.
        currentCooldown = Mathf.Min(actualCooldown, currentCooldown + actualCooldown);
        return true;
    }

    public override bool DoLevelUp()
    {
        base.DoLevelUp();
        // Spawn a ring after every upgrade option, just like in the original game.
        SpawnRing(owner);
        ActivateCooldown(false);
        return true;
    }
}