using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

/// <summary>
/// Componente que deve ser ligado a todas as prefabs das armas. O prefab funciona juntamente com o WeaponData
/// ScriptableObjects para manusear e rodar os behaviours de todas as armas do jogo
/// </summary>
public abstract class Weapon : MonoBehaviour
{
    [System.Serializable]
    public struct Stats
    {
        public string name, description;

        [Header("Visuals")]
        public ParticleSystem hitEffect;
        public Rect spawnVariance;

        [Header("Values")]
        public float lifespan; //se for 0 dura pra sempre
        public float damage, damageVariance, area, speed, cooldown, projectileInterval, knockback;
        public int number, piercing, maxInstances;

        public static Stats operator +(Stats s1, Stats s2)
        {
            Stats result = new Stats();
            result.name = s2.name ?? s1.name;
            result.description = s2.description ?? s1.description;
            //result.projetilePrefab = s2.projectilePrefab ?? s1.projectilePrefab;
            //result.auraPrefab = s2.auraPrefab ?? s1.auraPrefab;;
            result.hitEffect = s2.hitEffect == null ? s1.hitEffect : s2.hitEffect;
            result.spawnVariance = s2.spawnVariance;
            result.lifespan = s1.lifespan + s2.lifespan;
            result.damage = s1.damage + s2.damage;
            result.damageVariance = s1.damageVariance + s2.damageVariance;
            result.area = s1.area + s2.area;
            result.speed = s1.speed + s2.speed;
            result.cooldown = s1.cooldown + s2.cooldown;
            result.number = s1.number + s2.number;
            result.piercing = s1.piercing + s2.piercing;
            result.projectileInterval = s1.projectileInterval + s2.projectileInterval;
            result.knockback = s1.knockback + s2.knockback;
            return result;
        }

        public float GetDamage()
        {
            return damage + Random.Range(0, damageVariance);
        }
    }

    public int currentLevel = 1, maxLevel = 1;
    
    protected PlayerStats owner;

    protected Stats currentStats;

    public WeaponData data;

    protected float currentCooldown;

    protected PlayerMovement movement; //referencia para o player movement

    //para criacao de armas dinamicas, chama a funcao initialise para setar tudo certinho.
    public virtual void initialise(WeaponData data)
    {
        maxLevel = data.maxLevel;
        owner = FindObjectOfType<PlayerStats>();

        this.data = data;
        currentStats = data.baseStats;
        movement = GetComponentInParent<PlayerMovement>();
        currentCooldown = currentStats.cooldown;
    }
    protected virtual void Awake()
    {
        if (data) currentStats = data.baseStats;
    }

    protected virtual void Start()
    {
        if(data)
        {
            initialise(data);
        }
    }
    protected virtual void Update()
    {
        currentCooldown -= Time.deltaTime;
        if(currentCooldown <= 0f) //quando o cooldown der 0, ataca
        {
            Attack(currentStats.number);
        }
    }

    public virtual bool CanLevelup()
    {
        return currentLevel <=maxLevel;
    }

    //Aumenta o level da arma em 1, e calcula os status correspondentes
    public virtual bool DoLevelUp()
    {
        //evita aumentar o level se ja estiver no nivel maximo
        if(!CanLevelup())
        {
            Debug.LogWarning(string.Format("Cannot level up {0} to level {1}, max level of {2} already reached.", name, currentLevel, data.maxLevel));
            return false;
        }

        //senao, adiciona os status do proximo level da arma
        currentStats += data.GetLevelData(++currentLevel);
        return true;
    }

    //verifica se a arma pode atacar nesse momento
    public virtual bool CanAttack()
    {
        return currentCooldown <= 0;
    }

    //performa um ataque com a arma
    //retorna true se o ataque for bem sucedido
    //This doesnt do anything. we have to override this at the child class to add a behaviour.

    protected virtual bool Attack(int attackCount = 1)
    {
        if(CanAttack())
        {
            currentCooldown += currentStats.cooldown;
            return true;
        }
        return false;
    }

    //gets the amount of damage that the weapon is supposed to deal.
    //incluindo a variacao de dano e o might do personagem

    public virtual float GetDamage()
    {
        return currentStats.GetDamage() * owner.CurrentMight;
    }

    //para pegar os stats da arma
    public virtual Stats GetStats() {return currentStats;}
    
}
