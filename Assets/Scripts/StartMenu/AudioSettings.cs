using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
public class AudioSettings : MonoBehaviour{   
    
    [SerializeField] private AudioMixer audioMix;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private AudioSource SFXSource;

    public AudioClip posButtonSFX;
    
    public void SetMusicVolume(){
        float volume = musicSlider.value;
        audioMix.SetFloat("Music", volume);
    }
    public void SetSFXVolume(){
        float volume = sfxSlider.value;
        audioMix.SetFloat("SFX", volume);
    }

    public void PlaySFX(AudioClip clip){
        SFXSource.PlayOneShot(clip);
    }
}
