using UnityEngine;

public class HealthPotion : MonoBehaviour, ICollectable
{
    public int healthToRestore;
    public void Collect()
    {
        PlayerStats player = FindFirstObjectByType<PlayerStats>();
        player.RestoreHealth(healthToRestore);
        Destroy(gameObject);
    }
}
