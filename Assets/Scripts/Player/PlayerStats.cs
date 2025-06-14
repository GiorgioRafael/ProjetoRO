using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : EntityStats
{
    
    private int remainingRevivals; // Add this field
    CharacterData characterData;
    public CharacterData.Stats baseStats;
    [SerializeField] CharacterData.Stats actualStats;

    public CharacterData.Stats Stats
    {
        get { return actualStats;  }
        set { 
            actualStats = value;
        }
    }

    public CharacterData.Stats Actual
    {
        get { return actualStats; }
    }


    #region Current Stats Properties
    public float CurrentHealth
    {

        get { return health; }

        // If we try and set the current health, the UI interface
        // on the pause screen will also be updated.
        set
        {
            //Check if the value has changed

            if (health != value)
            {
                health = value;
                UpdateHealthBar();
            }
        }
    }


    #endregion

    [Header("Visuals")]
    public ParticleSystem damageEffect;
    public ParticleSystem blockedEffect; //se a armadura bloquear completamente o dano

    //Experience and level of the player
    [Header("Experience/Level")]
    public int experience = 0;
    public int level = 1;
    public int experienceCap;

    //Class for defining a level range and the corresponding experience cap increase for that range
    [System.Serializable]
    public class LevelRange
    {
        public int startLevel;
        public int endLevel;
        public int experienceCapIncrease;
    }

    //I-Frames
    [Header("I-Frames")]
    public float invincibilityDuration;
    float invincibilityTimer;
    bool isInvincible;

    public List<LevelRange> levelRanges;


    PlayerInventory inventory;
    PlayerCollector collector;

    [Header("UI")]
    public Image healthBar;
    public Image expBar;
    public TMP_Text levelText;

    PlayerAnimator playerAnimator;

    void Awake()
    {
        characterData = UICharacterSelector.GetData();

        //if(CharacterSelector.instance) 
        //    CharacterSelector.instance.DestroySingleton();

        inventory = GetComponent<PlayerInventory>();
        collector = GetComponentInChildren<PlayerCollector>();

        //Assign the variables
        baseStats = actualStats = characterData.stats;
        collector.SetRadius(actualStats.magnet);
        health = actualStats.maxHealth;

        playerAnimator = GetComponent<PlayerAnimator>();
        playerAnimator.SetAnimatorController(characterData.animator);
        remainingRevivals = actualStats.revival; // Initialize remaining revivals
        Debug.Log("Remaining revivals: " + remainingRevivals);

    }

    protected override void Start()
    {
        base.Start();

        if (UILevelSelector.globalBuff && !UILevelSelector.globalBuffAffectsPlayer)
            ApplyBuff(UILevelSelector.globalBuff);

        RecalculateStats();

        //Spawn the starting weapon
        inventory.Add(characterData.StartingWeapon);

        //Initialize the experience cap as the first experience cap increase
        experienceCap = levelRanges[0].experienceCapIncrease;

        GameManager.instance.AssignChosenCharacterUI(characterData);

        UpdateHealthBar();
        UpdateExpBar();
        UpdateLevelText();
    }

    protected override void Update()
    {
        base.Update();
        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
        }
        //If the invincibility timer has reached 0, set the invincibility flag to false
        else if (isInvincible)
        {
            isInvincible = false;
        }

        Recover();
    }

public override void RecalculateStats()
{
    // Reset stats to base values
    actualStats = baseStats;

    // Apply passive item boosts
    foreach (PlayerInventory.Slot s in inventory.passiveSlots)
    {
        Passive p = s.item as Passive;
        if (p)
        {
            actualStats += p.GetBoosts();
        }
    }

    // Create multiplier stats initialized to 1
    CharacterData.Stats multiplier = new CharacterData.Stats
    {
        maxHealth = 1f, recovery = 1f, armor = 1f, moveSpeed = 1f, might = 1f,
        area = 1f, speed = 1f, duration = 1f, amount = 1, cooldown = 1f,
        luck = 1f, growth = 1f, greed = 1f, curse = 1f, magnet = 1f, revival = 1
    };

    // Apply buffs
    foreach(Buff b in activeBuffs)
    {
        BuffData.Stats bd = b.GetData();
        switch(bd.modifierType)
        {
            case BuffData.ModifierType.additive:
                actualStats += bd.playerModifier;
                break;
            case BuffData.ModifierType.multiplicative:
                multiplier *= bd.playerModifier;
                break;
        }
    }

    // Apply permanent upgrades from UpgradeManager
    if (UpgradeManager.instance != null)
    {
        foreach (var upgrade in UpgradeManager.instance.upgrades)
        {
            // Get the current level for this upgrade
            int currentLevel = UpgradeManager.instance.GetUpgradeLevel(upgrade.upgradeName);
            
            // Skip if level is 0
            if (currentLevel <= 0) continue;

            foreach (var boost in upgrade.boosts)
            {
                switch (boost.statName)
                {
                    case "maxhealth":
                        actualStats.maxHealth += upgrade.GetBoost("maxhealth", currentLevel);
                        break;
                    case "recovery":
                        actualStats.recovery += upgrade.GetBoost("recovery", currentLevel);
                        break;
                    case "armor":
                        actualStats.armor += upgrade.GetBoost("armor", currentLevel);
                        break;
                    case "movespeed":
                        actualStats.moveSpeed += upgrade.GetBoost("movespeed", currentLevel);
                        break;
                    case "might":
                        actualStats.might += upgrade.GetBoost("might", currentLevel);
                        break;
                    case "area":
                        actualStats.area += upgrade.GetBoost("area", currentLevel);
                        break;
                    case "speed":
                        actualStats.speed += upgrade.GetBoost("speed", currentLevel);
                        break;
                    case "duration":
                        actualStats.duration += upgrade.GetBoost("duration", currentLevel);
                        break;
                    case "amount":
                        actualStats.amount += (int)upgrade.GetBoost("amount", currentLevel);
                        break;
                    case "cooldown":
                        actualStats.cooldown += upgrade.GetBoost("cooldown", currentLevel);
                        break;
                    case "luck":
                        actualStats.luck += upgrade.GetBoost("luck", currentLevel);
                        break;
                    case "growth":
                        actualStats.growth += upgrade.GetBoost("growth", currentLevel);
                        break;
                    case "greed":
                        actualStats.greed += upgrade.GetBoost("greed", currentLevel);
                        break;
                    case "curse":
                        actualStats.curse += upgrade.GetBoost("curse", currentLevel);
                        break;
                    case "magnet":
                        actualStats.magnet += upgrade.GetBoost("magnet", currentLevel);
                        break;
                    case "revival":
                        actualStats.revival += (int)upgrade.GetBoost("revival", currentLevel);
                        break;
                }
            }
        }
    }
    
    // Apply multipliers
    actualStats *= multiplier;
    
    // Update collector radius
    collector.SetRadius(actualStats.magnet);
}

    public void IncreaseExperience(int amount)
    {
        float growthModifier = actualStats.growth;
        // Subtract 1 since growth of 1 means 0% bonus
        float finalExp = amount * growthModifier;
        
        experience += (int)finalExp;
        
        LevelUpChecker();
        UpdateExpBar();
    }

    void LevelUpChecker()
    {
        if (experience >= experienceCap)
        {
            //Level up the player and reduce their experience by the experience cap
            level++;
            experience -= experienceCap;

            //Find the experience cap increase for the current level range
            int experienceCapIncrease = 0;
            foreach (LevelRange range in levelRanges)
            {
                if (level >= range.startLevel && level <= range.endLevel)
                {
                    experienceCapIncrease = range.experienceCapIncrease;
                    break;
                }
            }
            experienceCap += experienceCapIncrease;

            UpdateLevelText();
            UpdateExpBar();

            GameManager.instance.StartLevelUp();

            if(experience >= experienceCap) LevelUpChecker();
        }
    }

    void UpdateExpBar()
    {
        // Update exp bar fill amount
        expBar.fillAmount = (float)experience / experienceCap;
    }

    void UpdateLevelText()
    {
        // Update level text
        levelText.text = "LV " + level.ToString();
    }

        public override void TakeDamage(float dmg)
    {
        //If the player is not currently invincible, reduce health and start invincibility
        if (!isInvincible)
        {
            // Take armor into account before dealing the damage.
            dmg -= actualStats.armor;

            if (dmg > 0)
            {
                // Deal the damage.
                CurrentHealth -= dmg;

                // If there is a damage effect assigned, play it.
                if (damageEffect) Destroy(Instantiate(damageEffect, transform.position, Quaternion.identity), 5f);

                if (CurrentHealth <= 0)
                {
                    Kill();
                }
            }
            else
            {
                // If there is a blocked effect assigned, play it.
                if (blockedEffect) Destroy(Instantiate(blockedEffect, transform.position, Quaternion.identity), 5f);
            }

            invincibilityTimer = invincibilityDuration;
            isInvincible = true;
        }
    }

    void UpdateHealthBar()
    {
        //Update the health bar
        healthBar.fillAmount = CurrentHealth / actualStats.maxHealth;
    }

    public override void Kill()
    {
        if (actualStats.revival > 0)
        {
            Debug.Log("REVIVED");
            remainingRevivals--;
            CurrentHealth = actualStats.maxHealth;
            return;
        }

        if (!GameManager.instance.isGameOver && actualStats.revival == 0)
        {
            AudioController.instance.StopBackgroundMusic(1f);
            GameManager.instance.AssignLevelReachedUI(level);
            GameManager.instance.GameOver();
        }
    }

    public override void RestoreHealth(float amount)
    {
        // Only heal the player if their current health is less than their maximum health
        if (CurrentHealth < actualStats.maxHealth)
        {
            CurrentHealth += amount;

            // Make sure the player's health doesn't exceed their maximum health
            if (CurrentHealth > actualStats.maxHealth)
            {
                CurrentHealth = actualStats.maxHealth;
            }

            UpdateHealthBar();
        }
    }

    void Recover()
    {
        if (CurrentHealth < actualStats.maxHealth)
        {

            CurrentHealth += Stats.recovery * Time.deltaTime;

            // Make sure the player's health doesn't exceed their maximum health
            if (CurrentHealth > actualStats.maxHealth)
            {
                CurrentHealth = actualStats.maxHealth;
            }

            UpdateHealthBar();
        }
    }


}