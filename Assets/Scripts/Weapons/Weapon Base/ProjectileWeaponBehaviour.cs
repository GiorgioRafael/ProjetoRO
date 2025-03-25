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
        return currentDamage *= FindFirstObjectByType<PlayerStats>().CurrentMight;
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

    if (dirx > 0 && diry == 0) // right
    {
        rotation.z = -90f;
    }
    else if (dirx < 0 && diry == 0) // left
    {
        rotation.z = 90f;
    }
    else if (dirx == 0 && diry > 0) // up
    {
        rotation.z = 0f;
    }
    else if (dirx == 0 && diry < 0) // down
    {
        rotation.z = 180f;
    }
    else if (dirx > 0 && diry > 0) // right up
    {
        rotation.z = -45f;
    }
    else if (dirx > 0 && diry < 0) // right down
    {
        rotation.z = -135f;
    }
    else if (dirx < 0 && diry > 0) // left up
    {
        rotation.z = 45f;
    }
    else if (dirx < 0 && diry < 0) // left down
    {
        rotation.z = 135f;
    }

    transform.rotation = Quaternion.Euler(rotation);
}
        protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        
        if(col.CompareTag("Enemy"))
        {
            //Debug.Log("Enemy Hit");
            EnemyStats enemy = col.GetComponent<EnemyStats>();
            enemy.TakeDamage(GetCurrentDamage(), transform.position);     //Make sure to use currentDamage instead of weaponData.Damage in case any damage multipliers in the future
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
