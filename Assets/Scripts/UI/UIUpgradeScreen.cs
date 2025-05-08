using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIUpgradeScreen : MonoBehaviour
{
    [Header("Template")]
    public GameObject template; // The template GameObject
    public Transform contentParent; // Parent object for the generated upgrades

    [Header("Upgrade Paths")]
    public string upgradeNamePath = "Upgrade Name";
    public string upgradeLevelPath = "Upgrade Level";
    public string upgradeIconPath = "Upgrade Icon";

    [HideInInspector]
    public List<GameObject> generatedUpgrades = new List<GameObject>(); // List of generated upgrade UI elements
}