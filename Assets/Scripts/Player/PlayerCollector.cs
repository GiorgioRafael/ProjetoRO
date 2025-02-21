using UnityEngine;

public class PlayerCollector : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        //verifica se o outro game object tem a interface ICollectible
        if(col.gameObject.TryGetComponent(out ICollectable collectable))
        {
            //se tem, chama o metodo de coleta
            collectable.Collect();
        }
    }
}
