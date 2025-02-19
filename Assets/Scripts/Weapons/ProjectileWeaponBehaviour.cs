using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Script basico para o funcionamento de todas as armas que são projeteis [deve ser colocado em uma arma prefab que seja um projetil]
public class ProjectileWeaponBehaviour : MonoBehaviour
{
    
    protected Vector3 direction;
    public float destroyAfterSeconds;

    protected virtual void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);
    }

    // Update is called once per frame
    public void DirectionChecker(Vector3 dir)
    {
        direction = dir;
    }
}
