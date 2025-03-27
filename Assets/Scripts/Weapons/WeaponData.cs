using UnityEngine;

/// <summary>
/// Replacement for the WeaponScriptableObject class. The idea is we want to store all weapon evolution
/// data in one single object, instead of having multiple objects to store a single weapon, which is
/// what we would have had to do if we continued using WeaponScriptableObject.
/// </summary>
[CreateAssetMenu(fileName = "Weapon Data", menuName = "2D Top-down Rogue-like/Weapon Data")]
public class WeaponData : ItemData
{

    [HideInInspector] public string behaviour;
    public Weapon.Stats baseStats;
    public Weapon.Stats[] linearGrowth;
    public Weapon.Stats[] randomGrowth;

    // Gives us the stat growth / description of the next level.
    public Weapon.Stats GetLevelData(int level)
    {
        // Pick the stats from the next level.
        if (level - 2 < linearGrowth.Length)
            return linearGrowth[level - 2];

        // Otherwise, pick one of the stats from the random growth array.
        if (randomGrowth.Length > 0)
            return randomGrowth[Random.Range(0, randomGrowth.Length)];

        // Return an empty value and a warning.
        Debug.LogWarning(string.Format("Weapon doesn't have its level up stats configured for Level {0}!",level));
        return new Weapon.Stats();
    }

}