using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Audio : MonoBehaviour
{
    public static Audio instance;
    private void Awake()
    {
        if ( instance != null )
            Debug.Log("Multiple Audio Managers!");
        instance = this;
        //DontDestroyOnLoad(this);
    }

    public AudioMixer main;
    public float minVolume = -40f;
    public float maxVolume = 10f;
    public AudioSource musicChannel;
    public List<AudioSource> sfxChannels;


    //public readonly string masterMix = "MasterVol";
    public readonly string musicMix = "MusicVolume";
    public readonly string effectMix = "SFXVolume";

    private int channelIndex = 0;

    //public void SetMainVolume( float percent )
    //{
    //    float volume = Mathf.Lerp(minVolume, maxVolume, percent);
    //    main.SetFloat(masterMix, volume);
    //}

    public void SetMusicVolume( float percent )
    {
        float volume = Mathf.Lerp(minVolume, maxVolume, percent);
        main.SetFloat(musicMix, volume);
    }

    public void SetSfxVolume( float percent )
    {
        float volume = Mathf.Lerp(minVolume, maxVolume, percent);
        main.SetFloat(effectMix, volume);
    }

    public void PlayMusic()
    {
        musicChannel.Play();
    }

    public void StopMusic()
    {
        musicChannel.Stop();
    }

    public void PlaySFX( AudioClip sound, Vector2 position, float pitch = 1f )
    {
        sfxChannels[channelIndex].transform.SetPositionAndRotation(position, Quaternion.identity);
        //Debug.Log($"Playing Sound index {soundIndex} on channel {channelIndex}");
        sfxChannels[channelIndex].pitch = pitch;
        sfxChannels[channelIndex].PlayOneShot(sound);
        channelIndex = ( channelIndex + 1 ) % sfxChannels.Count;
    }
}
