using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioController : MonoBehaviour
{
    public static AudioController instance;

    [Header("Background Music")]
    [SerializeField] private AudioSource backgroundMusicSource;
    [SerializeField] private AudioClip[] backgroundMusics;
    
    [Header("UI Sound Effects")]
    [SerializeField] private AudioSource uiSoundSource;
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip levelUpSound;

    [Header("Typewriter Sound Effects")]
    [SerializeField] private AudioSource typewriterSource;
    [SerializeField] private AudioClip[] typewriterSounds; // Multiple pre-edited sounds
    [SerializeField] private AudioClip baseTypewriterSound; // Single sound to modify
    [SerializeField] private bool useMultipleSounds = true; // Toggle between approaches
    
    [Header("Typewriter Sound Variation")] 
    [Range(0.8f, 1.2f)] public float minPitch = 0.9f;
    [Range(0.8f, 1.2f)] public float maxPitch = 1.1f;
    
    [Header("Player Sound Effects")]
    [SerializeField] private AudioSource playerSoundSource;
    [SerializeField] private AudioClip playerAttackSound;
    [SerializeField] private AudioClip playerHitSound;
    [SerializeField] private AudioClip playerDeathSound;

    [Header("Audio Settings")]
    [Range(0, 1)] public float masterVolume = 1f;
    [Range(0, 1)] public float musicVolume = 1f;
    [Range(0, 1)] public float sfxVolume = 1f;

    private void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
    }

    public void PlayBackgroundMusic(int musicIndex)
    {
        if (musicIndex < 0 || musicIndex >= backgroundMusics.Length) return;

        backgroundMusicSource.clip = backgroundMusics[musicIndex];
        backgroundMusicSource.Play();
    }

    public void StopBackgroundMusic(float fadeOutDuration = 0f)
    {
        if (backgroundMusicSource != null)
        {
            if (fadeOutDuration <= 0)
            {
                backgroundMusicSource.Stop();
            }
            else
            {
                StartCoroutine(FadeOutMusic(fadeOutDuration));
            }
        }
    }

    private IEnumerator FadeOutMusic(float fadeOutDuration)
    {
        float startVolume = backgroundMusicSource.volume;
        float currentTime = 0;

        while (currentTime < fadeOutDuration)
        {
            currentTime += Time.deltaTime;
            backgroundMusicSource.volume = startVolume * (1 - (currentTime / fadeOutDuration));
            yield return null;
        }

        backgroundMusicSource.Stop();
        backgroundMusicSource.volume = startVolume; // Reset volume for next play
    }

    public void UpdateVolume()
    {
        backgroundMusicSource.volume = masterVolume * musicVolume;
        uiSoundSource.volume = masterVolume * sfxVolume;
        playerSoundSource.volume = masterVolume * sfxVolume;
        typewriterSource.volume = masterVolume * sfxVolume;

    }
    public void PlayTypewriterSound()
    {
        if (useMultipleSounds)
        {
            // Approach 1: Multiple pre-edited sounds
            if (typewriterSounds != null && typewriterSounds.Length > 0)
            {
                int randomIndex = Random.Range(0, typewriterSounds.Length);
                typewriterSource.pitch = 1f; // Reset pitch
                typewriterSource.PlayOneShot(typewriterSounds[randomIndex]);
            }
        }
        else
        {
            // Approach 2: Single sound with runtime modifications
            if (baseTypewriterSound != null)
            {
                typewriterSource.pitch = Random.Range(minPitch, maxPitch);
                typewriterSource.PlayOneShot(baseTypewriterSound);
            }
        }
    }
}
