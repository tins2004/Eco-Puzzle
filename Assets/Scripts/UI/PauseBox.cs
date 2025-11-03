using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseBox : MonoBehaviour
{
    private BoxUI boxUI;
    private AudioManager audioManager;
    private LanguageManager languageManager;
    [SerializeField] private TMP_Text tileBox;
    [SerializeField] private CanvasGroup mainCanvasGroup;
    [SerializeField] public SceneTransition sceneTransition;

    [Header("Buttom")]
    [SerializeField] private Button musicButton;
    [SerializeField] private Button sfxButton;
    [SerializeField] private Button vibrationButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button homeButton;

    [Header("Sprite")]
    [SerializeField] private Sprite musicIconOn;
    [SerializeField] private Sprite musicIconOff;
    [SerializeField] private Sprite sfxIconOn;
    [SerializeField] private Sprite sfxIconOff;
    [SerializeField] private Sprite vibrationIconOn;
    [SerializeField] private Sprite vibrationIconOff;

    [Header("Gem Box")]
    [SerializeField] private GemBox gemBox;

    void Awake()
    {
        gameObject.SetActive(true);
        gameObject.GetComponent<CanvasGroup>().alpha = 0;
    }

    void Start()
    {
        boxUI = GetComponent<BoxUI>();

        // --- Audio Button ---
        musicButton.image.sprite = audioManager.musicOn == true ? musicIconOn : musicIconOff;
        sfxButton.image.sprite = audioManager.sfxOn == true ? sfxIconOn : sfxIconOff;
        vibrationButton.image.sprite = audioManager.vibrationOn == true ? vibrationIconOn : vibrationIconOff;

        musicButton.onClick.AddListener(() =>
        {
            audioManager.SetMusic(!audioManager.musicOn);
            musicButton.image.sprite = audioManager.musicOn == true ? musicIconOn : musicIconOff;
            audioManager.PlaySFXButton();
            audioManager.PlayMusicBackground();
        });

        sfxButton.onClick.AddListener(() =>
        {
            audioManager.SetSFX(!audioManager.sfxOn);
            sfxButton.image.sprite = audioManager.sfxOn == true ? sfxIconOn : sfxIconOff;
            audioManager.PlaySFXButton();
        });

        vibrationButton.onClick.AddListener(() =>
        {
            audioManager.SetVibration(!audioManager.vibrationOn);
            vibrationButton.image.sprite = audioManager.vibrationOn == true ? vibrationIconOn : vibrationIconOff;
            audioManager.PlaySFXButton();
            audioManager.VibrateClassic();
        });

        // --- System Button ---
        continueButton.onClick.AddListener(() =>
        {
            audioManager.PlaySFXButton();
            boxUI.HideBox();
            mainCanvasGroup.DOFade(1, 0.3f);
        });

        homeButton.onClick.AddListener(() =>
        {
            audioManager.PlaySFXButton();
            boxUI.HideBox();

            // FindObjectOfType<LevelManager>().ChangeToHomeScene();
            gemBox.ShowGem(true, 0);
        });
    }

    private void OnEnable()
    {
        if (audioManager == null)
            audioManager = AudioManager.Instance;

        audioManager.PlaySFXOpenBox();
        audioManager.VibrateClassic();

        if (languageManager == null)
            languageManager = LanguageManager.Instance;

        SetLanguageOfText();
    }

    private void SetLanguageOfText()
    {
        TMP_Text continueButtonText = continueButton.GetComponentInChildren<TMP_Text>();
        TMP_Text homeButtonText = homeButton.GetComponentInChildren<TMP_Text>();

        tileBox.fontSize = 60f;

        switch (languageManager.currentLanguage)
        {
            case "English":
                languageManager.DisplayEnglishText(tileBox, "Pause");
                languageManager.DisplayEnglishText(homeButtonText, "Home");
                languageManager.DisplayEnglishText(continueButtonText, "Continue");
                break;
            case "Vietnamese":
                languageManager.DisplayEnglishText(tileBox, "Tạm dừng");
                languageManager.DisplayEnglishText(homeButtonText, "Trở về");
                languageManager.DisplayEnglishText(continueButtonText, "Tiếp tục");
                break;
            case "Chinese":
                languageManager.DisplayChineseText(tileBox, "暂停");
                languageManager.DisplayChineseText(homeButtonText, "返回");
                languageManager.DisplayChineseText(continueButtonText, "继续");
                break;
            case "Japanese":
                languageManager.DisplayJapaneseText(tileBox, "ポーズ");
                languageManager.DisplayJapaneseText(homeButtonText, "ホーム");
                languageManager.DisplayJapaneseText(continueButtonText, "続ける");
                break;
            case "Korean":
                languageManager.DisplayKoreanText(tileBox, "일시정지");
                languageManager.DisplayKoreanText(homeButtonText, "홈");
                languageManager.DisplayKoreanText(continueButtonText, "계속하기");
                break;
            case "Spanish":
                continueButtonText.fontSize = 47f;
                homeButtonText.fontSize = 43f;
                languageManager.DisplayEnglishText(tileBox, "Pausa");
                languageManager.DisplayEnglishText(homeButtonText, "Inicio");
                languageManager.DisplayEnglishText(continueButtonText, "Continuar");
                break;
            case "Portuguese":
                continueButtonText.fontSize = 47f;
                homeButtonText.fontSize = 43f;
                languageManager.DisplayEnglishText(tileBox, "Pausar");
                languageManager.DisplayEnglishText(homeButtonText, "Início");
                languageManager.DisplayEnglishText(continueButtonText, "Continuar");
                break;
            case "French":
                continueButtonText.fontSize = 47f;
                homeButtonText.fontSize = 43f;
                languageManager.DisplayEnglishText(tileBox, "Pause");
                languageManager.DisplayEnglishText(homeButtonText, "Accueil");
                languageManager.DisplayEnglishText(continueButtonText, "Continuer");
                break;
            case "German":
                continueButtonText.fontSize = 47f;
                homeButtonText.fontSize = 43f;
                languageManager.DisplayEnglishText(tileBox, "Pause");
                languageManager.DisplayEnglishText(homeButtonText, "Startseite");
                languageManager.DisplayEnglishText(continueButtonText, "Fortsetzen");
                break;
            case "Russian":
                continueButtonText.fontSize = 47f;
                homeButtonText.fontSize = 43f;
                languageManager.DisplayEnglishText(tileBox, "Пауза");
                languageManager.DisplayEnglishText(homeButtonText, "Домой");
                languageManager.DisplayEnglishText(continueButtonText, "Продолжить");
                break;
            case "Thai":
                languageManager.DisplayThaiText(tileBox, "หยุดชั่วคราว");
                languageManager.DisplayThaiText(homeButtonText, "หน้าหลัก");
                languageManager.DisplayThaiText(continueButtonText, "ดำเนินต่อ");
                break;
            default:
                languageManager.DisplayEnglishText(tileBox, "Pause");
                languageManager.DisplayEnglishText(homeButtonText, "Home");
                languageManager.DisplayEnglishText(continueButtonText, "Continue");
                break;
        }
    }

}
