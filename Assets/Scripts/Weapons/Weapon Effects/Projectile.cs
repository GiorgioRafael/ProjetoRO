using UnityEngine;

/// <summary>
/// Component that you attach to all projectile prefabs. All spawned projectiles will fly in the direction
/// they are facing and deal damage when they hit an object.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : WeaponEffect
{

    public enum DamageSource { projectile, owner };
    public DamageSource damageSource = DamageSource.projectile;
    public bool hasAutoAim = false;
    public Vector3 rotationSpeed = new Vector3(0, 0, 0);

    protected Rigidbody2D rb;
    protected int piercing;
    protected float area;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Weapon.Stats stats = weapon.GetStats();
        if (rb.bodyType == RigidbodyType2D.Dynamic)
        {
            rb.angularVelocity = rotationSpeed.z;
            rb.linearVelocity = transform.right * weapon.GetSpeed();
        }

        // Prevent the area from being 0, as it hides the projectile.

        area = weapon.GetArea();
        if(area <= 0) area = 1;
        transform.localScale = new Vector3(
            area * Mathf.Sign(transform.localScale.x),
            area * Mathf.Sign(transform.localScale.y), 1
        );

        // Set how much piercing this object has.
        piercing = stats.piercing;

        // Destroy the projectile after its lifespan expires.
        if (weapon.GetLifespan() > 0) Destroy(gameObject, weapon.GetLifespan());

        // If the projectile is auto-aiming, automatically find a suitable enemy.
        if (hasAutoAim) AcquireAutoAimFacing();
    }

    // If the projectile is homing, it will automatically find a suitable target
    // to move towards.
    public virtual void AcquireAutoAimFacing()
    {
        float aimAngle;

        // Find all enemies
        EnemyStats[] targets = FindObjectsOfType<EnemyStats>();

        if (targets.Length > 0)
        {
            // Track the closest enemy
            EnemyStats closestTarget = null;
            float closestDistance = Mathf.Infinity;

            foreach (var enemy in targets)
            {
                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestTarget = enemy;
                    closestDistance = distance;
                }
            }

            if (closestTarget != null)
            {
                Vector2 difference = closestTarget.transform.position - transform.position;
                aimAngle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            }
            else
            {
                aimAngle = Random.Range(0f, 360f);
            }
        }
        else
        {
            aimAngle = Random.Range(0f, 360f);
        }

        // Apply the rotation
        transform.rotation = Quaternion.Euler(0, 0, aimAngle);
    }

    // Update is called once per frame
    protected virtual void FixedUpdate()
    {
        // Only drive movement ourselves if this is a kinematic.
        if (rb.bodyType == RigidbodyType2D.Kinematic)
        {
            Weapon.Stats stats = weapon.GetStats();
            transform.position += transform.right * stats.speed * weapon.Owner.Stats.speed * Time.fixedDeltaTime;
            rb.MovePosition(transform.position);
            transform.Rotate(rotationSpeed * Time.fixedDeltaTime);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        EnemyStats es = other.GetComponent<EnemyStats>();
        BreakableProps p = other.GetComponent<BreakableProps>();

        // Only collide with enemies or breakable stuff.
        if (es)
        {
            // If there is an owner, and the damage source is set to owner,
            // we will calculate knockback using the owner instead of the projectile.
            Vector3 source = damageSource == DamageSource.owner && owner ? owner.transform.position : transform.position;

            // Deals damage
            es.TakeDamage(GetDamage(), source);

            Weapon.Stats stats = weapon.GetStats();

            weapon.ApplyBuffs(es); // Apply all assigned buffs to the target.

            // Reduce the piercing value, and destroy the projectile if it runs of out of piercing.
            piercing--;
            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);
            }
        }
        else if (p)
        {
            p.TakeDamage(GetDamage());
            piercing--;

            Weapon.Stats stats = weapon.GetStats();
            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);
            }
        }

        // Destroy this object if it has run out of health from hitting other stuff.
        if (piercing <= 0) Destroy(gameObject);
    }
}