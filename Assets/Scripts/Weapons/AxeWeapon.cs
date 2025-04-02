using System.Diagnostics;
using UnityEngine;
public class AxeWeapon : ProjectileWeapon
{
    protected override float GetSpawnAngle()
    {
        int offset = currentAttackCount > 0 ? currentStats.number - currentAttackCount : 1;

        //UnityEngine.Debug.Log($"LastMovedVector X: {movement.lastMovedVector.x}, Offset: {offset}");
        UnityEngine.Debug.Log(90f - Mathf.Sign(movement.lastMovedVector.x) * (5 * offset));
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