using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// We require a VerticalLayoutGroup on the GameObject this is
// attached to, because it uses the component to make sure the
// buttons are evenly spaced out.
[RequireComponent(typeof(VerticalLayoutGroup))]
public class UIUpgradeWindow : MonoBehaviour
{
    // We will need to access the padding / spacing attributes on the layout.
    VerticalLayoutGroup verticalLayout;

    // The button and tooltip template GameObjects we have to assign.
    public RectTransform upgradeOptionTemplate;
    public TextMeshProUGUI tooltipTemplate;

    [Header("Settings")]
    public int maxOptions = 4; // We cannot show more options than this.
    public string newText = "New!"; // The text that shows when a new upgrade is shown;

    // Color of the "New!" text and the regular text.
    public Color newTextColor = Color.yellow, levelTextColor = Color.white;

    // These are the paths to the different UI elements in the <upgradeOptionTemplate>.
    [Header("Paths")]
    public string iconPath = "Icon/Item Icon";
    public string namePath = "Name", descriptionPath = "Description", buttonPath = "Button", levelPath = "Level";

    // These are private variables that are used by the functions to track the status
    // of different things in the UIUpgradeWindow.
    RectTransform rectTransform; // The RectTransform of this element for easy reference.
    float optionHeight; // The default height of the upgradeOptionTemplate.
    int activeOptions; // Tracks the number of options that are active currently.

    // This is a list of all the upgrade buttons on the window.
    List<RectTransform> upgradeOptions = new List<RectTransform>();

    // This is used to track the screen width / height of the last frame.
    // To detect screen size changes, so we know when we have to recalculate the size.
    Vector2 lastScreen;

    // This is the main function that we will be calling on this script.
    // You need to specify which <inventory> to add the item to, and a list of all
    // <possibleUpgrades> to show. It will select <pick> number of upgrades and show
    // them. Finally, if you specify a <tooltip>, then some text will appear at the bottom of
    // the window.
    public void SetUpgrades(PlayerInventory inventory, List<ItemData> possibleUpgrades, int pick = 3, string tooltip = "") 
    {
        pick = Mathf.Min(maxOptions, pick);
        
        // If we don't have enough upgrade option boxes, create them.
        if (maxOptions > upgradeOptions.Count)
        {
            for (int i = upgradeOptions.Count; i < pick; i++)
            {
                GameObject go = Instantiate(upgradeOptionTemplate.gameObject, transform);
                upgradeOptions.Add((RectTransform)go.transform);
            }
        }

        // If a string is provided, turn on the tooltip.
        tooltipTemplate.text = tooltip;
        tooltipTemplate.gameObject.SetActive(tooltip.Trim() != "");

        // Activate only the number of upgrade options we need, and arm the buttons and the
        // different attributes like descriptions, etc.
        activeOptions = 0;
        int totalPossibleUpgrades = possibleUpgrades.Count; // How many upgrades do we have to choose from?
        foreach(RectTransform r in upgradeOptions)
        {
            if (activeOptions < pick && activeOptions < totalPossibleUpgrades)
            {
                r.gameObject.SetActive(true);

                // Select one of the possible upgrades, then remove it from the list.
                ItemData selected = possibleUpgrades[Random.Range(0, possibleUpgrades.Count)];
                possibleUpgrades.Remove(selected);
                Item item = inventory.Get(selected);

                // Insert the name of the item.
                TextMeshProUGUI name = r.Find(namePath).GetComponent<TextMeshProUGUI>();
                if(name)
                {
                    name.text = selected.name;
                }

                // Insert the current level of the item, or a "New!" text if it is a new weapon.
                TextMeshProUGUI level = r.Find(levelPath).GetComponent<TextMeshProUGUI>();
                if(level)
                {
                    if(item) 
                    {
                        if (item.currentLevel >= item.maxLevel)
                        {
                            level.text = "Max!";
                            level.color = newTextColor;
                        }
                        else
                        {
                            level.text = selected.GetLevelData(item.currentLevel + 1).name;
                            level.color = levelTextColor;
                        }
                    }
                    else
                    {
                        level.text = newText;
                        level.color = newTextColor;
                    }
                }

                // Insert the description of the item.
                TextMeshProUGUI desc = r.Find(descriptionPath).GetComponent<TextMeshProUGUI>();
                if (desc)
                {
                    if (item)
                    {
                        desc.text = selected.GetLevelData(item.currentLevel + 1).description;
                    }
                    else
                    {
                        desc.text = selected.GetLevelData(1).description;
                    }
                }

                // Insert the icon of the item.
                Image icon = r.Find(iconPath).GetComponent<Image>();
                if(icon)
                {
                    icon.sprite = selected.icon;
                }

                // Insert the button action binding.
                Button b = r.Find(buttonPath).GetComponent<Button>();
                if (b)
                {
                    b.onClick.RemoveAllListeners();
                    if (item)
                        b.onClick.AddListener(() => inventory.LevelUp(item));
                    else
                        b.onClick.AddListener(() => inventory.Add(selected));
                }

                activeOptions++;
            }
            else r.gameObject.SetActive(false);
        }

        // Sizes all the elements so they do not exceed the size of the box.
        RecalculateLayout();
    }

    // Recalculates the heights of all elements.
    // Called whenever the size of the window changes.
    // We are doing this manually because the VerticalLayoutGroup doesn't always
    // space all the elements evenly.
    void RecalculateLayout()
    {
        
        // Calculates the total available height for all options, then divides it by the number of options.
        optionHeight = (rectTransform.rect.height - verticalLayout.padding.top - verticalLayout.padding.bottom - (maxOptions - 1) * verticalLayout.spacing);
        if (activeOptions == maxOptions && tooltipTemplate.gameObject.activeSelf)
            optionHeight /= maxOptions + 1;
        else
            optionHeight /= maxOptions;

        // Recalculates the height of the tooltip as well if it is currently active.
        if (tooltipTemplate.gameObject.activeSelf)
        {
            RectTransform tooltipRect = (RectTransform)tooltipTemplate.transform;
            tooltipTemplate.gameObject.SetActive(true);
            tooltipRect.sizeDelta = new Vector2(tooltipRect.sizeDelta.x, optionHeight);
            tooltipTemplate.transform.SetAsLastSibling();
        }

        // Sets the height of every active Upgrade Option button.
        foreach (RectTransform r in upgradeOptions)
        {
            if (!r.gameObject.activeSelf) continue;
            r.sizeDelta = new Vector2(r.sizeDelta.x, optionHeight);
        }
    }

    // This function just checks if the last screen width / height
    // is the same as the current one. If not, the screen has changed sizes
    // and we will call RecalculateLayout() to update the height of our buttons.
    void Update()
    {
        if (GameManager.instance?.treasureChestScreen?.activeSelf == true) return;

        // Redraws the boxes in this element if the screen size changes.
        if (lastScreen.x != Screen.width || lastScreen.y != Screen.height)
        {
            RecalculateLayout();
            lastScreen = new Vector2(Screen.width, Screen.height);
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        // Populates all our important variables.
        verticalLayout = GetComponentInChildren<VerticalLayoutGroup>();
        if (tooltipTemplate) tooltipTemplate.gameObject.SetActive(false);
        if (upgradeOptionTemplate) upgradeOptions.Add(upgradeOptionTemplate);

        // Get the RectTransform of this object for height calculations.
        rectTransform = (RectTransform)transform;
    }

    // Just a convenience function to automatically populate our variables.
    // It will automatically search for a GameObject called "Upgrade Option" and assign
    // it as the upgradeOptionTemplate, then search for a GameObject "Tooltip" to be assigned
    // as the tooltipTemplate.
    void Reset()
    {
        upgradeOptionTemplate = (RectTransform)transform.Find("Upgrade Option");
        tooltipTemplate = transform.Find("Tooltip").GetComponentInChildren<TextMeshProUGUI>();
    }
}