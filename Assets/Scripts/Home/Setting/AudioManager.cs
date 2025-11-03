using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [HideInInspector] public static AudioManager Instance;

    [HideInInspector] public bool musicOn = true;
    [HideInInspector] public bool sfxOn = true;
    [HideInInspector] public bool vibrationOn = true;



    [Header("Audio")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Sounds Background")]
    [SerializeField] private AudioClip musicHome1Sound;
    [SerializeField] private AudioClip musicHome2Sound;


    [Header("Sounds UI")]
    [SerializeField] private AudioClip popSFXSound;
    [SerializeField] private AudioClip failClickSFXSound;
    [SerializeField] private AudioClip buttonSFXSound;
    [SerializeField] private AudioClip openBoxSFXSound;
    [SerializeField] private AudioClip addStarSFXSound;
    [SerializeField] private AudioClip finishSFXSound;
    [SerializeField] private AudioClip openGemBoxSFXSound;
    [SerializeField] private AudioClip checkMissionSound;
    [SerializeField] private AudioClip gemReceiveSound;
    [SerializeField] private AudioClip slideChangeSound;

    [Header("Sounds Tile & Animal")]
    [SerializeField] private AudioClip swapTileSFXSound;
    [SerializeField] private AudioClip rainSFXSound;
    [SerializeField] private AudioClip wildBoarSFXSound;
    [SerializeField] private AudioClip wolfSFXSound;
    [SerializeField] private AudioClip brownBearSFXSound;
    [SerializeField] private AudioClip jaguarSFXSound;
    [SerializeField] private AudioClip elephantSFXSound;
    [SerializeField] private AudioClip beeSFXSound;
    [SerializeField] private AudioClip butterflySFXSound;



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            if (musicSource == null || sfxSource == null) GetComponentsAudioSource();
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void GetComponentsAudioSource()
    {
        foreach (var src in GetComponentsInChildren<AudioSource>())
        {
            Debug.Log(src.gameObject.name);
            if (src.gameObject.name == "Audio Music")
            {
                musicSource = src;
            }
            else if (src.gameObject.name == "Audio SFX")
            {
                sfxSource = src;
            }
        }
    }

    private void Start()
    {
        Vibration.Init();
        
        musicOn = GameData.GetMusic() == 1;
        sfxOn = GameData.GetSFX() == 1;
        vibrationOn = GameData.GetVibration() == 1;

        musicSource.mute = !musicOn;
        sfxSource.mute = !sfxOn;
    }

    public void SetVibration(bool on)
    {
        vibrationOn = on;
        GameData.SetVibration(on ? 1 : 0);
    }

    public void VibrateClassic()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (vibrationOn)
            Vibration.Vibrate();
#elif UNITY_EDITOR
        Debug.Log("Giả lập rung (Editor)");
#endif
    }

    public void VibratePop()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (vibrationOn)
            Vibration.VibratePop();
#elif UNITY_EDITOR
        Debug.Log("Giả lập rung (Editor)");
#endif
    }

    // ----- Music -----
    public void SetMusic(bool on)
    {
        musicOn = on;
        musicSource.mute = !on;
        GameData.SetMusic(on ? 1 : 0);
    }

    public void StopMusicBackground()
    {
        if (musicSource == null || sfxSource == null) GetComponentsAudioSource();

        if (!musicSource.mute && musicSource.isPlaying)
        {
            float currentVolume = musicSource.volume;
            // Dừng fade cũ (nếu đang chạy)
            musicSource.DOKill();

            // Giảm âm lượng về 0 trong 0.5 giây rồi Stop
            musicSource.DOFade(0f, 0.3f).OnComplete(() =>
            {
                musicSource.Stop();
                musicSource.volume = currentVolume; // reset lại âm lượng cho lần sau
            });
        }
    }
    
    public void PlayMusicBackground()
    {
        if (musicSource == null) GetComponentsAudioSource();

        if (musicSource.mute)
        {
            musicSource.Stop();
            return;
        }

        musicSource.Stop();
        // Gán clip và bật loop
        musicSource.clip = Random.value > 0.5f ? musicHome1Sound : musicHome2Sound;
        musicSource.loop = true;

        // Phát nhạc
        musicSource.Play();
    }

    // ----- SFX -----
    public void SetSFX(bool on)
    {
        sfxOn = on;
        sfxSource.mute = !on;
        GameData.SetSFX(on ? 1 : 0);
    }

    private void PlaySFX(AudioClip clip)
    {
        if (musicSource == null || sfxSource == null) GetComponentsAudioSource();

        if (!sfxSource.mute && clip != null)
            sfxSource.PlayOneShot(clip);
    }

    public void StopSFX()
    {
        if (musicSource == null || sfxSource == null) GetComponentsAudioSource();

        if (!sfxSource.mute && sfxSource.isPlaying)
        {
            float currentVolume = sfxSource.volume;
            // Dừng fade cũ (nếu đang chạy)
            sfxSource.DOKill();

            // Giảm âm lượng về 0 trong 0.5 giây rồi Stop
            sfxSource.DOFade(0f, 0.2f).OnComplete(() =>
            {
                sfxSource.Stop();
                sfxSource.volume = currentVolume; // reset lại âm lượng cho lần sau
            });
        }
    }

    // --- UI ---
    public void PlaySFXPop()
    {
        PlaySFX(popSFXSound);
    }

    public void PlaySFXFailClick()
    {
        PlaySFX(failClickSFXSound);
    }

    public void PlaySFXButton()
    {
        PlaySFX(buttonSFXSound);
    }

    public void PlaySFXOpenBox()
    {
        PlaySFX(openBoxSFXSound);
    }

    public void PlaySFXAddStar()
    {
        PlaySFX(addStarSFXSound);
    }

    public void PlaySFXFinish()
    {
        PlaySFX(finishSFXSound);
    }

    public void PlaySFXOpenGemBox()
    {
        PlaySFX(openGemBoxSFXSound);
    }

    public void PlaySFXCheckMission()
    {
        PlaySFX(checkMissionSound);
    }

    public void PlaySFXGemReceive()
    {
        PlaySFX(gemReceiveSound);
    }

    public void PlaySFXSlideChange()
    {
        PlaySFX(slideChangeSound);
    }

    // --- Tile ---
    public void PlaySFXSwapTile()
    {
        PlaySFX(swapTileSFXSound);
    }

    public void PlaySFXRain()
    {
        PlaySFX(rainSFXSound);
    }

    public void PlaySFXAnimalSound(AnimalType animalType)
    {
        switch(animalType)
        {
            case AnimalType.A01_WILD_BOAR:
                PlaySFX(wildBoarSFXSound);
                break;
            case AnimalType.A02_WOLF:
                PlaySFX(wolfSFXSound);
                break;
            case AnimalType.A03_BROWN_BEAR:
                PlaySFX(brownBearSFXSound);
                break;
            case AnimalType.A04_JAGUAR:
                PlaySFX(jaguarSFXSound);
                break;
            case AnimalType.A05_ELEPHANT:
                PlaySFX(elephantSFXSound);
                break;
            case AnimalType.A06_BEE:
                PlaySFX(beeSFXSound);
                break;
            case AnimalType.A07_BUTTERFLY:
                PlaySFX(butterflySFXSound);
                break;
            default:
                PlaySFXButton();
                break;
        }
    }
}
