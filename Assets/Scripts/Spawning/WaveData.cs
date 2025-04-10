using UnityEngine;

[CreateAssetMenu(fileName = "Wave Data", menuName = "2D Top-down Rogue-like/Wave Data")]
public class WaveData : SpawnData
{

    [Header("Wave Data")]

    [Tooltip("If there are less than this number of enemies, we will keep spawning until we get there.")]
    [Min(0)] public int startingCount = 0;

    [Tooltip("How many enemies can this wave spawn at maximum?")]
    [Min(1)] public uint totalSpawns = uint.MaxValue;

    [System.Flags] public enum ExitCondition { waveDuration = 1, reachedTotalSpawns = 2 }
    [Tooltip("Set the things that can trigger the end of this wave")]
    public ExitCondition exitConditions = (ExitCondition)1;
    
    [Tooltip("All enemies must be dead for the wave to advance.")]
    public bool mustKillAll = false;

    [HideInInspector] public uint spawnCount;  //The number of enemies already spawned in this wave

    // Returns an array of prefabs that this wave can spawn.
    // Takes an optional parameter of how many enemies are on the screen at the moment.
    public override GameObject[] GetSpawns(int totalEnemies = 0)
    {
        // Determine how many enemies to spawn.
        int count = Random.Range(spawnsPerTick.x, spawnsPerTick.y);

        // If we have less than <minimumEnemies> on the screen, we will 
        // set the count to be equals to the number of enemies to spawn to
        // populate the screen until it has <minimumEnemies> within.
        if (totalEnemies + count < startingCount)
            count = startingCount - totalEnemies;

        // Generate the result.
        GameObject[] result = new GameObject[count];
        for(int i = 0; i < count; i++)
        {
            // Randomly picks one of the possible spawns and inserts it
            // into the result array.
            result[i] = possibleSpawnPrefabs[Random.Range(0, possibleSpawnPrefabs.Length)];
        }

        return result;
    }
}