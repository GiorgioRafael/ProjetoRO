using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEditor.Events;
using UnityEngine.UI;

[DisallowMultipleComponent]
[CustomEditor(typeof(UIUpgradeSelector))]
public class UIUpgradeSelectorEditor : Editor
{
    UIUpgradeSelector selector;

    void OnEnable()
    {
        selector = target as UIUpgradeSelector;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate Upgrades"))
        {
            CreateTogglesForUpgradeData();
        }
    }

    public void CreateTogglesForUpgradeData()
    {
        if (!selector.toggleTemplate)
        {
            Debug.LogWarning("Please assign a toggle template for the UI upgrade selector first.");
            return;
        }

        // Clear existing toggles except template
        for (int i = selector.toggleTemplate.transform.parent.childCount - 1; i >= 0; i--)
        {
            Toggle tog = selector.toggleTemplate.transform.parent.GetChild(i).GetComponent<Toggle>();
            if (tog == selector.toggleTemplate) continue;
            Undo.DestroyObjectImmediate(tog.gameObject);
        }

        // Record changes and clear list
        Undo.RecordObject(selector, "Updates to UIUpgradeSelector.");
        selector.selectableToggles.Clear();

        // Get all upgrade data
        UpgradeData[] upgrades = UIUpgradeSelector.GetAllUpgradeDataAssets();
        Debug.Log($"Found {upgrades.Length} upgrade assets");

        // Create toggle for each upgrade
        for (int i = 0; i < upgrades.Length; i++)
        {
            Toggle tog;
            if (i == 0)
            {
                tog = selector.toggleTemplate;
                Undo.RecordObject(tog, "Modifying the template.");
            }
            else
            {
                tog = Instantiate(selector.toggleTemplate, selector.toggleTemplate.transform.parent);
                Undo.RegisterCreatedObjectUndo(tog.gameObject, "Created new toggle.");
            }

            // Set upgrade name
            Transform upgradeName = tog.transform.Find(selector.upgradeNamePath);
            if (upgradeName && upgradeName.TryGetComponent(out TextMeshProUGUI tmp))
            {
                tmp.text = tog.gameObject.name = upgrades[i].upgradeName;
            }

            // Set upgrade level (default to 0 in editor)
            Transform upgradeLevel = tog.transform.Find(selector.upgradeLevelPath);
            if (upgradeLevel && upgradeLevel.TryGetComponent(out TextMeshProUGUI levelText))
            {
                levelText.text = $"Level: 0/{upgrades[i].maxLevel}";
            }

            // Set upgrade icon
            Transform upgradeIcon = tog.transform.Find(selector.upgradeIconPath);
            if (upgradeIcon && upgradeIcon.TryGetComponent(out Image icon))
            {
                icon.sprite = upgrades[i].icon;
            }

            selector.selectableToggles.Add(tog);

            // Remove existing listeners and add new one
            for (int j = 0; j < tog.onValueChanged.GetPersistentEventCount(); j++)
            {
                if (tog.onValueChanged.GetPersistentMethodName(j) == "Select")
                {
                    UnityEventTools.RemovePersistentListener(tog.onValueChanged, j);
                }
            }
            UnityEventTools.AddObjectPersistentListener(tog.onValueChanged, selector.Select, upgrades[i]);
            Debug.Log($"Added Select method to toggle for upgrade: {upgrades[i].upgradeName}");
        }

        EditorUtility.SetDirty(selector);
    }
}