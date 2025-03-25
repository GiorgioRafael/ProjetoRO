using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Data", menuName ="ProjectRO/WeaponData")]
public class WeaponData : ScriptableObject
{
    public Sprite icon;
    public int maxLevel;

    [HideInInspector] public string behaviour;
    public Weapon.Stats baseStats;
    public Weapon.Stats[] linearGrowth;
    public Weapon.Stats[] randomGrowth;

    //gives us the stat growth / description of the next level.
    public Weapon.Stats GetLevelData(int level)
    {
        //pega os status do proximo level
        if(level - 2 < linearGrowth.Length)
            return linearGrowth[level -2];

        if(randomGrowth.Length > 0)
            return randomGrowth[Random.Range(0, randomGrowth.Length)];


        //retorna um valor vazio e um warning
        Debug.LogWarning(string.Format("Weapon doesnt gave its level up stats configured for level {0}!"));
        return new Weapon.Stats();    
    }
}
