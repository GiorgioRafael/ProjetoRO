using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class audiocon : MonoBehaviour
{

    public AudioSource audioSourceMusicaDeFundo;
    public AudioClip[] musicasDeFundo; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioClip musicaDeFundoDessaFase = musicasDeFundo[0];
        audioSourceMusicaDeFundo.clip = musicaDeFundoDessaFase;
        audioSourceMusicaDeFundo.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
