using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public EnemyScriptableObject enemyData;
    Transform player;
    
    void Start()
    {
        player = FindFirstObjectByType<PlayerMovement>().transform;

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, enemyData.MoveSpeed * Time.deltaTime); //move-se sempre na direcao do player
    }
}
