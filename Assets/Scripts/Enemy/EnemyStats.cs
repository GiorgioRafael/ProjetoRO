using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public EnemyScriptableObject enemyData;
    
    //current stats
    float currentMoveSpeed;
    float currentHealth;
    float currentDamage;

    void Awake()
    {
        currentMoveSpeed = enemyData.MoveSpeed;
        currentHealth = enemyData.MaxHealh;
        currentDamage = enemyData.Damage;
    }
    public void TakeDamage(float dmg)
    {
        currentDamage -= dmg;
        if(currentHealth <= 0)
        {
            Kill()
        }
    }
    public void Kill()
    {
        Destroy(gameObject);
    }
}
