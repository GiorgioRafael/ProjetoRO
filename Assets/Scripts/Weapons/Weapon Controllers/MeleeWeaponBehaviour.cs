using UnityEngine;


//script base para todas as armas melee [deve ser colocado no prefab da arma melee]
public class MeleeWeaponBehaviour : MonoBehaviour
{
    public float destroyAfterSeconds;
    protected virtual void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);
    }

}
