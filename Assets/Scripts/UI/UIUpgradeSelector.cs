using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using System.Linq;

public class UIUpgradeSelector : MonoBehaviour, IDataPersistence
{
    public UpgradeData defaultUpgrade;

    private float saveDelay = 2f; // Save after 2 seconds of no purchases
    private float lastPurchaseTime;
    private bool needsSave = false;

    [Header("Template")]
    public Toggle toggleTemplate;
    public string upgradeNamePath = "Upgrade Name";
    public string upgradeLevelPath = "Upgrade Level";
    public string upgradeIconPath = "Upgrade Icon";
    public List<Toggle> selectableToggles = new List<Toggle>();

    [Header("DescriptionBox")]
    public TextMeshProUGUI upgradeName;
    public TextMeshProUGUI upgradeLevel;
    public Image upgradeIcon;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI descriptionText;

    [Header("Purchase")]
    public Button purchaseButton;
    private UpgradeData selectedUpgrade;

    [Header("Currency")]
    private int currentCoins;
    private Dictionary<string, int> upgradeLevels = new Dictionary<string, int>();
        
    [Header("Toggle Group")]
    [SerializeField] private ToggleGroup characterToggleGroup;

    void Start()
    {
        if (purchaseButton)
        {
            purchaseButton.onClick.AddListener(BuyUpgrade);
            purchaseButton.interactable = false;
        }
        UpdateCoinsDisplay();
        SelectNextAvailableUpgrade();

       foreach(Toggle toggle in selectableToggles)
        {
            toggle.group = characterToggleGroup;
        }
    }

    private void SelectNextAvailableUpgrade()
    {
        var allUpgrades = GetAllUpgradeDataAssets();
        if (allUpgrades.Length == 0) return;

        // Try to find first non-maxed upgrade
        UpgradeData nextUpgrade = allUpgrades.FirstOrDefault(upgrade => 
        {
            if (upgradeLevels.TryGetValue(upgrade.upgradeName, out int level))
            {
                return level < upgrade.maxLevel;
            }
            return true; // If upgrade not found in dictionary, it's level 0
        });

        // If all upgrades are maxed, select the first one
        if (nextUpgrade == null)
        {
            nextUpgrade = allUpgrades[0];
        }

        Select(nextUpgrade);
    }

    public void LoadData(GameData data)
    {
        this.currentCoins = data.coinCount;
        UpdateCoinsDisplay();

        var allUpgrades = GetAllUpgradeDataAssets();
        upgradeLevels.Clear();

        if (data.upgrades == null)
        {
            data.upgrades = new List<GameData.UpgradeInfo>();
        }

        foreach (var upgrade in allUpgrades)
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
            Debug.Log($"Loaded upgrade: {upgrade.upgradeName} at level {upgradeLevels[upgrade.upgradeName]}");
        }
        UpdateAllToggleLevels();
        DataPersistenceManager.instance.SaveGame();
    }

    public void SaveData(ref GameData data)
    {
        data.coinCount = currentCoins;
        data.upgrades.Clear();
        foreach (var upgrade in GetAllUpgradeDataAssets())
        {
            if (upgradeLevels.TryGetValue(upgrade.upgradeName, out int level))
            {
                data.upgrades.Add(new GameData.UpgradeInfo(upgrade.upgradeName, level));
            }
        }
    }

    private void UpdateAllToggleLevels()
    {
        foreach (Toggle toggle in selectableToggles)
        {
            // Get the upgrade name from the toggle's name
            string upgradeName = toggle.gameObject.name;
            
            // Find the level text component
            Transform levelTransform = toggle.transform.Find(upgradeLevelPath);
            if (levelTransform && levelTransform.TryGetComponent(out TextMeshProUGUI levelText))
            {
                // Get the upgrade data to get maxLevel
                var upgradeData = GetAllUpgradeDataAssets().FirstOrDefault(u => u.upgradeName == upgradeName);
                if (upgradeData != null && upgradeLevels.TryGetValue(upgradeName, out int level))
                {
                    levelText.text = $"Level: {level}/{upgradeData.maxLevel}";
                }
            }
        }
    }

    public static UpgradeData[] GetAllUpgradeDataAssets()
    {
        List<UpgradeData> upgrades = new List<UpgradeData>();

#if UNITY_EDITOR
        string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
        foreach (string assetPath in allAssetPaths)
        {
            if (assetPath.EndsWith(".asset"))
            {
                UpgradeData upgradeData = AssetDatabase.LoadAssetAtPath<UpgradeData>(assetPath);
                if (upgradeData != null)
                {
                    upgrades.Add(upgradeData);
                }
            }
        }
#else
        Debug.LogWarning("This function cannot be called on builds.");
#endif
        return upgrades.ToArray();
    }

    public void Select(UpgradeData upgrade)
    {
        if (upgrade == null)
        {
            Debug.LogError("Upgrade is null. Cannot select a null upgrade.");
            return;
        }

        selectedUpgrade = upgrade;
        int currentLevel = upgradeLevels[upgrade.upgradeName];

        upgradeName.text = upgrade.upgradeName;
        upgradeLevel.text = $"Level: {currentLevel}/{upgrade.maxLevel}";
        upgradeIcon.sprite = upgrade.icon;
        costText.text = $"{upgrade.costPerLevel}";
        descriptionText.text = upgrade.upgradeDescription;

        if (purchaseButton)
        {
            bool canAfford = currentCoins >= upgrade.costPerLevel;
            bool notMaxLevel = currentLevel < upgrade.maxLevel;
            purchaseButton.interactable = canAfford && notMaxLevel;
        }
    }
    
    void Update()
    {
        if (needsSave && Time.time - lastPurchaseTime >= saveDelay)
        {
            needsSave = false;
            DataPersistenceManager.instance.SaveGame();
        }
    }

    public void BuyUpgrade()
    {
        if (selectedUpgrade == null)
        {
            Debug.LogWarning("No upgrade selected to purchase.");
            return;
        }

        int currentLevel = upgradeLevels[selectedUpgrade.upgradeName];

        if (currentLevel >= selectedUpgrade.maxLevel)
        {
            Debug.LogWarning($"Upgrade {selectedUpgrade.upgradeName} is already at max level!");
            return;
        }

        if (currentCoins >= selectedUpgrade.costPerLevel)
        {
            currentCoins -= selectedUpgrade.costPerLevel;
            DataManager.instance.AddCoin(-selectedUpgrade.costPerLevel);

            upgradeLevels[selectedUpgrade.upgradeName] = currentLevel + 1;
            Select(selectedUpgrade);
            UpdateCoinsDisplay();
            UpdateAllToggleLevels(); // Add this line


            DataPersistenceManager.instance.SaveGame();
        }
        else
        {
            Debug.LogWarning("Not enough coins!");
        }
    }

    private void UpdateCoinsDisplay()
    {
        if (coinsText != null)
        {
            coinsText.text = $"Coins: {currentCoins}";
        }
    }

    void OnDestroy()
    {
        if (purchaseButton)
        {
            purchaseButton.onClick.RemoveListener(BuyUpgrade);
        }
    }
}