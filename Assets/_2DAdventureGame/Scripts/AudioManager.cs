using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Джерела")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Музика")]
    public AudioClip backgroundMusic;
    [Range(0f, 1f)] public float musicVolume = 1f;

    [Header("Звуки")]
    public AudioClip collectible;
    public AudioClip enemyFix;
    public AudioClip enemyHit;
    public AudioClip enemyHit2;
    public AudioClip enemyWalk;
    public AudioClip playerWalk;
    public AudioClip projectile;
    public AudioClip questComplete;

    [Header("Гучність окремих ефектів")]
    [Range(0f, 1f)] public float collectibleVolume = 1f;
    [Range(0f, 1f)] public float enemyFixVolume = 1f;
    [Range(0f, 1f)] public float enemyHitVolume = 1f;
    [Range(0f, 1f)] public float enemyHit2Volume = 1f;
    [Range(0f, 1f)] public float enemyWalkVolume = 1f;
    [Range(0f, 1f)] public float playerHitVolume = 1f;
    [Range(0f, 1f)] public float playerWalkVolume = 1f;
    [Range(0f, 1f)] public float projectileVolume = 1f;
    [Range(0f, 1f)] public float questCompleteVolume = 1f;

    private void Start()
    {
        if (backgroundMusic != null && musicSource != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.volume = musicVolume;
            musicSource.loop = true; 
            musicSource.Play();
        }
    }

    private void OnValidate()
    {
        musicVolume = Mathf.Clamp01(musicVolume);
        if (Application.isPlaying && musicSource != null)
            musicSource.volume = musicVolume;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        AudioSource sceneMusicSource = musicSource;
        if (sfxSource == null || sfxSource == sceneMusicSource)
        {
            sfxSource = sceneMusicSource != null
                ? sceneMusicSource
                : GetComponent<AudioSource>();
        }

        if (sfxSource == null)
            sfxSource = gameObject.AddComponent<AudioSource>();

        sfxSource.volume = 1f;
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
        sfxSource.spatialBlend = 0f;

        GameObject musicPlaybackObject = new GameObject("Music Playback");
        musicPlaybackObject.transform.SetParent(transform, false);
        musicSource = musicPlaybackObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.spatialBlend = 0f;
        musicSource.loop = true;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, Mathf.Clamp01(volume));
        }
    }
}
