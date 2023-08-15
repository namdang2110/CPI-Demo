using Scrips;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;
    public static SoundManager Instance => _instance;

    // Sound effects
    public AudioSource touchSoundEffect;
    public AudioSource clickSoundEffect;
    public AudioSource winSoundEffect;
    public bool isMute;
    
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }

        isMute = PlayerPrefs.GetInt("IsMute", 0) == 1;

        clickSoundEffect.mute = isMute;
        touchSoundEffect.mute = isMute;
        winSoundEffect.mute = isMute;
    }

    public void PlayTouchSound()
    {
        touchSoundEffect.Play();
    }

    public void PlayClickSound()
    {
        clickSoundEffect.Play();
    }

    public void PlayWinSound()
    {
        winSoundEffect.Play();
    }

    public void ToggleSound()
    {
        isMute = !isMute;
        
        clickSoundEffect.mute = isMute;
        touchSoundEffect.mute = isMute;
        winSoundEffect.mute = isMute;

        PlayerPrefs.SetInt("IsMute", isMute ? 1 : 0);
        PlayerPrefs.Save();
        
        Gameplay.Instance.soundImg.sprite = isMute ? Gameplay.Instance.mute : Gameplay.Instance.notMute;
    }
}
