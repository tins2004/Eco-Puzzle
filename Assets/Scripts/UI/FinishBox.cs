using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FinishBox : MonoBehaviour
{
    private BoxUI boxUI;
    private AudioManager audioManager;

    [SerializeField] private Transform starsParent;
    [SerializeField] private TMP_Text tileBox;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button homeButton;
    [SerializeField] private Sprite starEmpty;
    [SerializeField] private Sprite starFilled;

    [Header("Gem Box")]
    [SerializeField] private GemBox gemBox;

    void Awake()
    {
        gameObject.SetActive(true);
        gameObject.GetComponent<CanvasGroup>().alpha = 0;
    }

    void Start()
    {
        audioManager = AudioManager.Instance;
        boxUI = GetComponent<BoxUI>();

        SetLanguageOfText();

        nextLevelButton.onClick.AddListener(() =>
        {
            audioManager.PlaySFXButton();
            boxUI.HideBox();

            // FindObjectOfType<LevelManager>().NextLevel();
            gemBox.ShowGem(false, 2);
        });

        homeButton.onClick.AddListener(() =>
        {
            audioManager.PlaySFXButton();
            boxUI.HideBox();

            // sceneTransition.OpenEffect("Home Scene");
            // FindObjectOfType<LevelManager>().ChangeToHomeScene();
            gemBox.ShowGem(false, 0);
        });

        for (int i = 0; i < starsParent.childCount; i++)
        {
            var star = starsParent.GetChild(i);
            var img = star.GetComponent<Image>();

            // Đặt màu xám ban đầu
            img.sprite = starEmpty;
            star.localScale = Vector3.one; // reset scale
        }
    }

    private void SetLanguageOfText()
    {
        TMP_Text nextLevelButtonText = nextLevelButton.GetComponentInChildren<TMP_Text>();
        TMP_Text homeButtonText = homeButton.GetComponentInChildren<TMP_Text>();

        tileBox.fontSize = 60f;

        LanguageManager languageManager = LanguageManager.Instance;

        switch (languageManager.currentLanguage)
        {
            case "English":
                languageManager.DisplayEnglishText(tileBox, "Complete");
                languageManager.DisplayEnglishText(homeButtonText, "Home");
                languageManager.DisplayEnglishText(nextLevelButtonText, "Next");
                break;
            case "Vietnamese":
                languageManager.DisplayEnglishText(tileBox, "Qua màn");
                languageManager.DisplayEnglishText(homeButtonText, "Trở về");
                languageManager.DisplayEnglishText(nextLevelButtonText, "Tiếp");
                break;
            case "Chinese":
                languageManager.DisplayChineseText(tileBox, "通关");
                languageManager.DisplayChineseText(homeButtonText, "返回");
                languageManager.DisplayChineseText(nextLevelButtonText, "下一步");
                break;
            case "Japanese":
                languageManager.DisplayJapaneseText(tileBox, "返回");
                languageManager.DisplayJapaneseText(homeButtonText, "ホーム");
                languageManager.DisplayJapaneseText(nextLevelButtonText, "次へ");
                break;
            case "Korean":
                languageManager.DisplayKoreanText(tileBox, "클리어");
                languageManager.DisplayKoreanText(homeButtonText, "홈");
                languageManager.DisplayKoreanText(nextLevelButtonText, "다음");
                break;
            case "Spanish":
                languageManager.DisplayEnglishText(tileBox, "Completado");
                languageManager.DisplayEnglishText(homeButtonText, "Inicio");
                languageManager.DisplayEnglishText(nextLevelButtonText, "Siguiente");
                break;
            case "Portuguese":
                languageManager.DisplayEnglishText(tileBox, "Concluído");
                languageManager.DisplayEnglishText(homeButtonText, "Início");
                languageManager.DisplayEnglishText(nextLevelButtonText, "Próximo");
                break;
            case "French":
                languageManager.DisplayEnglishText(tileBox, "Terminé");
                languageManager.DisplayEnglishText(homeButtonText, "Accueil");
                languageManager.DisplayEnglishText(nextLevelButtonText, "Suivant");
                break;
            case "German":
                tileBox.fontSize = 45f;
                languageManager.DisplayEnglishText(tileBox, "Abgeschlossen");
                languageManager.DisplayEnglishText(homeButtonText, "Startseite");
                languageManager.DisplayEnglishText(nextLevelButtonText, "Weiter");
                break;
            case "Russian":
                languageManager.DisplayEnglishText(tileBox, "Пройдено");
                languageManager.DisplayEnglishText(homeButtonText, "Домой");
                languageManager.DisplayEnglishText(nextLevelButtonText, "Далее");
                break;
            case "Thai":
                languageManager.DisplayThaiText(tileBox, "ผ่านด่าน");
                languageManager.DisplayThaiText(homeButtonText, "หน้าหลัก");
                languageManager.DisplayThaiText(nextLevelButtonText, "ถัดไป");
                break;
            default:
                languageManager.DisplayEnglishText(tileBox, "Complete");
                languageManager.DisplayEnglishText(homeButtonText, "Home");
                languageManager.DisplayEnglishText(nextLevelButtonText, "Next");
                break;
        }
    }

    private void OnEnable()
    {
        homeButton.transform.localScale = Vector3.zero;
        nextLevelButton.transform.localScale = Vector3.zero;

        if (audioManager == null)
            audioManager = AudioManager.Instance;

        audioManager.PlaySFXOpenBox();
        audioManager.VibrateClassic();

        SetLanguageOfText();

        int stars = GameData.GetStars(GameData.GetCurrentLevel());
        // Debug.Log("Stars collected: " + stars);

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.7f); // delay trước khi bắt đầu hiệu ứng

        for (int i = 0; i < stars; i++)
        {
            if (i < starsParent.childCount)
            {
                var star = starsParent.GetChild(i);
                var img = star.GetComponent<Image>();

                // Sequence để kết hợp nhiều tween
                seq.AppendInterval(0.3f); // delay giữa các sao
                seq.Join(star.DOPunchScale(Vector3.one * 0.5f, 0.3f, 8, 1)); // hiệu ứng nhún nhẹ
                seq.JoinCallback(() => img.sprite = starFilled);
                seq.JoinCallback(() => audioManager.PlaySFXAddStar());
            }
        }

        seq.Append(nextLevelButton.transform.DOScale(1, 0.2f).SetEase(Ease.OutBack));
        seq.Join(homeButton.transform.DOScale(1, 0.2f).SetEase(Ease.OutBack));
        seq.JoinCallback(() => audioManager.PlaySFXFinish());
    }
}
