using System.Diagnostics;
using UnityEngine;
public class AxeWeapon : ProjectileWeapon
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    protected override bool Attack(int attackCount = 1)
{
    if (!currentStats.projectilePrefab || !CanAttack())
        return false;

    float spawnAngle = GetSpawnAngle();

    Projectile prefab = Instantiate(
        currentStats.projectilePrefab,
        owner.transform.position + (Vector3)GetSpawnOffset(spawnAngle),
        Quaternion.Euler(0, 0, spawnAngle)
    );

    // Flip the projectile if player is facing left
    if (movement.lastMovedVector.x < 0)
    {
        Vector3 scale = prefab.transform.localScale;
        scale.x = -Mathf.Abs(scale.x);
        prefab.transform.localScale = scale;
    }

    // Assign weapon and owner
    prefab.weapon = this;
    prefab.owner = owner;

    // Cooldown logic
    if (currentCooldown <= 0)
        currentCooldown += currentStats.cooldown;

    attackCount--;

    if (attackCount > 0)
    {
        currentAttackCount = attackCount;
        currentAttackInterval = data.baseStats.projectileInterval;
    }

    return true;
}
    
    protected override float GetSpawnAngle()
    {
        int offset = currentAttackCount > 0 ? currentStats.number - currentAttackCount : 1;

        //UnityEngine.Debug.Log($"LastMovedVector X: {movement.lastMovedVector.x}, Offset: {offset}");
        //UnityEngine.Debug.Log(90f - Mathf.Sign(movement.lastMovedVector.x) * (5 * offset));
        return 90f - Mathf.Sign(movement.lastMovedVector.x) * (5 * offset) * 2;

    }

    protected override Vector2 GetSpawnOffset(float spawnAngle = 0)
    {
        return new Vector2(
            Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax),
            Random.Range(currentStats.spawnVariance.yMin, currentStats.spawnVariance.yMax)
        );
    }

}