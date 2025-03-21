using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public CharacterScriptableObject characterData;

    //status atuais
    float currentHealth;
    float currentRecovery;
    float currentMoveSpeed;
    float currentMight; // basicamente dano
    float currentProjectileSpeed;
    float currentProjectileDuration;
    float currentMagnet;

    #region Current Stats Properties
    public float CurrentHealth
    {
        get { return currentHealth; }
        set 
        {
            if(currentHealth != value)
            {
                currentHealth = value;
                if(GameManager.instance != null)
                {
                    GameManager.instance.currentHealthDisplay.text = "Health: " + currentHealth;
                }
                //da update no valor real do status

            }
        }
    }

    public float CurrentRecovery
    {
        get { return currentRecovery; }
        set 
        {
            if(currentRecovery != value)
            {
                currentRecovery = value;
                if(GameManager.instance != null)
                {
                    GameManager.instance.currentRecoveryDisplay.text = "Recovery: " + currentRecovery;
                }
                //da update no valor real do status

            }
        }
    }

    public float CurrentMoveSpeed
    {
        get { return currentMoveSpeed; }
        set 
        {
            if(currentMoveSpeed != value)
            {
                currentMoveSpeed = value;
                if(GameManager.instance != null)
                {
                    GameManager.instance.currentMoveSpeedDisplay.text = "Move Speed: " + currentMoveSpeed;
                }
                //da update no valor real do status

            }
        }
    }

    public float CurrentMight
    {
        get { return currentMight; }
        set 
        {
            if(currentMight != value)
            {
                currentMight = value;
                if(GameManager.instance != null)
                {
                    GameManager.instance.currentMightDisplay.text = "Might: " + currentMight;
                }
                //da update no valor real do status

            }
        }
    }

    public float CurrentProjectileSpeed
    {
        get { return currentProjectileSpeed; }
        set 
        {
            if(currentProjectileSpeed!= value)
            {
                currentProjectileSpeed= value;
                if(GameManager.instance != null)
                {
                    GameManager.instance.currentProjectileSpeedDisplay.text = "Projectile Speed: " + currentProjectileSpeed;
                }
                //da update no valor real do status

            }
        }
    }

    public float CurrentProjectileDuration
    {
        get { return currentProjectileDuration; }
        set 
        {
            if(currentProjectileDuration != value)
            {
                currentProjectileDuration = value;
                //da update no valor real do status

            }
        }
    }

    public float CurrentMagnet
    {
        get { return currentMagnet; }
        set 
        {
            if(currentMagnet != value)
            {
                currentMagnet = value;
                if(GameManager.instance != null)
                {
                    GameManager.instance.currentMagnetDisplay.text = "Current Might: " + currentMight;
                }
                //da update no valor real do status

            }
        }
    }
    #endregion

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

        CurrentHealth = characterData.MaxHealth;
        CurrentRecovery = characterData.Recovery;
        CurrentMoveSpeed = characterData.MoveSpeed;
        CurrentMight = characterData.Might;
        CurrentProjectileSpeed = characterData.ProjectileSpeed;
        CurrentProjectileDuration = characterData.ProjectileDuration;
        CurrentMagnet = characterData.Magnet;

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

        GameManager.instance.currentHealthDisplay.text = "Health: " + currentHealth;
        GameManager.instance.currentRecoveryDisplay.text = "Recovery: " + currentRecovery;
        GameManager.instance.currentMoveSpeedDisplay.text = "Move Speed: " + currentMoveSpeed;
        GameManager.instance.currentMightDisplay.text = "Might: " + currentMight;
        GameManager.instance.currentProjectileSpeedDisplay.text = "Projectile Speed: " + currentProjectileSpeed;
        GameManager.instance.currentMagnetDisplay.text = "Magnet: " + currentMagnet;

        GameManager.instance.AssignChosenCharacterUI(characterData);
    }

    void Update()
    {

    if (isInvincible)
    {
        invincibilityTimer -= Time.deltaTime;
    }
    if(isInvincible)
    {
        if (invincibilityTimer <= 0)
        {
            isInvincible = false;
            Debug.Log("Iframe off");
        }
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
            //Debug.Log("Player leveled up");
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
            //Debug.Log("Valor de xp necessario para o nivel atual: " + experienceCap);
            /*
            
            
            */
            GameManager.instance.StartLevelUp();
            LevelUpChecker(); //verifica novamente me caso de ganhar muito xp de uma vez
        }
    }
    public void TakeDamage(float dmg)
    {
        if (!isInvincible)
        {
            CurrentHealth -= dmg;

            invincibilityTimer = invincibilityDuration;
            isInvincible = true;
            Debug.Log("Iframe on");

            if (CurrentHealth <= 0)
            {
                Kill();
            }
        }

    }
    public void Kill()
    {
        if(!GameManager.instance.isGameOver)
        {
            GameManager.instance.AssignLevelReachedUI(level);
            GameManager.instance.GameOver();
            GameManager.instance.AssignChosenWeaponsAndPassiveItemsUI(inventory.weaponUISlots, inventory.passiveItemUISlots);
        }
    }
    public void RestoreHealth(float amount)
    {
        //somente cura se a vida nao estiver cheia
        if (CurrentHealth < characterData.MaxHealth)
        {
            CurrentHealth += amount;
            //se a cura passar do maximo de vida, fica com a vida cheia
            if (CurrentHealth > characterData.MaxHealth)
            {
                CurrentHealth = characterData.MaxHealth;
            }
        }

    }
    void Recover()
    {
        if (CurrentHealth < characterData.MaxHealth)
        {
            CurrentHealth += CurrentRecovery * Time.deltaTime;
        }
        if (CurrentHealth > characterData.MaxHealth)
        {
            CurrentHealth = characterData.MaxHealth;
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



