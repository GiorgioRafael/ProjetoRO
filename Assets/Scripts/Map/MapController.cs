using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public List<GameObject> terrainChunks;
    public GameObject player;
    public float checkerRadius;
    public Vector3 noTerrainPosition;
    public LayerMask terrainMask;
    public GameObject currentChunk;
    Vector3 playerLastPosition;


    [Header("Optimization")]
    public List<GameObject> spawnedChunks;
    GameObject latestChunk;
    public float maxOpDist; //must be greater than the length and width of the tilemap
    float opDist;
    float optimizerCooldown;
    public float optimizerCooldownDur;
    

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
        if(!currentChunk)
        {
            return;
        }
        Vector3 moveDir = player.transform.position - playerLastPosition;
        playerLastPosition = player.transform.position;

        string directionName = GetDirectionName(moveDir);

        CheckAndSpawnChunk(directionName);
        
        if(directionName.Contains("Up"))
        {
            CheckAndSpawnChunk("Up");
        }
        if(directionName.Contains("Right"))
        {
            CheckAndSpawnChunk("Right");
        }
        if(directionName.Contains("Left"))
        {
            CheckAndSpawnChunk("Left");
        }
        if(directionName.Contains("Down"))
        {
            CheckAndSpawnChunk("Down");
        }
    }
    void CheckAndSpawnChunk(string direction)
    {
        if(!Physics2D.OverlapCircle(currentChunk.transform.Find(direction).position, checkerRadius, terrainMask))
        {
            SpawnChunk(currentChunk.transform.Find(direction).position);
        }
    }

    string GetDirectionName(Vector3 direction)
    {
        direction = direction.normalized;

        if(Mathf.Abs(direction.x)> Mathf.Abs(direction.y))
        {
            //movendo-se horizontalmente mais do que verticalmente
            if(direction.y > 0.5f)
            {
                //tambem se movendo para cima
                return direction.x > 0 ? "Right Up" : "Left Up";
            }
            else if( direction.y < -0.5f)
            {
                //tambem se movendo para baixo
                return direction.x > 0 ? "Right Down" : "Left Down";
            }
            else
            {
                //somente se movendo horizontalmente
                return direction.x > 0 ? "Right" : "Left";
            }
        }
        else
        {
            //movendo-se verticalmente mais do que horizontalmente
            if(direction.x > 0.5f)
            {
                //tambem se movendo para direita
                return direction.y > 0 ? "Right Up" : "Right Down";
            }
            else if( direction.x < -0.5f)
            {
                //tambem se movendo para esquerda
                return direction.y > 0 ? "Left Up" : "Left Down";
            }
            else
            {
                //movendo somente verticalmente
                return direction.y > 0 ? "Up" : "Down";
            }
        }
    }

    void SpawnChunk(Vector3 spawnPosition)
    {
        int rand = UnityEngine.Random.Range(0, terrainChunks.Count);
        latestChunk = Instantiate(terrainChunks[rand], spawnPosition, Quaternion.identity);
        spawnedChunks.Add(latestChunk);
    }
    void ChunkOptimizer()
    {

        optimizerCooldown -= Time.deltaTime;

        if(optimizerCooldown <= 0f)
        {
            optimizerCooldown = optimizerCooldownDur;
        }
        else
        {
            return;
        }

        foreach (GameObject chunk in spawnedChunks)
        {
            opDist = Vector3.Distance(player.transform.position, chunk.transform.position);
            if (opDist > maxOpDist)
            {
                chunk.SetActive(false);
            }
            else
            {
                chunk.SetActive(true);
            }
        }
    }
}