using System.Collections;
using UnityEngine;

public class IgnoreEnemyCollision : MonoBehaviour
{
    IEnumerator Start()
    {
        // Aguarda 0.5 segundos para garantir que todos os inimigos estejam carregados
        yield return new WaitForSeconds(0.5f);

        Collider2D myCollider = GetComponent<Collider2D>();
        if (myCollider == null)
        {
            Debug.LogWarning("Collider da planta não encontrado!");
            yield break;
        }

        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in allEnemies)
        {
            if (enemy == gameObject) continue; // Ignora a si mesmo

            Collider2D enemyCollider = enemy.GetComponent<Collider2D>();
            if (enemyCollider != null)
            {
                Physics2D.IgnoreCollision(myCollider, enemyCollider);
            }
        }
    }
}
