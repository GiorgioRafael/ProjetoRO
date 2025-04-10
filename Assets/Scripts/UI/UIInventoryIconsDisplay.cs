using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(LayoutGroup))]
public class UIInventoryIconsDisplay : MonoBehaviour
{

    public GameObject slotTemplate;
    public uint maxSlots = 6;
    public bool showLevels = true;
    public PlayerInventory inventory;

    public GameObject[] slots;

    [Header("Paths")]
    public string iconPath;
    public string levelTextPath;
    [HideInInspector] public string targetedItemList;

    void Reset()
    {
        slotTemplate = transform.GetChild(0).gameObject;
        inventory = FindObjectOfType<PlayerInventory>();
    }

    void OnEnable()
    {
        Refresh();
    }

    // This will read the inventory and see if there are any new updates
    // to the items on the PlayerCharacter.
    public void Refresh()
    {
        if (!inventory) Debug.LogWarning("No inventory attached to the UI icon display.");

        // Figure out which inventory I want.
        Type t = typeof(PlayerInventory);
        FieldInfo field = t.GetField(targetedItemList, BindingFlags.Public | BindingFlags.Instance);

        // If the given field is not found, then show a warning.
        if (field == null)
        {
            Debug.LogWarning("The list in the inventory is not found.");
            return;
        }

        // Get the list of inventory slots.
        List<PlayerInventory.Slot> items = (List<PlayerInventory.Slot>)field.GetValue(inventory);

        // Start populating the icons.
        for(int i = 0; i < items.Count; i++)
        {
            // Check if we have enough slots for the item.
            // Otherwise let's print a warning so that our users set this component up properly.
            if(i >= slots.Length)
            {
                Debug.LogWarning(
                    string.Format(
                        "You have {0} inventory slots, but only {1} slots on the UI.",
                        items.Count, slots.Length
                    )
                );
                break;
            }

            // Get the item data.
            Item item = items[i].item;

            Transform iconObj = slots[i].transform.Find(iconPath);
            if(iconObj)
            {
                Image icon = iconObj.GetComponentInChildren<Image>();

                // If the item doesn't exist, make the icon transparent.
                if (!item) icon.color = new Color(1, 1, 1, 0);
                else
                {
                    // Otherwise make it visible and update the icon.
                    icon.color = new Color(1, 1, 1, 1);
                    if (icon) icon.sprite = item.data.icon;
                }
            }

            // Set the level as well.
            Transform levelObj = slots[i].transform.Find(levelTextPath);
            if(levelObj)
            {
                // Find the Text component and put the level inside.
                TextMeshProUGUI levelTxt = levelObj.GetComponentInChildren<TextMeshProUGUI>();
                if (levelTxt)
                {
                    if (!item || !showLevels) levelTxt.text = "";
                    else levelTxt.text = item.currentLevel.ToString();
                }
            }
        }
    }
}