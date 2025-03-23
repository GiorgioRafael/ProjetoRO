using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public List<EnemyGroup> enemyGroups; //lista de um grupo de inimigos para spawnar nessa wave
        public int waveQuota; //numero total de inimigos a serem spawnados nessa wave
        public float spawnInterval; //cooldown para spawnar inimigos
        public int spawnCount; //numero de inimigos ja spawnados

    }

    [System.Serializable]
    public class EnemyGroup
    {
        public string enemyName; 
        public int enemyCount; //numero de inimigmos a ser spawnado nessa wave
        public int spawnCount; //numero de inimigos desse tipo que ja spawnaram nessa wave
        public GameObject enemyPrefab; //

    }


    public List<Wave> waves; //lista de todas as waves no jogo
    public int currentWaveCount;

    [Header("Spawner Attributes")]
    float spawnTimer; //timer usado para definir quando spawnar proximo inimigo
    public int enemiesAlive;
    public int maxEnemiesAllowed; //numero maximo de inimigos simultaneamente
    public bool maxEnemiesReached = false; //variavel para armazenar se se a tela esta com o maximo de inimigos
    public float waveInterval; //o intervalo entre cada wave
    public bool isWaveActive = false;

    [Header("Spawn Positions")]
    public List<Transform> relativeSpawnPoints; //uma lista para armazenar todos os spawn points relativos

    Transform player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<PlayerStats>().transform;
        CalculateWaveQuota();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentWaveCount < waves.Count && waves[currentWaveCount].spawnCount == 0 && !isWaveActive) //verifica se a wave terminou e a proxima wave deve comecar
        {
            StartCoroutine(BeginNextWave());
        }
        spawnTimer += Time.deltaTime;

        if(spawnTimer >= waves[currentWaveCount].spawnInterval)
        {
            spawnTimer = 0f;
            SpawnEnemies();
        }
    }

    IEnumerator BeginNextWave()
    {
        isWaveActive = true;
        //wait for waveInterval seconds before starting the next wave
        yield return new WaitForSeconds(waveInterval);

        //IOf verifica se tem mais wave para spawnar depois da wave atual, vai para proxima wave
        if(currentWaveCount < waves.Count - 1)
        {
            isWaveActive = false;
            currentWaveCount++;
            CalculateWaveQuota();
        }
    }

    void CalculateWaveQuota()
    {
        int currentWaveQuota = 0;
        foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
        {
            currentWaveQuota += enemyGroup.enemyCount;
        }
        waves[currentWaveCount].waveQuota = currentWaveQuota;
        Debug.LogWarning(currentWaveQuota);
    }
    /// <summary>
    /// esse metodo vai parar de spawnar inimigos se a quantidade de inimigos no mapa for a maxima
    /// o metodo so vai spawnar inimigos de uma wave especifica até o tempo de fim dessa wave (proxima wave começar)
    /// </summary>

    void SpawnEnemies()
    {
        if(waves[currentWaveCount].spawnCount < waves[currentWaveCount].waveQuota &&!maxEnemiesReached)
        {
            foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
            {
                if(enemyGroup.spawnCount < enemyGroup.enemyCount)
                {


                    //spawna o inimigo em uma posicao aleatoria perto do player
                    Instantiate(enemyGroup.enemyPrefab, player.position + relativeSpawnPoints[Random.Range(0, relativeSpawnPoints.Count)].position, Quaternion.identity);

                    //Vector2 spawnPosition = new Vector2(player.transform.position.x + Random.Range(-10f, 10f), player.transform.position.y + Random.Range(-10f, 10f));
                    //Instantiate(enemyGroup.enemyPrefab, spawnPosition, Quaternion.identity);

                    enemyGroup.spawnCount++;
                    waves[currentWaveCount].spawnCount++;
                    enemiesAlive++;

                    //limita quantidade de inimigos na tela
                    if(enemiesAlive >= maxEnemiesAllowed)
                    {
                        maxEnemiesReached = true;
                        return;
                    }
                }
            }
        }
    }
    public void OnEnemyKilled()
    {
        enemiesAlive--;
        //reseta a flag se a quantidade diminuir da quantidade maxima
        if(enemiesAlive < maxEnemiesAllowed)
        {
            maxEnemiesReached = false;
        }
    }
}
