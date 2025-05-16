using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using Unity.VisualScripting;

public class UICharacterSelector : MonoBehaviour
{
    public CharacterData defaultCharacter;
    public static CharacterData selected;
    public UIStatDisplay statsUI;

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
        if(defaultCharacter) Select(defaultCharacter);


        foreach(Toggle toggle in selectableToggles)
        {
            toggle.group = characterToggleGroup;
        }
    }  

    //public CharacterData characterData;

    public static CharacterData[] GetAllCharacterDataAssets()
    {
        List<CharacterData> characters = new List<CharacterData>();

            //ramddomly pick a character if we are playing from the Game Scene
#if UNITY_EDITOR
            string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
            foreach (string assetPath in allAssetPaths)
            {
                if (assetPath.EndsWith(".asset"))
                {
                    CharacterData characterData = AssetDatabase.LoadAssetAtPath<CharacterData>(assetPath);
                    if (characterData != null)
                    {
                        characters.Add(characterData);
                    }
                }
            }
#else
        Debug.LogWarning("This function cannot be called on builds.");

#endif  
        return characters.ToArray();
    }

    public static CharacterData GetData()
    {
        //use the selected chosen in the Select() function
        if(selected)
            return selected;
        else
        {
            //randomly pick a character if we are playing from the editor
            CharacterData[] characters = GetAllCharacterDataAssets();
            if(characters.Length > 0) return characters[Random.Range(0, characters.Length)];
        }
        return null;
    }

    public void Select(CharacterData character)
    {

        //Debug.Log($"Select method called for character: {character?.FullName ?? "null"}");

        if (character == null)
        {
            Debug.LogError("Character is null. Cannot select a null character.");
            return;
        }

        selected = statsUI.character = character;
        statsUI.UpdateStatFields();

        characterFullName.text = character.FullName;
        characterDescription.text = character.CharacterDescription;
        selectedCharacterIcon.sprite = character.Icon;
        characterLore.text = character.CharacterLore;

        //SET DA ARMA
        //pega os dados do nivel 1 da arma para colocar a descricao 
        Item.LevelData weaponLevelData = character.StartingWeapon.GetLevelData(1);
        weaponName.text = weaponLevelData.name;
        weaponDescription.text = weaponLevelData.description;
        selectedCharacterWeapon.sprite = character.StartingWeapon.icon; 
    }
}