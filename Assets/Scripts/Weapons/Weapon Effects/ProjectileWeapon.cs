using UnityEngine;

public class ProjectileWeapon : Weapon
{

    protected float currentAttackInterval;
    protected int currentAttackCount; // Number of times this attack will happen.

    protected override void Update()
    {
        base.Update();

        // Otherwise, if the attack interval goes from above 0 to below, we also call attack.
        if (currentAttackInterval > 0)
        {
            currentAttackInterval -= Time.deltaTime;
            if (currentAttackInterval <= 0) Attack(currentAttackCount);
        }
    }

    public override bool CanAttack()
    {
        if (currentAttackCount > 0) return true;
        return base.CanAttack();
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

        // Otherwise, calculate the angle and offset of our spawned projectile.
        float spawnAngle = GetSpawnAngle();

        // And spawn a copy of the projectile.
        Projectile prefab = Instantiate(
            currentStats.projectilePrefab,
            owner.transform.position + (Vector3)GetSpawnOffset(spawnAngle),
            Quaternion.Euler(0, 0, spawnAngle)
        );

        prefab.weapon = this;
        prefab.owner = owner;


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

    // Gets which direction the projectile should face when spawning.
    protected virtual float GetSpawnAngle()
    {
        return Mathf.Atan2(movement.lastMovedVector.y, movement.lastMovedVector.x) * Mathf.Rad2Deg;
    }

    // Generates a random point to spawn the projectile on, and
    // rotates the facing of the point by spawnAngle.
    protected virtual Vector2 GetSpawnOffset(float spawnAngle = 0)
    {
        return Quaternion.Euler(0, 0, spawnAngle) * new Vector2(
            Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax),
            Random.Range(currentStats.spawnVariance.yMin, currentStats.spawnVariance.yMax)
        );
    }
}