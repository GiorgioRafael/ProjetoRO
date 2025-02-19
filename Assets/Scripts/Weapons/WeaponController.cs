using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Script Basico para todas as armas
public class WeaponController : MonoBehaviour
{


    [Header("Weapon Stats")]
    public GameObject prefab;
    public float damage;
    public float speed;
    public float cooldownDuration;
    float currentCooldown;
    public int pierce;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        currentCooldown = cooldownDuration; //instancia o cooldown da arma (inicial) ao cooldown da arma
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        currentCooldown -= Time.deltaTime; //reduz o cooldown da arma
        if(currentCooldown <= 0f) //se o cooldown estiver em zero Attack
        {
            Attack();
        }
        
    }
   protected virtual void Attack()
    {
        currentCooldown = cooldownDuration; //ao atacar, instancia novamente o cooldown
    }
}
