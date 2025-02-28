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
        public float dropRate; // Drop chance percentage
    }

    public List<Drops> drops;

    void OnDestroy()
    {
        if(!gameObject.scene.isLoaded)
        {
            return;
        }
        
        float randomNumber = UnityEngine.Random.Range(0f, 100f);
        Drops rarestDrop = null;

        foreach (Drops drop in drops)
        {
            if (randomNumber <= drop.dropRate)
            {
                // Always pick the rarest drop possible
                if (rarestDrop == null || drop.dropRate < rarestDrop.dropRate)
                {
                    rarestDrop = drop;
                }
            }
        }

        // Ensure scene is still active before spawning
        if (rarestDrop != null && gameObject.scene.isLoaded)
        {
            Instantiate(rarestDrop.itemPrefab, transform.position, Quaternion.identity);
        }
    }
}