using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class AudioManager : MonoBehaviour
{

    public static AudioManager Instance;
    [SerializeField] private Sound[] musicSounds, sfxSounds;
    [SerializeField] private AudioSource musicSource, sfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        musicSource.volume = PlayerPrefs.GetFloat("MusicVolume", 0.3f);
        musicSource.mute = PlayerPrefs.GetInt("IsMusicMuted", 1) == 0;
        sfxSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        sfxSource.mute = PlayerPrefs.GetInt("IsSFXMuted", 1) == 0;
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.m_name == name);

        if (s == null)
        {
            Debug.Log("Sound Not Found");
        }
        else
        {
            musicSource.clip = s.m_clip;
            musicSource.Play();
        }
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.m_name == name);

        if (s == null)
        {
            Debug.Log("Sound Not Found");
        }
        else
        {
            sfxSource.PlayOneShot(s.m_clip);
        }
    }

    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
        PlayerPrefs.SetInt("IsMusicMuted", musicSource.mute ? 0 : 1);
    }

    public void ToggleSFX()
    {
        sfxSource.mute = !sfxSource.mute;

        PlayerPrefs.SetInt("IsSFXMuted", musicSource.mute ? 0 : 1);
    }

    public void MusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        musicSource.volume = volume;
    }

    public void SFXVolume(float volume)
    {
        PlayerPrefs.SetFloat("SFXVolume", volume);
        sfxSource.volume = volume;
    }
}
