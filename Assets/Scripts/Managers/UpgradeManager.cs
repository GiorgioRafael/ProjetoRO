using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance;

    [Header("Upgrades")]
    public List<UpgradeData> upgrades; // List of all available upgrades

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Upgrade a specific upgrade if the player has enough coins
    public bool Upgrade(UpgradeData upgrade, int playerCoins)
    {
        if (upgrade.currentLevel < upgrade.maxLevel && playerCoins >= upgrade.costPerLevel)
        {
            upgrade.currentLevel++;
            DataManager.instance.AddCoin(-upgrade.costPerLevel); // Deduct coins
            return true;
        }
        return false;
    }

    // Save upgrade progress
    public void SaveUpgrades()
    {
        foreach (var upgrade in upgrades)
        {
            PlayerPrefs.SetInt(upgrade.name + "_Level", upgrade.currentLevel);
        }
    }

    // Load upgrade progress
    public void LoadUpgrades()
    {
        foreach (var upgrade in upgrades)
        {
            upgrade.currentLevel = PlayerPrefs.GetInt(upgrade.name + "_Level", 0);
        }
    }
}