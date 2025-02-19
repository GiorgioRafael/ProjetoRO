using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Script Basico para todas as armas
public class WeaponController : MonoBehaviour
{


    [Header("Weapon Stats")]
    public WeaponScriptableObject weaponData;
    float currentCooldown;

    protected PlayerMovement pm;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        pm = FindFirstObjectByType<PlayerMovement>();
        currentCooldown = weaponData.CooldownDuration; //instancia o cooldown da arma (inicial) ao cooldown da arma
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
        currentCooldown = weaponData.CooldownDuration; //ao atacar, instancia novamente o cooldown
    }
}
