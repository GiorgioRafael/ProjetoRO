using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DataManager : MonoBehaviour, IDataPersistence
{
    //VALORES
    private int coinCount;
    public int CoinCount => coinCount; // Public getter for coins
    private List<GameData.CharacterInfo> characters;
    public static DataManager instance { get; private set; }

    [Header("UI Elements")]
    public TMP_Text coinText; // Drag the TMP_Text field here in the Inspector

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadData(GameData data)
    {
        this.coinCount = data.coinCount;
        this.characters = data.characters ?? new List<GameData.CharacterInfo>();

    }

    public void SaveData(ref GameData data)
    {
        data.coinCount = this.coinCount;
        data.characters = this.characters;
    }

    void Update()
    {
        coinText.text = coinCount.ToString();
    }

    public void AddCoin(int amount)
    {
        coinCount += amount;
        DataPersistenceManager.instance.SaveGame();

        if (coinText != null)
        {
            coinText.text = coinCount.ToString();
        }
        else
        {
            Debug.LogWarning("CoinText is not assigned in the Inspector.");
        }
    }

    public bool IsCharacterUnlocked(string characterFullName)
    {
        return characters.Exists(c => c.characterFullName == characterFullName && c.isUnlocked);
    }

    public void UnlockCharacter(string characterFullName)
    {

        int index = characters.FindIndex(c => c.characterFullName == characterFullName);
        if (index >= 0)
        {
            var charInfo = characters[index];
            charInfo.isUnlocked = true;
            characters[index] = charInfo;
        }
        else
        {
            characters.Add(new GameData.CharacterInfo(characterFullName, true));
        }
        DataPersistenceManager.instance.SaveGame();
    }

}
