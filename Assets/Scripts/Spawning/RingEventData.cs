using UnityEngine;

[CreateAssetMenu(fileName = "Ring Event Data", menuName = "2D Top-down Rogue-like/Event Data/Ring")]
public class RingEventData : EventData
{
    [Header("Mob Data")]
    public ParticleSystem spawnEffectPrefab;
    public Vector2 scale = new Vector2(1, 1);
    [Min(0)] public float spawnRadius = 10f, lifespan = 15f;

    public override bool Activate(PlayerStats player = null)
    {
        // Only activate this if the player is present.
        if (player)
        {
            GameObject[] spawns = GetSpawns();
            float angleOffset = 2 * Mathf.PI / Mathf.Max(1, spawns.Length);
            float currentAngle = 0;
            foreach(GameObject g in spawns)
            {
                // Calculate the spawn position.
                Vector3 spawnPosition = player.transform.position + new Vector3(
                    spawnRadius * Mathf.Cos(currentAngle) * scale.x,
                    spawnRadius * Mathf.Sin(currentAngle) * scale.y
                );

                // If a particle effect is assigned, play it on the position.
                if(spawnEffectPrefab)
                    Instantiate(spawnEffectPrefab, spawnPosition, Quaternion.identity);

                // Then spawn the enemy.
                GameObject s = Instantiate(g, spawnPosition, Quaternion.identity);

                // If there is a lifespan on the mob, set them to be destroyed.
                if (lifespan > 0) Destroy(s, lifespan);

                currentAngle += angleOffset;
            }
        }

        return false;
    }
}