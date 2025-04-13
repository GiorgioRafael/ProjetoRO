using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingBibleProjectile : Projectile
{
    Dictionary<EnemyStats, float> affectedTargets = new Dictionary<EnemyStats, float>();
    List<EnemyStats> targetsToUnaffect = new List<EnemyStats>();
    public float hitDelay = 1.7f;

    //We multiply this with the base speed, as the default values by themselves rotate too slowly.
    public float speedMultiplier = 5f; 

    // We need these multipliers for area since we scale the radius and projectile size differently.
    // If we don't, we will have really big books as the radius increases which is not the case in-game.
    public float radiusMultiplier = 1.1f; 
    public float projectileSizeMultiplier = 0.9f;

    //How much time does it take for the projectile to grow/shrink to its needed size
    public float transitionTime = 0.5f;

    private float currentLifespan;

    Vector3 startScale;
    float startLifespan;
    bool isAlive = false;
    private float angle;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        startScale = new Vector3(area, area,1);
        startLifespan = weapon.GetLifespan();
        transform.localScale = Vector3.zero;
        StartCoroutine(BibleGrow());
        Vector3 offset = transform.position - owner.transform.position; //Get the initial spawn position
        angle = Mathf.Atan2(offset.y, offset.x); // Get the angle in radians
    }

    protected override void FixedUpdate()
    {
        if (rb && rb.bodyType == RigidbodyType2D.Kinematic)
        {
            float x = owner.transform.position.x + Mathf.Cos(angle) * startScale.x * radiusMultiplier;
            float y = owner.transform.position.y + Mathf.Sin(angle) * startScale.y * radiusMultiplier;
            rb.MovePosition(new Vector3(x, y, 0));
            angle -= weapon.GetStats().speed * speedMultiplier * Time.fixedDeltaTime;
        }
    }
    private void Update()
    {
        HitDelay();

        currentLifespan += Time.deltaTime;

        if (!rb)
        {
            float x = owner.transform.position.x + Mathf.Cos(angle) * startScale.x * radiusMultiplier;
            float y = owner.transform.position.y + Mathf.Sin(angle) * startScale.y * radiusMultiplier;
            transform.position = new Vector3(x, y, 0);
            angle -= weapon.GetStats().speed * speedMultiplier * Time.deltaTime;
        }

        if (currentLifespan > startLifespan-transitionTime && isAlive)
        {
            StartCoroutine(BibleShrink());
            isAlive = false;
        }
        if (!weapon && isAlive)
        {
            StartCoroutine(BibleShrink());
            isAlive = false;
        }
    }
    public void HitDelay()
    {
        Dictionary<EnemyStats, float> affectedTargsCopy = new Dictionary<EnemyStats, float>(affectedTargets);

        // Loop through every target that has been hit by this projectile, and reduce the cooldown
        // of the projectile for it. If the cooldown reaches 0, deal damage to it.
        foreach (KeyValuePair<EnemyStats, float> pair in affectedTargsCopy)
        {
            if (pair.Key)
            {
                Vector3 source = damageSource == DamageSource.owner && owner ? owner.transform.position : transform.position;
                affectedTargets[pair.Key] -= Time.deltaTime;
                if (pair.Value <= 0)
                {
                    if (targetsToUnaffect.Contains(pair.Key))
                    {
                        // If the target is marked for removal, remove it.
                        affectedTargets.Remove(pair.Key);
                        targetsToUnaffect.Remove(pair.Key);
                    }
                    else
                    {
                        // Reset the cooldown and deal damage.
                        Weapon.Stats stats = weapon.GetStats();
                        affectedTargets[pair.Key] = hitDelay;
                        pair.Key.TakeDamage(GetDamage(), source, stats.knockback);

                        //assign later i guess
                        //weapon.ApplyBuffs(pair.Key); // Apply all assigned buffs to the target.

                        // Play the hit effect if it is assigned.
                        if (stats.hitEffect)
                        {
                            Destroy(Instantiate(stats.hitEffect, pair.Key.transform.position, Quaternion.identity).gameObject, 5f);
                        }
                        piercing--;
                    }
                }
            }

        }
    }

    public IEnumerator BibleShrink()
    {
        Vector3 currentScale = transform.localScale;

        // Waits for a single frame.
        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0;

        // This is a loop that fires every frame.
        while (t < transitionTime)
        {
            yield return w;
            t += Time.deltaTime;

           // Reduce the current size to 0 within transitionTime
           transform.localScale = new Vector3(currentScale.x - (t/ transitionTime), currentScale.y - (t / transitionTime), 1f);
        }
        if (!weapon) Destroy(gameObject);
    }
    public IEnumerator BibleGrow()
    {
        if (!isAlive)
        {
            isAlive = true;
            // Waits for a single frame.
            WaitForEndOfFrame w = new WaitForEndOfFrame();
            float t = 0;

            // This is a loop that fires every frame.
            while (t < transitionTime)
            {
                yield return w;
                t += Time.deltaTime;

                // Grow the size from 0 to the scale from the weapon's area multiplied by the projectileSizeMultiplier, within the transitionTime.
                transform.localScale = new Vector3(0 + (t/ transitionTime * startScale.x) * projectileSizeMultiplier, 0 + (t/ transitionTime * startScale.y) * projectileSizeMultiplier, 1f);
            }

        }


        //Destroy(gameObject);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out EnemyStats es))
        {
            // If the target is not yet affected by this bible, add it
            // to our list of affected targets.
            if (!affectedTargets.ContainsKey(es))
            {
                // Always starts with an interval of 0, so that it will get
                // damaged in the next Update() tick.
                affectedTargets.Add(es, 0);
            }
        }
        else if (other.TryGetComponent(out BreakableProps p))
        {
            p.TakeDamage(GetDamage());
            piercing--;

            Weapon.Stats stats = weapon.GetStats();
            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity).gameObject, 5f);
            }
        }
        // Destroy this object if it has run out of health from hitting other stuff.
        if (piercing <= 0) Destroy(gameObject);
    }
}