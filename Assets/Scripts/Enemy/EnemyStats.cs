using System.Collections;
using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
public class EnemyStats : MonoBehaviour
{
    public EnemyScriptableObject enemyData;

    private Animator animator;

    //Current stats
    [HideInInspector]
    public float currentMoveSpeed;
    [HideInInspector]
    public float currentHealth;
    [HideInInspector]
    public float currentDamage;

    public float despawnDistance = 20f;
    Transform player;

    [Header("Damage Feedback")]
    public Color damageColor = new Color(1,0,0,1);
    public float damageFlashDuration = 0.1f;
    public float deathFadeTime = 0.1f;
    Color originalColor;
    SpriteRenderer sr;
    EnemyMovement movement;

    void Awake()
    {
        //Assign the vaiables
        currentMoveSpeed = enemyData.MoveSpeed;
        currentHealth = enemyData.MaxHealth;
        currentDamage = enemyData.Damage;
    }

    void Start()
    {
        animator = GetComponent<Animator>();


        player = FindFirstObjectByType<PlayerStats>().transform;

        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;

        movement = GetComponent<EnemyMovement>();
    }

    void Update()
    {
        if(Vector2.Distance(transform.position, player.position) >= despawnDistance)
        {
            ReturnEnemy();
        }
    }

    float duration = 0.3f;
    public void TakeDamage(float dmg, Vector2 sourcePosition, float knockbackForce = 5f, float knockbackDuration = 0.2f)
    {
        currentHealth -= dmg;
        StartCoroutine(DamageFlash());

        if(dmg > 0)
        {
            GameManager.GenerateFloatingText(Mathf.FloorToInt(dmg).ToString(), transform, duration);
        }

        if(knockbackForce > 0)
        {
            Vector2 dir = (Vector2)transform.position - sourcePosition;
            movement.Knockback(dir.normalized * knockbackForce, knockbackDuration);
        }

        if (currentHealth <= 0)
        {
            Kill();
        }
    }

    IEnumerator DamageFlash()
    {
        sr.color = damageColor;
        yield return new WaitForSeconds(damageFlashDuration);
        sr.color = originalColor;
    }

    public void Kill()
    {
        StartCoroutine(KillFade());
    }

    IEnumerator KillFade()
    {
        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0, origAlpha = sr.color.a;

        while(t < deathFadeTime)
        {
            yield return w;
            animator.enabled = false;
            t += Time.deltaTime;

            //set the colour for this frame
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, (1 - t / deathFadeTime) * origAlpha);
        }
        Destroy(gameObject);
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        //referencia o script do collider colidido e da dano usando TakeDamage()
        if(col.gameObject.CompareTag("Player"))
        {
            PlayerStats player = col.gameObject.GetComponent<PlayerStats>();
            player.TakeDamage(currentDamage);
        }
    }

    private void OnDestroy()
    {
        EnemySpawner es = FindFirstObjectByType<EnemySpawner>();
        if(es != null)
         {
            es.OnEnemyKilled();
         }
    }
    void ReturnEnemy()
    {
        EnemySpawner es = FindFirstObjectByType<EnemySpawner>();
        transform.position = player.position + es.relativeSpawnPoints[Random.Range(0, es.relativeSpawnPoints.Count)].position;
    }
}