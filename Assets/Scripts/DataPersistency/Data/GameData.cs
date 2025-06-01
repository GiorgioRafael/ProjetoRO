using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int coinCount;
    public List<UpgradeInfo> upgrades;
    public List<CharacterInfo> characters;

    // the values defined in this constructor will be the default values
    // the game starts with when there's no data to load
    public GameData()
    {
        this.coinCount = 0;
        this.upgrades = new List<UpgradeInfo>();
        this.characters = new List<CharacterInfo>();
    }

    [System.Serializable]
    public struct CharacterInfo
    {
        public string characterFullName;  // Using FullName as unique identifier
        public bool isUnlocked;          // Ownership status

        public CharacterInfo(string fullName, bool unlocked)
        {
            this.characterFullName = fullName;
            this.isUnlocked = unlocked;
        }
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