using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Obsolete("This will be replaced by the WeaponData class.")]
[CreateAssetMenu(fileName="WeaponScriptableObject", menuName="ScriptableObjects/Weapon")]
public class WeaponScriptableObject : ScriptableObject
{
    [SerializeField]
    GameObject prefab;
    public GameObject Prefab { get => prefab; private set => prefab = value; }
    //status base para as armas

    [SerializeField]
    float damage;
    public float Damage { get => damage; private set => damage = value; }

    [SerializeField]
    float speed;
    public float Speed { get => speed; private set => speed = value; }

    [SerializeField]
    float cooldownDuration;
    public float CooldownDuration { get => cooldownDuration; private set => cooldownDuration = value; }

    [SerializeField]
    int pierce;
    public int Pierce { get => pierce; private set => pierce = value; }

    [SerializeField]
    float destroyAfterSeconds;
    public float DestroyAfterSeconds { get => destroyAfterSeconds; private set => destroyAfterSeconds = value; }

    [SerializeField]
    int level; //nao e pra ser alterado no jogo [so no editor]
    public int Level { get => level; private set => level = value; }
    
    [SerializeField]
    GameObject nextLevelPrefab; // prefab do proximo nivel (ou seja: qual objeto vira quando upa de nivel)
    public GameObject NextLevelPrefab { get => nextLevelPrefab; private set => nextLevelPrefab = value; }

    [SerializeField]
    new string name;
    public string Name { get => name; private set => name = value; }
    
    [SerializeField]
    string description; //descricao da arma, se for um upgrade coloca a descricao do upgrade
    public string Description { get => description; private set => description = value; }
    
    [SerializeField]
    Sprite icon; //nao é pra ser modificado
    public Sprite Icon { get => icon; private set => icon = value; }

    [SerializeField]
    int evolvedUpgradeToRemove; //nao é pra ser modificado
    public int EvolvedUpgradeToRemove{ get => evolvedUpgradeToRemove; private set => evolvedUpgradeToRemove = value; }
}
