using UnityEngine;

public class TreasureChests : MonoBehaviour
{
    InventoryManager inventory;

    public void Start()
    {
        inventory = FindObjectOfType<InventoryManager>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            OpenTreasureChest();
            Destroy(gameObject);
        }
    }

    public void OpenTreasureChest()
    {
        Debug.Log("Chest opened");
    }

}
