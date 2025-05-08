using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade Data", menuName = "2D Top-down Rogue-like/UpgradeData")]
public class UpgradeData : ScriptableObject
{
    public Sprite icon; // Icon for the upgrade
    public string upgradeName;
    public int maxLevel; // Maximum level for the upgrade
    public int currentLevel; // Current level of the upgrade
    public int costPerLevel; // Cost to upgrade to the next level

    [System.Serializable]
    public struct StatBoost
    {
        public string statName; // Name of the stat to boost
        public float boostAmount; // Amount to boost the stat per level
    }

    public StatBoost[] boosts; // Array of stat boosts for this upgrade

    // Method to get the total boost for a specific stat based on the current level
    public float GetBoost(string statName)
    {
        foreach (var boost in boosts)
        {
            if (boost.statName == statName)
            {
                return boost.boostAmount * currentLevel;
            }
        }
        return 0f; // Return 0 if the stat is not found
    }
}