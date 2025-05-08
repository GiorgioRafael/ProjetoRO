using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [System.Serializable]
    public class ChunkData
    {
        public GameObject chunkPrefab;
    }

    [Header("Chunk Rarity Categories")]
    public List<ChunkData> commonChunks;
    public List<ChunkData> uncommonChunks;
    public List<ChunkData> rareChunks;
    public List<ChunkData> ultraRareChunks;

    [Header("Spawn Probabilities")]
    [Range(0f, 1f)] public float commonProbability = 0.7f;
    [Range(0f, 1f)] public float uncommonProbability = 0.2f;
    [Range(0f, 1f)] public float rareProbability = 0.09f;
    [Range(0f, 1f)] public float ultraRareProbability = 0.01f;

    public GameObject player;
    public float checkerRadius;
    public LayerMask terrainMask;
    public GameObject currentChunk;
    private Vector3 playerLastPosition;

    [Header("Optimization")]
    public List<GameObject> spawnedChunks = new List<GameObject>();
    public float maxOpDist = 50f;
    private float optimizerCooldown;
    public float optimizerCooldownDur = 2f;

    void Start()
    {
        playerLastPosition = player.transform.position;
    }

    void Update()
    {
        ChunkChecker();
        ChunkOptimizer();
    }

    void ChunkChecker()
    {
        if (!currentChunk) return;

        Vector3 moveDir = player.transform.position - playerLastPosition;
        playerLastPosition = player.transform.position;

        string directionName = GetDirectionName(moveDir);

        CheckAndSpawnChunk(directionName);

        if (directionName.Contains("Up")) CheckAndSpawnChunk("Up");
        if (directionName.Contains("Down")) CheckAndSpawnChunk("Down");
        if (directionName.Contains("Left")) CheckAndSpawnChunk("Left");
        if (directionName.Contains("Right")) CheckAndSpawnChunk("Right");
    }

    void CheckAndSpawnChunk(string direction)
    {
        Transform dirTransform = currentChunk.transform.Find(direction);
        if (dirTransform == null) return;

        Vector3 spawnPosition = dirTransform.position;

        if (!Physics2D.OverlapCircle(spawnPosition, checkerRadius, terrainMask))
        {
            SpawnChunk(spawnPosition);
        }
    }

    string GetDirectionName(Vector3 direction)
    {
        direction = direction.normalized;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.y > 0.5f) return direction.x > 0 ? "Right Up" : "Left Up";
            else if (direction.y < -0.5f) return direction.x > 0 ? "Right Down" : "Left Down";
            else return direction.x > 0 ? "Right" : "Left";
        }
        else
        {
            if (direction.x > 0.5f) return direction.y > 0 ? "Right Up" : "Right Down";
            else if (direction.x < -0.5f) return direction.y > 0 ? "Left Up" : "Left Down";
            else return direction.y > 0 ? "Up" : "Down";
        }
    }

    void SpawnChunk(Vector3 spawnPosition)
{
    float totalProbability = ultraRareProbability + rareProbability + uncommonProbability + commonProbability;
    float roll = Random.value * totalProbability;

    List<ChunkData> selectedList = null;

    if (roll <= ultraRareProbability && ultraRareChunks.Count > 0)
    {
        selectedList = ultraRareChunks;
    }
    else if (roll <= ultraRareProbability + rareProbability && rareChunks.Count > 0)
    {
        selectedList = rareChunks;
    }
    else if (roll <= ultraRareProbability + rareProbability + uncommonProbability && uncommonChunks.Count > 0)
    {
        selectedList = uncommonChunks;
    }
    else if (commonChunks.Count > 0)
    {
        selectedList = commonChunks;
    }

    if (selectedList == null || selectedList.Count == 0)
    {
        Debug.LogWarning("No valid chunks to spawn.");
        return;
    }

    int rand = Random.Range(0, selectedList.Count);
    GameObject chunkPrefab = selectedList[rand].chunkPrefab;

    GameObject spawnedChunk = Instantiate(chunkPrefab, spawnPosition, Quaternion.identity);
    spawnedChunks.Add(spawnedChunk);
}


    void ChunkOptimizer()
    {
        optimizerCooldown -= Time.deltaTime;

        if (optimizerCooldown > 0f) return;

        optimizerCooldown = optimizerCooldownDur;

        foreach (GameObject chunk in spawnedChunks)
        {
            if (!chunk) continue;

            float dist = Vector3.Distance(player.transform.position, chunk.transform.position);
            chunk.SetActive(dist <= maxOpDist);
        }
    }
}
