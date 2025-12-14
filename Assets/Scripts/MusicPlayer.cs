using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Music Clips")]
    public AudioClip backgroundMusic;
    public AudioClip[] minigameMusic;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        PlayBackgroundMusic();
    }

    // 🎵 Background Music
    public void PlayBackgroundMusic()
    {
        PlayMusic(backgroundMusic);
    }

    // 🎮 Minigame Music
    public void PlayMinigameMusic(int index)
    {
        if (index < 0 || index >= minigameMusic.Length) return;
        PlayMusic(minigameMusic[index]);
    }

    void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip == clip) return;

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    // 🔊 Sound Effects
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
}
