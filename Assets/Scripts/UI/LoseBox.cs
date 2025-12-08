using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoseBox : MonoBehaviour
{
    private BoxUI boxUI;
    private AudioManager audioManager;
    private LanguageManager languageManager;
    [SerializeField] private TMP_Text tileBox;

    [Header("Countdown")]
    [SerializeField] private TMP_Text countdownText; // gán trong Inspector
    [SerializeField] private float countdownTime = 5f; // 5 giây
    private float currentTime;

    [Header("Buttons")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button homeButton;

    private bool isFirstShow = true;

    void Awake()
    {
        gameObject.SetActive(true);
        gameObject.GetComponent<CanvasGroup>().alpha = 0;
    }

    void Start()
    {
        audioManager = AudioManager.Instance;
        boxUI = GetComponent<BoxUI>();

        languageManager = LanguageManager.Instance;
        SetLanguageOfText();

        continueButton.transform.localPosition = new Vector3(0, -170, 0);
        continueButton.gameObject.SetActive(true);

        retryButton.transform.localPosition = new Vector3(154, -170, 0);
        retryButton.transform.localScale = Vector3.zero;
        retryButton.gameObject.SetActive(false);

        homeButton.transform.localPosition = new Vector3(-184.7f, -170, 0);
        homeButton.transform.localScale = Vector3.zero;
        homeButton.gameObject.SetActive(false);

        continueButton.onClick.AddListener(() =>
        {
            audioManager.PlaySFXButton();
            boxUI.HideBox();

            retryButton.transform.localScale = Vector3.zero;
            homeButton.transform.localScale = Vector3.zero;

            FindObjectOfType<LevelManager>().ContinueLevelWithPoint();
        });

        homeButton.onClick.AddListener(() =>
        {
            audioManager.PlaySFXButton();
            boxUI.HideBox();
            FindObjectOfType<LevelManager>().ChangeToHomeScene();
        });

        retryButton.onClick.AddListener(() =>
        {
            audioManager.PlaySFXButton();
            boxUI.HideBox();

            FindObjectOfType<LevelManager>().RetryLevel();
        });
    }

    private void SetLanguageOfText()
    {
        TMP_Text continueButtonText = continueButton.GetComponentInChildren<TMP_Text>();
        TMP_Text retryButtonText = retryButton.GetComponentInChildren<TMP_Text>();
        TMP_Text homeButtonText = homeButton.GetComponentInChildren<TMP_Text>();

        tileBox.fontSize = 60f;

        switch (languageManager.currentLanguage)
        {
            case "English":
                languageManager.DisplayEnglishText(tileBox, "Fail");
                languageManager.DisplayEnglishText(homeButtonText, "Home");
                languageManager.DisplayEnglishText(continueButtonText, "Continue");
                languageManager.DisplayEnglishText(retryButtonText, "Retry");
                break;
            case "Vietnamese":
                languageManager.DisplayEnglishText(tileBox, "Thất bại");
                languageManager.DisplayEnglishText(homeButtonText, "Trở về");
                languageManager.DisplayEnglishText(continueButtonText, "Tiếp tục");
                languageManager.DisplayEnglishText(retryButtonText, "Thử lại");
                break;
            case "Chinese":
                languageManager.DisplayChineseText(tileBox, "失败");
                languageManager.DisplayChineseText(homeButtonText, "返回");
                languageManager.DisplayChineseText(continueButtonText, "继续");
                languageManager.DisplayChineseText(retryButtonText, "重试");
                break;
            case "Japanese":
                languageManager.DisplayJapaneseText(tileBox, "失敗");
                languageManager.DisplayJapaneseText(homeButtonText, "ホーム");
                languageManager.DisplayJapaneseText(continueButtonText, "続ける");
                languageManager.DisplayJapaneseText(retryButtonText, "リトライ");
                break;
            case "Korean":
                languageManager.DisplayKoreanText(tileBox, "실패");
                languageManager.DisplayKoreanText(homeButtonText, "홈");
                languageManager.DisplayKoreanText(continueButtonText, "계속하기");
                languageManager.DisplayKoreanText(retryButtonText, "재시도");
                break;
            case "Spanish":
                retryButtonText.fontSize = 47f;
                continueButtonText.fontSize = 47f;
                homeButtonText.fontSize = 43f;
                languageManager.DisplayEnglishText(tileBox, "Fallo");
                languageManager.DisplayEnglishText(homeButtonText, "Inicio");
                languageManager.DisplayEnglishText(continueButtonText, "Continuar");
                languageManager.DisplayEnglishText(retryButtonText, "Reintentar");
                break;
            case "Portuguese":
                retryButtonText.fontSize = 47f;
                continueButtonText.fontSize = 47f;
                homeButtonText.fontSize = 43f;
                languageManager.DisplayEnglishText(tileBox, "Falha");
                languageManager.DisplayEnglishText(homeButtonText, "Início");
                languageManager.DisplayEnglishText(continueButtonText, "Continuar");
                languageManager.DisplayEnglishText(retryButtonText, "Repetir");
                break;
            case "French":
                retryButtonText.fontSize = 47f;
                continueButtonText.fontSize = 47f;
                homeButtonText.fontSize = 43f;
                languageManager.DisplayEnglishText(tileBox, "Échec");
                languageManager.DisplayEnglishText(homeButtonText, "Accueil");
                languageManager.DisplayEnglishText(continueButtonText, "Continuer");
                languageManager.DisplayEnglishText(retryButtonText, "Rejouer");
                break;
            case "German":
                tileBox.fontSize = 45f;
                retryButtonText.fontSize = 47f;
                continueButtonText.fontSize = 47f;
                homeButtonText.fontSize = 43f;
                languageManager.DisplayEnglishText(tileBox, "Fehlgeschlagen");
                languageManager.DisplayEnglishText(homeButtonText, "Startseite");
                languageManager.DisplayEnglishText(continueButtonText, "Fortsetzen");
                languageManager.DisplayEnglishText(retryButtonText, "Wiederholen");
                break;
            case "Russian":
                retryButtonText.fontSize = 47f;
                continueButtonText.fontSize = 47f;
                homeButtonText.fontSize = 43f;
                languageManager.DisplayEnglishText(tileBox, "Провал");
                languageManager.DisplayEnglishText(homeButtonText, "Домой");
                languageManager.DisplayEnglishText(continueButtonText, "Продолжить");
                languageManager.DisplayEnglishText(retryButtonText, "Повторить");
                break;
            case "Thai":
                languageManager.DisplayThaiText(tileBox, "ล้มเหลว");
                languageManager.DisplayThaiText(homeButtonText, "หน้าหลัก");
                languageManager.DisplayThaiText(continueButtonText, "ดำเนินต่อ");
                languageManager.DisplayThaiText(retryButtonText, "ลองอีกครั้ง");
                break;
            default:
                languageManager.DisplayEnglishText(tileBox, "Fail");
                languageManager.DisplayEnglishText(homeButtonText, "Home");
                languageManager.DisplayEnglishText(continueButtonText, "Continue");
                languageManager.DisplayEnglishText(retryButtonText, "Retry");
                break;
        }
    }

    void OnEnable()
    {
        if (audioManager == null)
            audioManager = AudioManager.Instance;

        audioManager.PlaySFXOpenBox();
        audioManager.VibrateClassic();

        if (languageManager == null)
            languageManager = LanguageManager.Instance;

        SetLanguageOfText();

        if (isFirstShow)
        {
            isFirstShow = false;
            StartCountdown();
        }
        else
        {
            ShowButtonsMinusContinue();
        }
    }

    public void StartCountdown()
    {
        currentTime = countdownTime;
        countdownText.gameObject.SetActive(true);
        continueButton.gameObject.SetActive(true);
        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        while (currentTime > 0)
        {
            countdownText.text = Mathf.Ceil(currentTime).ToString();
            yield return new WaitForSeconds(1f);
            currentTime -= 1f;
        }

        countdownText.text = "0";

        // Ẩn text hoặc gọi sự kiện sau khi đếm xong
        yield return new WaitForSeconds(1f);
        ShowButtons();
    }

    private void ShowButtons()
    {
        retryButton.gameObject.SetActive(true);
        homeButton.gameObject.SetActive(true);

        // Hiển thị các nút ở đây
        Sequence seq = DOTween.Sequence();

        seq.Append(countdownText.transform.DOScale(0, 0.2f).SetEase(Ease.InBack));
        seq.Append(continueButton.transform.DOMoveY(0, 0.2f).SetEase(Ease.OutBack));
        seq.Append(retryButton.transform.DOScale(1, 0.2f).SetEase(Ease.OutBack));
        seq.Join(homeButton.transform.DOScale(1, 0.2f).SetEase(Ease.OutBack));

        seq.Play();
    }

    private void ShowButtonsMinusContinue()
    {
        countdownText.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
        retryButton.gameObject.SetActive(true);
        homeButton.gameObject.SetActive(true);

        // Hiển thị các nút ở đây
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.5f);
        seq.Append(retryButton.transform.DOScale(1, 0.2f).SetEase(Ease.OutBack));
        seq.Join(homeButton.transform.DOScale(1, 0.2f).SetEase(Ease.OutBack));
        seq.Append(retryButton.transform.DOMoveX(0, 0.2f).SetEase(Ease.OutBack));
        seq.Join(homeButton.transform.DOMoveX(0, 0.2f).SetEase(Ease.OutBack));
        seq.Append(retryButton.transform.DOMoveY(0, 0.2f).SetEase(Ease.OutBack));

        seq.Play();
    }
}
