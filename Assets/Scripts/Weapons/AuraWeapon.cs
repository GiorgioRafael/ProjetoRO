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

    protected override void Update()
    {
        if (!isInitialized) 
         Init(); 
        
        if(!isInitialized) return;
        
        if (SantaWaterBeheaviour && !isSpawning)
        {
            StartCoroutine(SpawnAurasCoroutine());
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
        isSpawning = true;

        int count = currentStats.number + Owner.Stats.amount;
        float interval = currentStats.projectileInterval;

        allSelectedEnemies = new List<EnemyStats>(FindObjectsOfType<EnemyStats>());

        for (int i = 0; i < count; i++)
        {
            EnemyStats target = PickClosestFreeEnemy();

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

    private EnemyStats PickClosestFreeEnemy()
    {
        Vector2 playerPosition = owner.transform.position;
        float minDistanceBetweenAuras = 1.5f; // Ajuste conforme o tamanho da aura

        List<EnemyStats> sortedEnemies = new List<EnemyStats>(allSelectedEnemies);
        sortedEnemies.Sort((a, b) =>
            Vector2.Distance(a.transform.position, playerPosition)
            .CompareTo(Vector2.Distance(b.transform.position, playerPosition))
        );

        foreach (var enemy in sortedEnemies)
        {
            if (!enemy) continue;

            Renderer r = enemy.GetComponent<Renderer>();
            if (!r || !r.isVisible) continue;

            Vector2 pos = enemy.transform.position;
            if (!IsAuraNear(pos, minDistanceBetweenAuras))
            {
                allSelectedEnemies.Remove(enemy);
                return enemy;
            }
        }

        return null;
    }

    private bool IsAuraNear(Vector2 position, float minDistance)
    {
        Aura[] existingAuras = FindObjectsOfType<Aura>();
        foreach (var aura in existingAuras)
        {
            if (Vector2.Distance(aura.transform.position, position) < minDistance)
            {
                return true;
            }
        }
        return false;
    }
}
