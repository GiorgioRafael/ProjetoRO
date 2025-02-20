using UnityEngine;


[CreateAssetMenu(fileName="EnemyScriptableObject", menuName="ScriptableObjects/Enemy")]
public class EnemyScriptableObject : ScriptableObject
{
    //status base
    [SerializeField]
    float moveSpeed;
    public float MoveSpeed { get => moveSpeed; private set => moveSpeed = value; }

    [SerializeField]
    float maxHealth;
    public float MaxHealth { get => maxHealth; private set => maxHealth = value; }

    [SerializeField]
    float damage;
    public float Damage { get => damage; private set => damage = value; }
}
