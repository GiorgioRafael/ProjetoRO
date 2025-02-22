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
    public float currentMight;
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

    void Awake()
    {
        currentHealth = characterData.MaxHealth;
        currentRecovery  = characterData.Recovery;
        currentMoveSpeed = characterData.MoveSpeed;
        currentMight = characterData.Might;
        currentProjectileSpeed = characterData.ProjectileSpeed;
        currentProjectileDuration = characterData.ProjectileDuration;
        currentMagnet = characterData.Magnet;
    }

    void Start()
    {
        //Inicializa o cap de xp como o primeiro aumento de cap de xp
        experienceCap = levelRanges[0].experienceCapIncrease;
    }
    void Update()
    {
        if(invincibilityTimer > 0)
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
        if(experience >= experienceCap)
        {
            Debug.Log("Player leveled up");
            level++;
            experience -= experienceCap;

            int experienceCapIncrease = 0;
            foreach(LevelRange range in levelRanges)
            {
                if(level >= range.startLevel && level <= range.endLevel)
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
        if(!isInvincible)
        {
            currentHealth -= dmg;

            invincibilityTimer = invincibilityDuration;
            isInvincible = true;
        if(currentHealth <= 0)
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
        if(currentHealth < characterData.MaxHealth)
        {
            currentHealth += amount;
            //se a cura passar do maximo de vida, fica com a vida cheia
            if(currentHealth > characterData.MaxHealth) 
            {
                currentHealth = characterData.MaxHealth;
            }
        }

    }
    void Recover()
    {
        if(currentHealth < characterData.MaxHealth)
        {
            currentHealth += currentRecovery * Time.deltaTime;
        }
        if(currentHealth > characterData.MaxHealth)
        {
            currentHealth = characterData.MaxHealth;
        }
    }
}
