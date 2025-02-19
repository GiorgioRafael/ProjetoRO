using UnityEngine;

[CreateAssetMenu(fileName="WeaponScriptableObject", menuName="ScriptableObjects/Weapon")]
public class WeaponScriptableObject : ScriptableObject
{
    public GameObject prefab;
    //status base para as armas
    public float damage;
    public float speed;
    public float cooldownDuration;
    public int pierce;
}
