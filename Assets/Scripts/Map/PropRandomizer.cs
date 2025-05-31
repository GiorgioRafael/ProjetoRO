using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropRandomizer : MonoBehaviour
{
    public List<GameObject> propSpawnPoints;

    [Header("Props by Rarity")]
    public List<GameObject> commonProps;      // 50% chance 60
    public List<GameObject> uncommonProps;    // 20% chance 20
    public List<GameObject> rareProps;        // 15% chance 10 
    public List<GameObject> epicProps;        // 10% chance 6
    public List<GameObject> legendaryProps;   // 5% chance  4

    void Start()
    {
        SpawnProps();
    }

    GameObject GetRandomPropByRarity()
    {
        int roll = Random.Range(0, 100);

        if (roll < 60 && commonProps.Count > 0)
            return commonProps[Random.Range(0, commonProps.Count)];
        if (roll < 80 && uncommonProps.Count > 0)
            return uncommonProps[Random.Range(0, uncommonProps.Count)];
        if (roll < 90 && rareProps.Count > 0)
            return rareProps[Random.Range(0, rareProps.Count)];
        if (roll < 96 && epicProps.Count > 0)
            return epicProps[Random.Range(0, epicProps.Count)];
        if (legendaryProps.Count > 0)
            return legendaryProps[Random.Range(0, legendaryProps.Count)];


        if (commonProps.Count > 0)
            return commonProps[Random.Range(0, commonProps.Count)];

        return null;
    }

    void SpawnProps()
    {
        foreach (GameObject sp in propSpawnPoints)
        {
            GameObject selectedProp = GetRandomPropByRarity();
            if (selectedProp != null)
            {
                GameObject prop = Instantiate(selectedProp, sp.transform.position, Quaternion.identity);
                prop.transform.parent = sp.transform;
            }
        }
    }
}