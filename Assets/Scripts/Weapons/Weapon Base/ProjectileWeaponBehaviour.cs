using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Script basico para o funcionamento de todas as armas que são projeteis [deve ser colocado em uma arma prefab que seja um projetil]
public class ProjectileWeaponBehaviour : MonoBehaviour
{
    public WeaponScriptableObject weaponData;

    protected Vector3 direction;
    public float currentDestroyAfterSeconds;

    //Current stats
    protected float currentDamage;
    protected float currentSpeed;
    protected float currentCooldownDuration;
    protected int currentPierce;
    //References
    


    void Awake()
    {
        currentDamage = weaponData.Damage;
        currentSpeed = weaponData.Speed;
        currentCooldownDuration = weaponData.CooldownDuration;
        currentPierce = weaponData.Pierce;
        currentDestroyAfterSeconds = weaponData.DestroyAfterSeconds;
    }

    public float GetCurrentDamage()
    {
        return currentDamage *= FindFirstObjectByType<PlayerStats>().currentMight;
    }

    protected virtual void Start()
    {
        Destroy(gameObject, currentDestroyAfterSeconds);
    }

    // Update is called once per frame
    public void DirectionChecker(Vector3 dir)
    {
        direction = dir;

        float dirx = direction.x;
        float diry = direction.y;

        Vector3 scale = transform.localScale;
        Vector3 rotation = transform.rotation.eulerAngles;

        if(dirx < 0 && diry == 0) //rotacao do objeto
        {
            scale.x = scale.x * -1;
            scale.y = scale.y * -1;
        }
        else if (dirx == 0 && diry < 0) //down
        {
            scale.y = scale.y * -1;
        }
        else if (dirx == 0 && diry > 0) //up
        {
            scale.x = scale.x * -1;
        }
        else if (dirx > 0 && diry > 0) //right up
        {
            rotation.z = 0f;
        }
        else if (dirx > 0 && diry < 0) //right down
        {
            rotation.z = -90f;
        }
        else if (dirx < 0 && diry > 0) //left up
        {
            scale.x = scale.x * -1;
            scale.y = scale.y * -1;
            rotation.z = -90f;
        }
        else if (dirx < 0 && diry < 0) //left down
        {
            scale.x = scale.x * -1;
            scale.y = scale.y * -1;
            rotation.z = 0f;
        }

        transform.localScale = scale;
        transform.rotation = Quaternion.Euler(rotation); //nao pode setar o vector por que nao se converte
    }
        protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        
        if(col.CompareTag("Enemy"))
        {
            //Debug.Log("Enemy Hit");
            EnemyStats enemy = col.GetComponent<EnemyStats>();
            enemy.TakeDamage(GetCurrentDamage());     //Make sure to use currentDamage instead of weaponData.Damage in case any damage multipliers in the future
            ReducePierce();
        }
        else if(col.CompareTag("Prop"))
        {
            if(col.gameObject.TryGetComponent(out BreakableProps breakable))
            {
                breakable.TakeDamage(GetCurrentDamage());
                ReducePierce();
            }
        }
    }
    void ReducePierce() //destroi o objeto quando chegar a 0 (penetracao)
    {
        currentPierce--;
        if(currentPierce <= 0)
        {
            Destroy(gameObject);
        }
    }
}
