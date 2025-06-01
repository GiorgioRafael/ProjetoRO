using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement; // Add this line

public class UICharacterSelector : MonoBehaviour
{
    public CharacterData defaultCharacter;
    public static CharacterData selected;
    public UIStatDisplay statsUI;

    [Header("Purchase")]
    public Button actionButton;
    public TextMeshProUGUI buttonText;
    public string nextSceneName = "Level Select";

    [Header("Template")]
    public Toggle toggleTemplate;
    public string characterNamePath = "Charater Name";
    public string weaponIconPath = "Weapon Icon";
    public string characterIconPath = "Character Icon";
    public List<Toggle> selectableToggles = new List<Toggle>();

    [Header("DescriptionBox")]
    public TextMeshProUGUI characterFullName;
    public TextMeshProUGUI characterDescription;
    public Image selectedCharacterIcon;
    public TextMeshProUGUI characterLore;

    [Header("WeaponDescriptionBox")]
    public Image selectedCharacterWeapon;
    public TextMeshProUGUI weaponName;
    public TextMeshProUGUI weaponDescription;


    [Header("Toggle Group")]
    [SerializeField] private ToggleGroup characterToggleGroup;


    void Start()
    {
        if (actionButton)
        {
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(HandleButtonClick);
        }

        // Get all characters once
        CharacterData[] allCharacters = GetAllCharacterDataAssets();
        Debug.Log($"Found {allCharacters.Length} characters");

        // Set initial toggle group and colors
        foreach (Toggle toggle in selectableToggles)
        {
            toggle.group = characterToggleGroup;

            // Get both icons
            Image characterIcon = toggle.transform.Find(characterIconPath)?.GetComponent<Image>();
            Image weaponIcon = toggle.transform.Find(weaponIconPath)?.GetComponent<Image>();
            if (characterIcon != null && characterIcon.sprite != null)
            {
                CharacterData toggleCharacter = System.Array.Find(allCharacters, c => c.Icon == characterIcon.sprite);
                if (toggleCharacter.Price == 0)
                {
                    DataManager.instance.UnlockCharacter(defaultCharacter.FullName);
                }
                if (toggleCharacter != null && DataManager.instance != null)
                {

                    Debug.Log("Price" + toggleCharacter.Price);

                    if (DataManager.instance.IsCharacterUnlocked(toggleCharacter.FullName))
                    {
                        // Debug.Log($"Setting {toggleCharacter.FullName} to white (unlocked)");
                        characterIcon.color = Color.white;
                        if (weaponIcon != null)
                        {
                            weaponIcon.color = Color.white;
                        }
                    }
                    else
                    {
                        // Debug.Log($"Setting {toggleCharacter.FullName} to black (locked)");
                        characterIcon.color = Color.black;
                        if (weaponIcon != null)
                        {
                            weaponIcon.color = Color.black;
                        }
                    }
                }
                else if (toggleCharacter == null)
                {
                    // This case handles if a toggle's sprite doesn't match any CharacterData icon
                    // Or if DataManager.instance is null (though less likely here if it's a singleton)
                    // Debug.LogWarning($"Could not find CharacterData for toggle with sprite: {characterIcon.sprite.name} or DataManager is null. Setting to black.");
                    characterIcon.color = Color.black; // Default to black if character data not found
                    if (weaponIcon != null)
                    {
                        weaponIcon.color = Color.black;
                    }
                }
            }
            else if (characterIcon == null || characterIcon.sprite == null)
            {
                // Debug.LogWarning($"Character icon or its sprite is null for one of the toggles. Path: {characterIconPath}");
                // Optionally handle this case, e.g., by disabling the toggle or logging an error
                // For now, we do nothing, it will retain its editor color or be invisible if no sprite
            }
        }

        if (defaultCharacter)
        {
            // Ensure default character is unlocked if not already
            if (!DataManager.instance.IsCharacterUnlocked(defaultCharacter.FullName))
            {
                 DataManager.instance.UnlockCharacter(defaultCharacter.FullName);
            }
            Select(defaultCharacter); // This will correctly set the selected character's UI elements
        }
    }

    //public CharacterData characterData;

    public static CharacterData[] GetAllCharacterDataAssets()
    {
        List<CharacterData> characters = new List<CharacterData>();

#if UNITY_EDITOR
        // Use t:CharacterData to specifically find CharacterData ScriptableObjects
        string[] guids = AssetDatabase.FindAssets("t:CharacterData");
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            CharacterData characterData = AssetDatabase.LoadAssetAtPath<CharacterData>(assetPath);
            if (characterData != null)
            {
                characters.Add(characterData);
            }
        }
#else
        // In a build, you'll need a different way to load CharacterData assets,
        // e.g., by placing them in a "Resources" folder and using Resources.LoadAll.
        // Or by using Addressables.
        CharacterData[] loadedCharacters = Resources.LoadAll<CharacterData>("CharacterData"); // Assuming they are in a folder named "Resources/CharacterData"
        if (loadedCharacters != null && loadedCharacters.Length > 0)
        {
            characters.AddRange(loadedCharacters);
        }
        else
        {
            Debug.LogWarning("No CharacterData assets found in Resources. Make sure they are in a 'Resources/CharacterData' folder for builds.");
        }
#endif

        return characters.ToArray();
    }

    public static CharacterData GetData()
    {
        //use the selected chosen in the Select() function
        if (selected)
            return selected;
        else
        {
            //randomly pick a character if we are playing from the editor
            CharacterData[] characters = GetAllCharacterDataAssets();
            if (characters.Length > 0) return characters[Random.Range(0, characters.Length)];
        }
        return null;
    }
    

    public void Select(CharacterData character)
    {
    if (character == null) return;

    selected = character;
    bool isUnlocked = DataManager.instance.IsCharacterUnlocked(character.FullName);

    // Update UI text
    characterFullName.text = character.FullName;
    characterDescription.text = character.CharacterDescription;
    characterLore.text = isUnlocked ? character.CharacterLore : "???";
    
    // Set character icon and color for silhouette effect in the main display
    selectedCharacterIcon.sprite = character.Icon;
    selectedCharacterIcon.color = isUnlocked ? Color.white : Color.black;

    // Update ALL toggle colors based on their individual unlock status,
    // and then specifically ensure the selected one reflects its state correctly.
    // This loop is now more for ensuring consistency if Select is called externally,
    // as Start() already sets initial states.
    // However, it's crucial for when a character is selected by clicking a toggle.
    CharacterData[] allCharacters = GetAllCharacterDataAssets(); // Re-fetch or pass if performance is an issue

    foreach(Toggle toggle in selectableToggles)
    {
        Image charIcon = toggle.transform.Find(characterIconPath)?.GetComponent<Image>();
        Image wepIcon = toggle.transform.Find(weaponIconPath)?.GetComponent<Image>();

        if (charIcon != null && charIcon.sprite != null)
        {
            CharacterData toggleChar = System.Array.Find(allCharacters, c => c.Icon == charIcon.sprite);
            if (toggleChar != null)
            {
                bool currentToggleUnlocked = DataManager.instance.IsCharacterUnlocked(toggleChar.FullName);
                Color targetColor = currentToggleUnlocked ? Color.white : Color.black;
                charIcon.color = targetColor;
                if (wepIcon != null)
                {
                    wepIcon.color = targetColor;
                }
            }
            else // Sprite on toggle doesn't match any known character
            {
                charIcon.color = Color.black; // Default to black
                 if (wepIcon != null)
                {
                    wepIcon.color = Color.black;
                }
            }
        }
    }

        if (isUnlocked)
        {
            Item.LevelData weaponLevelData = character.StartingWeapon.GetLevelData(1);
            weaponName.text = weaponLevelData.name;
            weaponDescription.text = weaponLevelData.description;
            selectedCharacterWeapon.sprite = character.StartingWeapon.icon;
            selectedCharacterWeapon.color = Color.white; // Ensure weapon icon is visible
        }
        else
        {
            weaponName.text = "???";
            weaponDescription.text = "???";
            characterDescription.text = "???";
            selectedCharacterWeapon.sprite = null; // Or a placeholder "locked" sprite
            selectedCharacterWeapon.color = Color.clear; // Hide if no sprite
        }

        buttonText.text = isUnlocked ? "Selecionar" : $"Comprar ({character.Price} moedas)";
        actionButton.interactable = isUnlocked || DataManager.instance.CoinCount >= character.Price;

        if (statsUI != null)
        {
            statsUI.character = character;
            statsUI.UpdateFields();
        }
    }

       private void HandleButtonClick()
    {
        if (selected == null) return;

        bool isUnlocked = DataManager.instance.IsCharacterUnlocked(selected.FullName);
        if (isUnlocked)
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else if (DataManager.instance.CoinCount >= selected.Price)
        {
            DataManager.instance.AddCoin(-selected.Price);
            DataManager.instance.UnlockCharacter(selected.FullName);
            Select(selected); // Refresh UI to show as unlocked and update button
        }
    }
}