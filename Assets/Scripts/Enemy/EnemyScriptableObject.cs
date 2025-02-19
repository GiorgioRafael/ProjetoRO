using UnityEngine;


[CreateAssetMenu(fileName="EnemyScriptableObject", menuName="ScriptableObjects/Enemy")]
public class EnemyScriptableObject : ScriptableObject
{
    //status base
    public float moveSpeed;
    public float maxHealh;
    public float damage;
}
