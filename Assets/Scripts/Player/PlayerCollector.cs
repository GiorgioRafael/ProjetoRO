using UnityEngine;

public class PlayerCollector : MonoBehaviour
{
    PlayerStats player;
    CircleCollider2D playerCollector;
    public float pullSpeed;

    void Start()
    {
        player = FindFirstObjectByType<PlayerStats>();
        playerCollector = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        playerCollector.radius = player.CurrentMagnet;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //verifica se o outro game object tem a interface ICollectible
        if(col.gameObject.TryGetComponent(out ICollectable collectable))
        {
            //pega o componente rb do item 
            Rigidbody2D rb = col.gameObject.GetComponent<Rigidbody2D>();
            //vector2 apaonta do item para o player
            Vector2 forceDirection = (transform.position - col.transform.position).normalized;
            //aplica a forca que o item vai ser puxado para o player com o pullSpeed
            rb.AddForce(forceDirection * pullSpeed);
            //se tem, chama o metodo de coleta
            collectable.Collect();
        }
    }
}
