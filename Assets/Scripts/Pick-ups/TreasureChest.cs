using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TreasureChest : MonoBehaviour
{



    private void OnTriggerEnter2D(Collider2D col)
    {

        PlayerInventory p = col.GetComponent<PlayerInventory>();
        if (p)
        {
            bool randomBool = Random.Range(0, 2) == 0;


            OpenTreasureChest(p, randomBool);

            Destroy(gameObject);
        }
    }

    public void OpenTreasureChest(PlayerInventory inventory, bool isHigherTier)
    {
        // Try weapon evolution first
        if (TryWeaponEvolution(inventory)) return;

        // If no evolution happened, give random upgrades
        int upgradeCount = DetermineUpgradeCount(isHigherTier);
        Debug.Log("UpgradeCount = " + upgradeCount);
        ApplyRandomUpgrades(inventory, upgradeCount);
    }

    private bool TryWeaponEvolution(PlayerInventory inventory)
    {
        if (inventory == null) return false;

        foreach (PlayerInventory.Slot s in inventory.weaponSlots)
        {
            // Skip empty slots
            if (s?.item == null) continue;

            // Try to cast to Weapon
            Weapon w = s.item as Weapon;
            if (w?.data?.evolutionData == null)
            {
                continue;
            }

            // Check each evolution
            foreach (ItemData.Evolution e in w.data.evolutionData)
            {
                // Skip if evolution is invalid or not a treasure chest evolution
                if (object.ReferenceEquals(e, null) || e.condition != ItemData.Evolution.Condition.treasureChest)
                    continue;

                // Try to evolve
                if (w.AttemptEvolution(e, 0))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private int DetermineUpgradeCount(bool isHigherTier)
    {
        float roll = Random.value * 100f;

        if (roll < 3f) return 5;      // Ultra Rare (3%) - 5 upgrades
        if (roll < 15f) return 3;     // Rare (12%) - 3 upgrades
        if (roll < 40f) return 2;     // Uncommon (25%) - 2 upgrades
        return 1;                     // Common (60%) - 1 upgrade
    }

    private void ShowTreasureScreen(List<UpgradeEvent> upgradeEvents)
    {
        if (GameManager.instance == null) return;

        // Pause game and show screen
        Time.timeScale = 0f;
        GameManager.instance.treasureChestScreen.SetActive(true);

        // Disable all upgrade icons first
        for (int i = 0; i < GameManager.instance.upgradeIcons.Length; i++)
        {
            GameManager.instance.upgradeIcons[i].gameObject.SetActive(false);
        }

            // Enable and fill icons based on upgrade count
        switch (upgradeEvents.Count)
        {
            case 1:
                // For 1 upgrade, use icon 1
                SetUpgradeIcon(0, upgradeEvents[0]);
                break;
            case 2:
                // For 2 upgrades, use icons 2 and 3
                SetUpgradeIcon(1, upgradeEvents[0]);
                SetUpgradeIcon(2, upgradeEvents[1]);
                break;
            case 3:
                // For 3 upgrades, use icons 1, 2, and 3
                SetUpgradeIcon(0, upgradeEvents[0]);
                SetUpgradeIcon(1, upgradeEvents[1]);
                SetUpgradeIcon(2, upgradeEvents[2]);
                break;
            case 5:
                // For 5 upgrades, use all icons
                for (int i = 0; i < upgradeEvents.Count; i++)
                {
                    SetUpgradeIcon(i, upgradeEvents[i]);
                }
                break;
        }

        void SetUpgradeIcon(int iconIndex, UpgradeEvent upgradeEvent)
        {
            Image iconImage = GameManager.instance.upgradeIcons[iconIndex];
            iconImage.gameObject.SetActive(true);

            if (upgradeEvent.item is Weapon weapon)
            {
                iconImage.sprite = weapon.data.icon;
            }
            else if (upgradeEvent.item is Passive passive)
            {
                iconImage.sprite = passive.data.icon;
            }
        }

        // Clear previous detailed upgrade displays
        foreach (Transform child in GameManager.instance.upgradeDisplayContainer)
        {
            if (child.gameObject != GameManager.instance.upgradeDisplayTemplate)
                Destroy(child.gameObject);
        }
        
        // Hide template
        GameManager.instance.upgradeDisplayTemplate.SetActive(false);

        // Create detailed upgrade displays
        foreach (var upgradeEvent in upgradeEvents)
        {
            GameObject display = Instantiate(GameManager.instance.upgradeDisplayTemplate, 
                GameManager.instance.upgradeDisplayContainer);
            display.SetActive(true);

            // Get references
            Transform iconTransform = display.transform.Find("Icon");
            Image icon = iconTransform.Find("Item Icon").GetComponent<Image>();
            Image iconBorder = iconTransform.Find("Icon Border").GetComponent<Image>();
            TMP_Text description = display.transform.Find("Description").GetComponent<TMP_Text>();
            TMP_Text itemName = display.transform.Find("Name").GetComponent<TMP_Text>();
            TMP_Text level = display.transform.Find("Level").GetComponent<TMP_Text>();
            Image frame = display.transform.Find("Frame").GetComponent<Image>();

            // Get level data
            Item.LevelData levelData = upgradeEvent.item.data.GetLevelData(upgradeEvent.levelAfterUpgrade);

            // Set data
            if (upgradeEvent.item is Weapon weapon)
            {
                icon.sprite = weapon.data.icon;
                description.text = levelData.description;
                itemName.text = levelData.name;
                level.text = $"Level {upgradeEvent.levelAfterUpgrade}";
            }
            else if (upgradeEvent.item is Passive passive)
            {
                icon.sprite = passive.data.icon;
                description.text = levelData.description;
                itemName.text = levelData.name;
                level.text = $"Level {upgradeEvent.levelAfterUpgrade}";
            }
        }

        // Setup continue button
        GameManager.instance.continueButton.onClick.RemoveAllListeners();
        GameManager.instance.continueButton.onClick.AddListener(() => {
            GameManager.instance.CloseTreasureScreen();
        });
    }

    private struct UpgradeEvent
    {
        public Item item;
        public int levelAfterUpgrade;

        public UpgradeEvent(Item item, int level)
        {
            this.item = item;
            this.levelAfterUpgrade = level;
        }
    }

    private void ApplyRandomUpgrades(PlayerInventory inventory, int upgradeCount)
    {
        List<Item> availableItems = new List<Item>();
        List<UpgradeEvent> upgradeEvents = new List<UpgradeEvent>();

        // Collect available items
        foreach (var slot in inventory.weaponSlots)
        {
            if (slot.item != null && slot.item.CanLevelUp())
            {
                availableItems.Add(slot.item);
            }
        }
        foreach (var slot in inventory.passiveSlots)
        {
            if (slot.item != null && slot.item.CanLevelUp())
            {
                availableItems.Add(slot.item);
            }
        }

        // Apply upgrades
        for (int i = 0; i < upgradeCount && availableItems.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, availableItems.Count);
            Item selectedItem = availableItems[randomIndex];

            if (selectedItem.DoLevelUp())
            {
                upgradeEvents.Add(new UpgradeEvent(selectedItem, selectedItem.currentLevel));
                GameManager.GenerateFloatingText("Level Up!", selectedItem.transform);
            }

            // Only remove from available items if it can't level up anymore
            if (!selectedItem.CanLevelUp())
            {
                availableItems.RemoveAt(randomIndex);
            }
        }

        // Update inventory UI
        inventory.weaponUI.Refresh();
        inventory.passiveUI.Refresh();

        // Show treasure screen
        ShowTreasureScreen(upgradeEvents);
    }
}

//todo if not evolve weapon, it will give 1-5 items an upgrade
        //common 60% - 1 ugprade
        //uncommon 25%- 2 upgrade
        //rare 12%- 3 upgrades
        // ultra rare 3%- 5 upgrades
        //loop through every item that the player have and select random items based on the amount of upgrades
        //selected from above


        //todo lista todos os itens que o player possui e