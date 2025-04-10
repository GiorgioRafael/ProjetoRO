using UnityEngine;

public class ChargingEnemyMovement : EnemyMovement
{

    Vector2 chargeDirection;

    // We calculate the direction where the enemy charges towards first,
    // i.e. where the player is when the enemy spawns.
    protected override void Start()
    {
        base.Start();
        chargeDirection = (player.transform.position - transform.position).normalized;
    }

    // Instead of moving towards the player, we just move towards
    // the direction we are charging towards.
    public override void Move()
    {
        transform.position += (Vector3)chargeDirection * enemy.currentMoveSpeed * Time.deltaTime;
    }
}