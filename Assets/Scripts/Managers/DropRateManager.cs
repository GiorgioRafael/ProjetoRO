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
        public int minAmount = 1;
        public int maxAmount = 1;
        public float spreadRadius = 0.5f; // Maximum distance from center point
    }

    public bool active = false;
    public List<Drops> drops;

    void OnDestroy()
    {
        if(!active) return;
        if(!gameObject.scene.isLoaded) return;
        
        float randomNumber = UnityEngine.Random.Range(0f, 100f);
        Drops rarestDrop = null;

        foreach (Drops drop in drops)
        {
            if (randomNumber <= drop.dropRate)
            {
                if (rarestDrop == null || drop.dropRate < rarestDrop.dropRate)
                {
                    rarestDrop = drop;
                }
            }
        }

        if (rarestDrop != null && gameObject.scene.isLoaded)
        {
            int amount = UnityEngine.Random.Range(rarestDrop.minAmount, rarestDrop.maxAmount + 1);
            
            for(int i = 0; i < amount; i++)
            {
                // Calculate random position within a circle
                float angle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
                float radius = UnityEngine.Random.Range(0f, rarestDrop.spreadRadius);
                
                Vector3 offset = new Vector3(
                    Mathf.Cos(angle) * radius,
                    Mathf.Sin(angle) * radius,
                    0
                );
                
                Instantiate(rarestDrop.itemPrefab, transform.position + offset, Quaternion.identity);
            }
        }
    }
}