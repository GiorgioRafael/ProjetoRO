using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuraWeapon : Weapon
{
    protected Aura currentAura;
    protected bool isSpawning = false;
    private CharacterData.Stats actualStats;
    protected bool SantaWaterBeheaviour;
    
    private bool isInitialized = false;


    List<EnemyStats> allSelectedEnemies = new List<EnemyStats>();

    private void Init()
    {
        if (isInitialized) return;
        
        SantaWaterBeheaviour = currentStats.isSantaWater;
        Debug.Log(currentStats.isSantaWater);
        
        isInitialized = true;
    }
    /*
    void Start()
    {   
        Debug.Log(currentStats.isSantaWater);
        if(currentStats.isSantaWater == false)
        {
            SantaWaterBeheaviour = false ;
        }
    }
    */

    protected override void Update()
    {
        if (!isInitialized) return;
        if (SantaWaterBeheaviour && !isSpawning)
        {
            StartCoroutine(SpawnAurasCoroutine());
            //Debug.Log("Tentando iniciar Coroutine");
        }
    }

    public override void OnEquip()
    {
        Init();
        if (!SantaWaterBeheaviour)
        {
            // Garlic-style aura
            if (currentStats.auraPrefab)
            {
                if (currentAura) Destroy(currentAura);
                currentAura = Instantiate(currentStats.auraPrefab, transform);
                currentAura.weapon = this;
                currentAura.owner = owner;
                
                float area = GetArea();
                currentAura.transform.localScale = new Vector3(area, area, area);
            }
        }
    }

    public override void OnUnequip()
    {
        if (!SantaWaterBeheaviour)
        {
            if (currentAura) Destroy(currentAura);
        }
    }

    public override bool DoLevelUp()
    {
        if (!base.DoLevelUp()) return false;

        if (!SantaWaterBeheaviour)
        {
            if (currentAura)
            {
                float area = GetArea();
                currentAura.transform.localScale = new Vector3(area, area, area);
            }
        }

        return true;
    }

    private IEnumerator SpawnAurasCoroutine()
    {
        //Debug.Log("Coroutine iniciada");
        isSpawning = true;

        int count = currentStats.number + Owner.Stats.amount -1;
        float interval = currentStats.projectileInterval;

        // Refresh enemy list
        allSelectedEnemies = new List<EnemyStats>(FindObjectsOfType<EnemyStats>());

        for (int i = 0; i < count; i++)
        {
            EnemyStats target = PickEnemy();

            if (target)
            {
                SpawnAuraOnTarget(target);
            }

            if (i < count - 1)
                yield return new WaitForSeconds(interval);
        }

        yield return new WaitForSeconds(currentStats.cooldown * Owner.Stats.cooldown);
        isSpawning = false;
    }

    private void SpawnAuraOnTarget(EnemyStats target)
    {
        if (!currentStats.auraPrefab || !target) return;

        Vector2 spawnPosition = target.transform.position;

        Aura aura = Instantiate(currentStats.auraPrefab, spawnPosition, Quaternion.identity);
        aura.weapon = this;
        aura.owner = owner;


        float area = GetArea();
        aura.transform.localScale = new Vector3(area, area, area);
        Destroy(aura.gameObject, currentStats.lifespan * Owner.Stats.duration);
    }

    // Randomly picks a visible enemy on screen
    private EnemyStats PickEnemy()
    {
        EnemyStats target = null;

        while (!target && allSelectedEnemies.Count > 0)
        {
            int idx = Random.Range(0, allSelectedEnemies.Count);
            target = allSelectedEnemies[idx];

            if (!target)
            {
                allSelectedEnemies.RemoveAt(idx);
                continue;
            }

            Renderer r = target.GetComponent<Renderer>();
            if (!r || !r.isVisible)
            {
                allSelectedEnemies.Remove(target);
                target = null;
                continue;
            }
        }

        if (target) allSelectedEnemies.Remove(target);

        return target;
    }
}
