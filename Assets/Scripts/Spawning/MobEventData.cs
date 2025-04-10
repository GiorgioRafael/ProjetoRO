using UnityEngine;

[CreateAssetMenu(fileName = "Mob Event Data", menuName = "2D Top-down Rogue-like/Event Data/Mob")]
public class MobEventData : EventData
{
    [Header("Mob Data")]
    [Range(0f, 360f)] public float possibleAngles = 360f;
    [Min(0)] public float spawnRadius = 2f, spawnDistance = 20f;

    public override bool Activate(PlayerStats player = null)
    {
        // Only activate this if the player is present.
        if(player)
        {
            // Otherwise, we spawn a mob outside of the screen and move it towards the player.
            float randomAngle = Random.Range(0, possibleAngles) * Mathf.Deg2Rad;
            foreach (GameObject o in GetSpawns())
            {
                Instantiate(o, player.transform.position + new Vector3(
                    (spawnDistance + Random.Range(-spawnRadius, spawnRadius)) * Mathf.Cos(randomAngle),
                    (spawnDistance + Random.Range(-spawnRadius, spawnRadius)) * Mathf.Sin(randomAngle)
                ), Quaternion.identity);
            }
        }

        return false;
    }
}