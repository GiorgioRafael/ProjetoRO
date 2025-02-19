using UnityEngine;

public class KnifeBehaviour : ProjectileWeaponBehaviour
{
    KnifeController kc;

    void Start()
    {
        base.Start();
        kc = FindFirstObjectByType<KnifeController>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * kc.speed * Time.deltaTime; //seta o movimento da faca
    }
}
