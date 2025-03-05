using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public CharacterScriptableObject characterData;

    //status atuais
    [HideInInspector]
    public float currentHealth;
    [HideInInspector]
    public float currentRecovery;
    [HideInInspector]
    public float currentMoveSpeed;
    [HideInInspector]
    public float currentMight; // basicamente dano
    [HideInInspector]
    public float currentProjectileSpeed;
    [HideInInspector]
    public float currentProjectileDuration;
    [HideInInspector]
    public float currentMagnet;

    //experiencia e level do jogador
    [Header("Experience/Level")]
    public int experience = 0;
    public int level = 1;
    public int experienceCap; //muda todo level


    //faz com que os campos sejam editaveis no unity e tambem pode ser salvo e editado em arquivos (data)
    [System.Serializable]
    public class LevelRange
    {
        public int startLevel;
        public int endLevel;
        public int experienceCapIncrease;
    }

    //iframes
    [Header("I-Frames")]
    public float invincibilityDuration;
    float invincibilityTimer;
    bool isInvincible;

    public List<LevelRange> levelRanges;

    InventoryManager inventory;
    public int weaponIndex;
    public int passiveItemIndex;

    public GameObject secondWeaponTest;
    public GameObject firstPassiveItemTest, secondPassiveItemTest;


    void Awake()
    {

        characterData = CharacterSelector.GetData();
        CharacterSelector.instance.DestroySingleton();

        inventory = GetComponent<InventoryManager>();

        currentHealth = characterData.MaxHealth;
        currentRecovery = characterData.Recovery;
        currentMoveSpeed = characterData.MoveSpeed;
        currentMight = characterData.Might;
        currentProjectileSpeed = characterData.ProjectileSpeed;
        currentProjectileDuration = characterData.ProjectileDuration;
        currentMagnet = characterData.Magnet;

        //spawna a arma inicial
        SpawnWeapon(characterData.StartingWeapon);
        SpawnWeapon(secondWeaponTest);
        SpawnPassiveItem(firstPassiveItemTest);
        SpawnPassiveItem(secondPassiveItemTest);

    }

    void Start()
    {
        //Inicializa o cap de xp como o primeiro aumento de cap de xp
        experienceCap = levelRanges[0].experienceCapIncrease;
    }
    void Update()
    {
        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
        }
        else if (isInvincible)
        {
            isInvincible = false;
        }
        Recover();
    }

    public void IncreaseExperience(int amount)
    {
        experience += amount;
        LevelUpChecker();
    }
    void LevelUpChecker()
    {
        if (experience >= experienceCap)
        {
            Debug.Log("Player leveled up");
            level++;
            experience -= experienceCap;

            int experienceCapIncrease = 0;
            foreach (LevelRange range in levelRanges)
            {
                if (level >= range.startLevel && level <= range.endLevel)
                {
                    experienceCapIncrease = range.experienceCapIncrease;
                    break;
                }
            }
            /*experienceCap += experienceCapIncrease; //xp fixo por niveis */
            experienceCap = experienceCap + experienceCapIncrease; // xp aumenta a cada nivel o nivel do cap
            Debug.Log("Valor de xp necessario para o nivel atual: " + experienceCap);
            /*
            
            */
            LevelUpChecker(); //verifica novamente me caso de ganhar muito xp de uma vez
        }
    }
    public void TakeDamage(float dmg)
    {
        if (!isInvincible)
        {
            currentHealth -= dmg;

            invincibilityTimer = invincibilityDuration;
            isInvincible = true;
            if (currentHealth <= 0)
            {
                Kill();
            }
        }

    }
    public void Kill()
    {
        Debug.Log("Player died");
    }
    public void RestoreHealth(float amount)
    {
        //somente cura se a vida nao estiver cheia
        if (currentHealth < characterData.MaxHealth)
        {
            currentHealth += amount;
            //se a cura passar do maximo de vida, fica com a vida cheia
            if (currentHealth > characterData.MaxHealth)
            {
                currentHealth = characterData.MaxHealth;
            }
        }

    }
    void Recover()
    {
        if (currentHealth < characterData.MaxHealth)
        {
            currentHealth += currentRecovery * Time.deltaTime;
        }
        if (currentHealth > characterData.MaxHealth)
        {
            currentHealth = characterData.MaxHealth;
        }
    }
    public void SpawnWeapon(GameObject weapon)
    {
        //verifica se os slots estao cheios e retorna   
        if (weaponIndex >= inventory.passiveItemLevels.Length - 1) //troquei count por lenght pq comecou a dar erro  (sla???)
        //verificar depois quando tiver mais armas
        {
            Debug.LogError("inventory slots already full");
            return;
        }
        //arma inicial
        GameObject spawnedWeapon = Instantiate(weapon, transform.position, Quaternion.identity);
        spawnedWeapon.transform.SetParent(transform); //faz a arma ser filho do player
        inventory.AddWeapon(weaponIndex, spawnedWeapon.GetComponent<WeaponController>()); //adiciona a arma para seu slot no inventario

        weaponIndex++;
    }

    public void SpawnPassiveItem(GameObject passiveWeapon)
    {
        //verifica se os slots estao cheios e retorna   
        if (passiveItemIndex >= inventory.passiveItemSlots.Count - 1)
        {
            Debug.LogError("inventory slots already full");
            return;
        }
        //arma inicial
        GameObject spawnedPassiveItem = Instantiate(passiveWeapon, transform.position, Quaternion.identity);
        spawnedPassiveItem.transform.SetParent(transform); //faz a arma ser filho do player
        inventory.addPassiveItem(passiveItemIndex, spawnedPassiveItem.GetComponent<PassiveItem>()); //adiciona a arma para seu slot no inventario

        passiveItemIndex++;
    }
}



