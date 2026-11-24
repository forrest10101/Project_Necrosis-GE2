using DZAssets.AudioManager;
using UnityEngine;
using UnityEngine.UI;

// Volume adjuster by Evan Montgomery
public class AudioVolumeSliders : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;                    // Music Slider
    [SerializeField] private Slider sfxSlider;                      // SFX Slider

    private void OnEnable()
    {
        AudioManager audio = AudioManager.Instance;                 // Find Audio Manager

        musicSlider.SetValueWithoutNotify(audio.BGMVolume);         // Find volume value
        sfxSlider.SetValueWithoutNotify(audio.SFXVolume);           // Find volume value
    }

    public void SetMusicVolume(float volume)
    {
        AudioManager.Instance.SetBGMVolume(volume);                 // Adjust volume
    }

    public void SetSFXVolume(float volume)
    {
        AudioManager.Instance.SetSFXVolume(volume);                 // Adjust volume
    }
}