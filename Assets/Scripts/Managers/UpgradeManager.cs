using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour, IDataPersistence
{
    public static UpgradeManager instance;

    [Header("Upgrades")]
    public List<UpgradeData> upgrades;
    private Dictionary<string, int> upgradeLevels = new Dictionary<string, int>();

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

    public void LoadData(GameData data)
    {
        upgradeLevels.Clear();

        if (data.upgrades == null)
        {
            data.upgrades = new List<GameData.UpgradeInfo>();
        }

        // Initialize all upgrades in save data
        foreach (var upgrade in upgrades)
        {
            bool upgradeExists = data.upgrades.Exists(u => u.upgradeName == upgrade.upgradeName);
            
            if (upgradeExists)
            {
                var savedUpgrade = data.upgrades.Find(u => u.upgradeName == upgrade.upgradeName);
                upgradeLevels[upgrade.upgradeName] = savedUpgrade.upgradeLevel;
            }
            else
            {
                upgradeLevels[upgrade.upgradeName] = 0;
                data.upgrades.Add(new GameData.UpgradeInfo(upgrade.upgradeName, 0));
            }
        }
    }

    public void SaveData(ref GameData data)
    {
        data.upgrades.Clear();
        foreach (var upgrade in upgrades)
        {
            if (upgradeLevels.TryGetValue(upgrade.upgradeName, out int level))
            {
                data.upgrades.Add(new GameData.UpgradeInfo(upgrade.upgradeName, level));
            }
        }
    }

    public bool Upgrade(UpgradeData upgrade, int playerCoins)
    {
        if (!upgradeLevels.TryGetValue(upgrade.upgradeName, out int currentLevel))
        {
            currentLevel = 0;
            upgradeLevels[upgrade.upgradeName] = 0;
        }

        if (currentLevel < upgrade.maxLevel && playerCoins >= upgrade.costPerLevel)
        {
            upgradeLevels[upgrade.upgradeName] = currentLevel + 1;
            DataManager.instance.AddCoin(-upgrade.costPerLevel);
            DataPersistenceManager.instance.SaveGame();
            return true;
        }
        return false;
    }

    public int GetUpgradeLevel(string upgradeName)
    {
        return upgradeLevels.TryGetValue(upgradeName, out int level) ? level : 0;
    }
}