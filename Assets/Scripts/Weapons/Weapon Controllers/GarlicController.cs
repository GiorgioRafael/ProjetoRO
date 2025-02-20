using UnityEngine;

public class GarlicController : WeaponController
{
    

    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Attack()
    {
        base.Attack();
        GameObject spawnedGarlic = Instantiate(weaponData.Prefab);
        spawnedGarlic.transform.position = transform.position; //coloca a posicao do objeto como sendo a mesma que do seu pai (player)
        spawnedGarlic.transform.parent = transform; //spawna abaixo do objeto
    }
}
