using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Music Clips")]
    public AudioClip backgroundMusic;
    public AudioClip[] minigameMusic;

    [Header("Settings")]
    [Range(0f, 1f)] public float musicVolume = 0.5f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 5f)] public float fadeDuration = 1f;

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

    // ======================
    // MUSIC
    // ======================

    public void PlayBackgroundMusic()
    {
        StartCoroutine(FadeMusic(backgroundMusic));
    }

    public void PlayMinigameMusic(int index)
    {
        if (index < 0 || index >= minigameMusic.Length) return;
        StartCoroutine(FadeMusic(minigameMusic[index]));
    }

    private IEnumerator FadeMusic(AudioClip newClip)
    {
        if (musicSource.clip == newClip) yield break;

        // Fade-Out
        float t = 0f;
        float startVolume = musicSource.volume;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.loop = true;
        musicSource.Play();

        // Fade-In
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, musicVolume, t / fadeDuration);
            yield return null;
        }
        musicSource.volume = musicVolume;
    }

    // ======================
    // SFX
    // ======================

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    // ======================
    // ANIMATION EVENTS
    // ======================

    // Diese Methode kann direkt von Animation Events aufgerufen werden
    public void PlayAnimationSFX(AudioClip clip)
    {
        PlaySFX(clip);
    }

    
}
