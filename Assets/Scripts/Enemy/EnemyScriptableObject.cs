using UnityEngine;


[CreateAssetMenu(fileName="EnemyScriptableObject", menuName="ScriptableObjects/Enemy")]
public class EnemyScriptableObject : ScriptableObject
{
    //status base
    [SerializeField]
    float moveSpeed;
    public float MoveSpeed { get => moveSpeed; private set => moveSpeed = value; }

    [SerializeField]
    float maxHealh;
    public float MaxHealh { get => maxHealh; private set => maxHealh = value; }

    [SerializeField]
    float damage;
    public float Damage { get => damage; private set => damage = value; }
}
