using UnityEngine;

public class BreakableProps : MonoBehaviour
{
    public float health;
    public float damage = 0;

    // Keep existing TakeDamage and Kill methods
    public void TakeDamage(float dmg)
    {
        health -= dmg;
        if(health <= 0)
        {
            Kill();
        }
    }

    public void Kill()
    {
        Destroy(gameObject);
    }

    // Add collision damage like enemies
    void OnCollisionStay2D(Collision2D col)
    {
        if(Mathf.Approximately(damage, 0)) return;  // Skip if damage is 0

        if(col.collider.TryGetComponent(out PlayerStats player))
        {
            player.TakeDamage(damage);
        }
    }
}