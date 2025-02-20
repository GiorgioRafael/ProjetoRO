using System;
using System.Collections.Generic;
using UnityEngine;

public class DropRateManager : MonoBehaviour
{
    [System.Serializable]
    public class Drops
    {
        public string name;
        public GameObject itemPrefab;
        public float dropRate; //porcentagem
    }
    public List<Drops> drops;

    /*
    void OnDestroy()
    
    {
        float randomNumber = UnityEngine.Random.Range(0f, 100f);
        List<Drops> possibleDrops = new List<Drops>();
        foreach (Drops rate in drops)
        {
            if(randomNumber <= rate.dropRate)
            {
                possibleDrops.Add(rate);
            }
            
        }
        //verifica se há mais de um drop possivel
        if(possibleDrops.Count > 0)
        {
            Drops drops = possibleDrops[UnityEngine.Random.Range(0,possibleDrops.Count)];
            Instantiate(drops.itemPrefab, transform.position, Quaternion.identity);
        }
        
    }
    */
    void OnDestroy()
{
    float randomNumber = UnityEngine.Random.Range(0f, 100f);
    Debug.Log("Random number = " +randomNumber);
    Drops rarestDrop = null; // Store the rarest possible drop

    foreach (Drops rate in drops)
    {
        if (randomNumber <= rate.dropRate)
        {
            // Always pick the drop with the **lowest drop rate** that matches the roll
            if (rarestDrop == null || rate.dropRate < rarestDrop.dropRate)
            {
                rarestDrop = rate;
            }
        }
    }

    // If a valid drop was found, instantiate it
    if (rarestDrop != null)
    {
        Instantiate(rarestDrop.itemPrefab, transform.position, Quaternion.identity);
    }
}

  
}
