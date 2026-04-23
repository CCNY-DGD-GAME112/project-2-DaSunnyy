using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    public AudioClip BackgroundMusic;
    public AudioClip Boing;
    public AudioClip Coin;
    public AudioClip Die;
    public AudioClip Defeat;
    public AudioClip EnemyDie;
    public AudioClip Heal;
    public AudioClip Fireball;
    public AudioClip FireballHit;
    public AudioClip Jump;
    public AudioClip Ouch;
    public AudioClip PowerUp;
    public AudioClip PowerDown;
    public AudioClip Stomp;
    public AudioClip Star;
    public AudioClip ThankYou;
    public AudioClip Victory;

    public static AudioManager Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Start()
    {
        musicSource.clip = BackgroundMusic;
        musicSource.volume = 0.5f;
        musicSource.Play();
        musicSource.loop = true;
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void StopBGM()
    {
        if (musicSource.isPlaying)
            musicSource.Stop();
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;
        musicSource.clip = clip;
        musicSource.Play();
        musicSource.loop = true;
    }
}