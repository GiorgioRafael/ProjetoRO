using UnityEngine;

public class KnifeBehaviour : ProjectileWeaponBehaviour
{
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * currentSpeed * Time.deltaTime; //seta o movimento da faca
    }
}
    