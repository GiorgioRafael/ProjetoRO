using UnityEngine;

public class HealthPotion : Pickup, ICollectable
{
    public int healthToRestore;
    public void Collect()
    {
        PlayerStats player = FindFirstObjectByType<PlayerStats>();
        player.RestoreHealth(healthToRestore);
    }
}
