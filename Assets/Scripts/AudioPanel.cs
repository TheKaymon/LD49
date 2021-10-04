using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioPanel : MonoBehaviour
{
    //public Slider mainVolSlider;
    public Slider musicVolSlider;
    public Slider sfxVolSlider;

    //private float mainVolume;
    private float musicVolume;
    private float sfxVolume;

    private readonly float defaultVolume = 0.5f;

    //private readonly string mainVolPref = "MainVolume";
    private readonly string musicVolPref = "MusicVolume";
    private readonly string sfxVolPref = "SfxVolume";

    private void OnEnable()
    {
        //mainVolSlider.onValueChanged.AddListener(HandleMainSliderChanged);
        musicVolSlider.onValueChanged.AddListener(HandleMusicSliderChanged);
        sfxVolSlider.onValueChanged.AddListener(HandleSfxSliderChanged);
    }


    private void Start()
    {
        //mainVolume = PlayerPrefs.GetFloat(mainVolPref, defaultVolume);
        //Debug.Log($"Main Volume: {mainVolume}");
        //mainVolSlider.value = mainVolume;

        musicVolume = PlayerPrefs.GetFloat(musicVolPref, defaultVolume);
        Debug.Log($"Music Volume: {musicVolume}");
        musicVolSlider.value = musicVolume;

        sfxVolume = PlayerPrefs.GetFloat(sfxVolPref, defaultVolume);
        Debug.Log($"SFX Volume: {sfxVolume}");
        sfxVolSlider.value = sfxVolume;
    }

    private void OnDisable()
    {
        musicVolSlider.onValueChanged.RemoveAllListeners();
        sfxVolSlider.onValueChanged.RemoveAllListeners();
        //PlayerPrefs.SetFloat(mainVolPref, mainVolume);
        //PlayerPrefs.SetFloat(musicVolPref, musicVolume);
        //PlayerPrefs.SetFloat(sfxVolPref, sfxVolume);
    }

    //public void HandleMainSliderChanged( float pct )
    //{
    //    mainVolume = pct;
    //    Game.game.audioManager.SetMainVolume(pct);
    //}

    public void HandleMusicSliderChanged( float pct )
    {
        musicVolume = pct;
        Audio.instance.SetMusicVolume(pct);
        PlayerPrefs.SetFloat(musicVolPref, pct);
    }

    public void HandleSfxSliderChanged( float pct )
    {
        sfxVolume = pct;
        Audio.instance.SetSfxVolume(pct);
        PlayerPrefs.SetFloat(sfxVolPref, pct);
    }
}
