using UnityEngine;

/// <summary>
/// Base class for all weapons / passive items. The base class is used so that both WeaponData
/// and PassiveItemData are able to be used interchangeably if required.
/// </summary>
public abstract class ItemData : ScriptableObject
{
    public Sprite icon;
    public int maxLevel;

    [Range(0f, 5f)]
    [Tooltip("Controls how often this item appears in upgrades. 1 is default, higher values increase frequency, lower values decrease it.")]
    public float appearanceChance = 1f; // Default neutral value

    [System.Serializable]
    public struct Evolution
    {
        public string name;
        public enum Condition { auto, treasureChest }
        public Condition condition;

        [System.Flags] public enum Consumption { passives = 1, weapons = 2 }
        public Consumption consumes;

        public int evolutionLevel;
        public Config[] catalysts;
        public Config outcome;

        [System.Serializable]
        public struct Config
        {
            public ItemData itemType;
            public int level;
        }
    }

    public Evolution[] evolutionData;

    public abstract Item.LevelData GetLevelData(int level);
}