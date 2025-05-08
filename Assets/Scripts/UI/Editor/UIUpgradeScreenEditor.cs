using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

[DisallowMultipleComponent]
[CustomEditor(typeof(UIUpgradeScreen))]
public class UIUpgradeScreenEditor : Editor
{
    UIUpgradeScreen upgradeScreen;

    void OnEnable()
    {
        upgradeScreen = target as UIUpgradeScreen;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate Upgrades"))
        {
            GenerateUpgrades();
        }
    }

    private void GenerateUpgrades()
    {
        // Ensure the template and parent are assigned
        if (!upgradeScreen.template || !upgradeScreen.contentParent)
        {
            Debug.LogWarning("Please assign the template and content parent in the UIUpgradeScreen.");
            return;
        }

        // Clear existing upgrades
        for (int i = upgradeScreen.contentParent.childCount - 1; i >= 0; i--)
        {
            GameObject child = upgradeScreen.contentParent.GetChild(i).gameObject;
            if (child != upgradeScreen.template)
            {
                Undo.DestroyObjectImmediate(child);
            }
        }

        // Clear the generated upgrades list
        Undo.RecordObject(upgradeScreen, "Clear Generated Upgrades");
        upgradeScreen.generatedUpgrades.Clear();

        // Get all UpgradeData assets
        UpgradeData[] upgrades = GetAllUpgradeDataAssets();

        // Generate UI elements for each upgrade
        for (int i = 0; i < upgrades.Length; i++)
        {
            GameObject upgradeElement;

            if (i == 0)
            {
                // Use the template for the first element
                upgradeElement = upgradeScreen.template;
                Undo.RecordObject(upgradeElement, "Modify Template");
            }
            else
            {
                // Instantiate a new element for subsequent upgrades
                upgradeElement = Instantiate(upgradeScreen.template, upgradeScreen.contentParent);
                Undo.RegisterCreatedObjectUndo(upgradeElement, "Create Upgrade Element");
            }

            // Assign upgrade data to the UI element
            AssignUpgradeDataToUI(upgradeElement, upgrades[i]);

            // Add to the generated upgrades list
            upgradeScreen.generatedUpgrades.Add(upgradeElement);
        }

        // Mark the UIUpgradeScreen as dirty to save changes
        EditorUtility.SetDirty(upgradeScreen);
    }

    private void AssignUpgradeDataToUI(GameObject upgradeElement, UpgradeData upgradeData)
{
    // Set the upgrade name
    Transform nameTransform = upgradeElement.transform.Find(upgradeScreen.upgradeNamePath);
    if (nameTransform && nameTransform.TryGetComponent(out TextMeshProUGUI nameText))
    {
        nameText.text = upgradeData.upgradeName;
    }

    // Set the upgrade level
    Transform levelTransform = upgradeElement.transform.Find(upgradeScreen.upgradeLevelPath);
    if (levelTransform && levelTransform.TryGetComponent(out TextMeshProUGUI levelText))
    {
        // Since we're in editor, just show 0/maxLevel as the default
        levelText.text = $"Level: 0/{upgradeData.maxLevel}";
    }

    // Set the upgrade icon
    Transform iconTransform = upgradeElement.transform.Find(upgradeScreen.upgradeIconPath);
    if (iconTransform && iconTransform.TryGetComponent(out Image iconImage))
    {
        iconImage.sprite = upgradeData.icon;
    }
}

    private UpgradeData[] GetAllUpgradeDataAssets()
    {
#if UNITY_EDITOR
        List<UpgradeData> upgrades = new List<UpgradeData>();
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

        return upgrades.ToArray();
#else
        Debug.LogWarning("This function cannot be called outside the editor.");
        return new UpgradeData[0];
#endif
    }
}