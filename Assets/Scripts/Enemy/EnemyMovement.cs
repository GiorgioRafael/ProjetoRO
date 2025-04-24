using UnityEngine;

public class EnemyMovement : Sortable
{
    [SerializeField]
    protected bool LeftEnemy;

    protected EnemyStats stats;
    protected Transform player;
    protected Rigidbody2D rb; // For checking if enemy has a rigidbody.

    protected Vector2 knockbackVelocity;
    protected float knockbackDuration;

    public enum OutOfFrameAction { none, respawnAtEdge, despawn }
    public OutOfFrameAction outOfFrameAction = OutOfFrameAction.respawnAtEdge;

    [System.Flags]
    public enum KnockbackVariance { duration = 1, velocity = 2 }
    public KnockbackVariance knockbackVariance = KnockbackVariance.velocity;

    protected bool spawnedOutOfFrame = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        spawnedOutOfFrame = !SpawnManager.IsWithinBoundaries(transform);
        stats = GetComponent<EnemyStats>();

        // Picks a random player on the screen, instead of always picking the 1st player.
        PlayerMovement[] allPlayers = FindObjectsOfType<PlayerMovement>();
        player = allPlayers[Random.Range(0, allPlayers.Length)].transform;
    }

    protected virtual void Update()
    {
        // If we are currently being knocked back, then process the knockback.
        if (knockbackDuration > 0)
        {
            transform.position += (Vector3)knockbackVelocity * Time.deltaTime;
            knockbackDuration -= Time.deltaTime;
        }
        else
        {
            Move();
            HandleOutOfFrameAction();
        }
    }

    // If the enemy falls outside of the frame, handle it.
    protected virtual void HandleOutOfFrameAction()
    {
        // Handle the enemy when it is out of frame.
        if (!SpawnManager.IsWithinBoundaries(transform))
        {
            switch (outOfFrameAction)
            {
                case OutOfFrameAction.none:
                default:
                    break;
                case OutOfFrameAction.respawnAtEdge:
                    // If the enemy is outside the camera frame, teleport it back to the edge of the frame.
                    transform.position = SpawnManager.GeneratePosition();
                    break;
                case OutOfFrameAction.despawn:
                    // Don't destroy if it is spawned outside the frame.
                    if (!spawnedOutOfFrame)
                    {
                        Destroy(gameObject);
                    }
                    break;
            }
        }
        else spawnedOutOfFrame = false;
    }

    // This is meant to be called from other scripts to create knockback.
    public virtual void Knockback(Vector2 velocity, float duration)
    {
        // Ignore the knockback if the duration is greater than 0.
        if (knockbackDuration > 0) return;



        // Ignore knockback if the knockback type is set to none.
        if (knockbackVariance == 0) return;

        // Only change the factor if the multiplier is not 0 or 1.
        float pow = 1;
        bool reducesVelocity = (knockbackVariance & KnockbackVariance.velocity) > 0,
             reducesDuration = (knockbackVariance & KnockbackVariance.duration) > 0;

        if (reducesVelocity && reducesDuration) pow = 0.5f;

        // Check which knockback values to affect.
        knockbackVelocity = velocity * (reducesVelocity ? Mathf.Pow(stats.Actual.knockbackMultiplier, pow) : 1);
        knockbackDuration = duration * (reducesDuration ? Mathf.Pow(stats.Actual.knockbackMultiplier, pow) : 1);
    }

    public SpriteRenderer spriteRenderer;

    public virtual void Move()
    {
        Vector3 direction = player.transform.position - transform.position;

        // Move towards the player
        transform.position = Vector2.MoveTowards(
            transform.position,
            player.transform.position, 
            stats.Actual.moveSpeed * Time.deltaTime);

        // Flip the sprite based on movement direction
        if (direction.x < 0)
        {
            spriteRenderer.flipX = !LeftEnemy; // Moving left
        }
        else if (direction.x > 0)
        {
            spriteRenderer.flipX = LeftEnemy; // Moving right
        }
    }

}