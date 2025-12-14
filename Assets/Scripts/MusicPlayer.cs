using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource BackgroundMusic;
    public AudioSource MiniGameMusic;
    public AudioSource MiniGameMusic2;
    public AudioSource MiniGameMusic3;
    public AudioSource SoundEffects;


    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void PlayBackgroundMusic(AudioClip clip)
    {
        MiniGameMusic.Stop();
        BackgroundMusic.clip = clip;
        BackgroundMusic.loop = true;
        BackgroundMusic.Play();
    }


    public void PlayMiniGameMusic(AudioClip clip)
    {
        BackgroundMusic.Stop();
        MiniGameMusic.clip = clip;
        MiniGameMusic.loop = true;
        MiniGameMusic.Play();
    }

    public void PlaySoundeffects(AudioClip clip)
    {
        SoundEffects.PlayOneShot(clip);
    }
}
