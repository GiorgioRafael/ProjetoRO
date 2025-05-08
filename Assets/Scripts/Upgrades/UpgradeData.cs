using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Upgrade Data", menuName = "2D Top-down Rogue-like/UpgradeData")]
public class UpgradeData : ScriptableObject
{
    public enum StatType
    {
        MaxHealth,
        Recovery,
        Armor,
        MoveSpeed,
        Might,
        Area,
        Speed,
        Duration,
        Amount,
        Cooldown,
        Luck,
        Growth,
        Greed,
        Curse,
        Magnet,
        Revival
    }

    // Template data only
    public Sprite icon;
    public string upgradeName;
    public int maxLevel;
    public int costPerLevel;
    public string upgradeDescription;

    [System.Serializable]
    public struct StatBoost
    {
        public StatType statType;
        public float boostAmount;
        public string statName => statType.ToString().ToLower();
    }   

    public StatBoost[] boosts;

    // Modified to take current level as parameter
    public float GetBoost(string statName, int currentLevel)
    {
        if (currentLevel <= 0) return 0f;

        foreach (var boost in boosts)
        {
            if (string.Equals(boost.statName, statName, StringComparison.OrdinalIgnoreCase))
            {
                return boost.boostAmount * currentLevel;
            }
        }

        return 0f;
    }
}