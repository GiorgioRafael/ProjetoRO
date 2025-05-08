using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int coinCount;
    public List<UpgradeInfo> upgrades;

    // the values defined in this constructor will be the default values
    // the game starts with when there's no data to load
    public GameData() 
    {
        this.coinCount = 0;
        this.upgrades = new List<UpgradeInfo>();
    }

        [System.Serializable]
        public struct UpgradeInfo
        {
            public string upgradeName; // Name of the upgrade
            public int upgradeLevel;   // Current level of the upgrade

            public UpgradeInfo(string name, int level)
            {
            this.upgradeName = name;
            this.upgradeLevel = level;
        }
    }
    
}