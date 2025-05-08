using UnityEngine;
using TMPro;

public class DataManager : MonoBehaviour, IDataPersistence
{
    //VALORES
    private int coinCount;
    public static DataManager instance { get; private set;}

    [Header("UI Elements")]
    public TMP_Text coinText; // Drag the TMP_Text field here in the Inspector

    private void Awake()
    {
        if(instance !=null && instance != this)
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
    }

    public void SaveData(ref GameData data)
    {
        data.coinCount = this.coinCount;
    }

    void Update()
    {
        coinText.text = coinCount.ToString();
    }

    public void AddCoin(int amount)
    {
        coinCount += amount;
        //DataPersistenceManager.instance.SaveGame();
        
        if (coinText != null)
        {
            coinText.text = coinCount.ToString();
        }
        else
        {
            Debug.LogWarning("CoinText is not assigned in the Inspector.");
        }
    }
}
