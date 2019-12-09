using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SetVolume : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider volSlider;

    void Awake()
    {
        // volSlider.value = PlayerPrefs.GetFloat("SavedVolume"); // sets the volume according to the latest preference
        // print("Set");
    }
    void Start()
    {
        VolumeLevel(volSlider.value);
    }
    public void VolumeLevel(float sliderValue)
    {
        mixer.SetFloat("MusicVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("SavedVolume", volSlider.value); // saves the volume value 
    }
}
