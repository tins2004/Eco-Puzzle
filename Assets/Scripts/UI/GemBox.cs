using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GemBox : MonoBehaviour
{
    private BoxUI boxUI;
    private AudioManager audioManager;
    private LanguageManager languageManager;

    [Header("Gem")]
    [SerializeField] private Image gemIcon;
    [SerializeField] private TMP_Text gemText;

    [Header("Button")]
    [SerializeField] private Button getRewardButton;
    [SerializeField] private Button getRewardX2Button;

    [Header("Countdown")]
    private float countdownTime = 3f;
    private float currentTime;


    void Awake()
    {
        gameObject.SetActive(true);
        gameObject.GetComponent<CanvasGroup>().alpha = 0;
    }

    void Start()
    {
        SetUp();
    }
    
    void SetUp()
    {
        boxUI = GetComponent<BoxUI>();

        gemIcon.transform.localScale = Vector3.zero;
        audioManager = AudioManager.Instance;
        languageManager = LanguageManager.Instance;
    }


    public void ShowGem(bool isMinus, bool sceneChangeIsHome, int valueChange = 10)
    {
        if (boxUI == null) SetUp();
        boxUI.ShowBox();

        if (audioManager == null)
            audioManager = AudioManager.Instance;
        
        SetLanguageForGemBox();

        audioManager.PlaySFXOpenGemBox();

        gemIcon.gameObject.SetActive(true);
        gemIcon.transform.localScale = Vector3.zero;
        gemText.transform.localScale = Vector3.one;
        getRewardButton.transform.localScale = Vector3.zero;
        getRewardX2Button.transform.localScale = Vector3.zero;

        getRewardButton.onClick.RemoveAllListeners();
        getRewardX2Button.onClick.RemoveAllListeners();

        // int valueChange = 10;
        // Debug.Log("Giá trị thay đổi: " + valueChange);
        gemText.text = "0";

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.5f);
        seq.Append(gemIcon.transform.DOScale(1, 0.2f).SetEase(Ease.OutBack));

        seq.AppendInterval(0.5f);
        if (isMinus)
        {
            // valueChange = Random.Range(10, 30);
            // seq.AppendCallback(() =>
            // DOTween.To(() => 0, x =>
            // {
            //     valueChange = x;
            //     gemText.text = "-" + valueChange.ToString();
            // }, valueChange, 0.8f)
            // .SetEase(Ease.InQuad));
            gemText.text = valueChange.ToString();
        }
        else
        {
            // valueChange = Random.Range(4, 18);
            int randomValue = Random.Range(4, 18);
            valueChange = randomValue;
            seq.AppendCallback(() =>
            DOTween.To(() => 0, x =>
            {
                randomValue = x;
                gemText.text = "+" + randomValue.ToString();
            }, randomValue, 0.8f)
            .SetEase(Ease.InQuad));
        }

        seq.Append(getRewardX2Button.transform.DOScale(1, 0.2f).SetEase(Ease.OutBack));
        seq.AppendCallback(() =>
        {
            SetUpButton(isMinus, valueChange, sceneChangeIsHome);

            currentTime = countdownTime;
            StartCoroutine(CountdownRoutine());
        });
        seq.Play();

    }

    private IEnumerator CountdownRoutine()
    {
        while (currentTime > 0)
        {
            yield return new WaitForSeconds(1f);
            currentTime -= 1f;
        }

        // Ẩn text hoặc gọi sự kiện sau khi đếm xong
        yield return new WaitForSeconds(1f);

        getRewardButton.transform.DOScale(1, 0.2f).SetEase(Ease.OutBack);
    }

    private void SetUpButton(bool isMinus, int valueChange, bool sceneChangeIsHome)
    {

        getRewardButton.onClick.AddListener(() =>
        {
            audioManager.PlaySFXButton();
            if (isMinus == true)
            {
                GameData.MinusGem(valueChange);
            }
            else {
                GameData.AddGem(valueChange);
                Debug.Log("Nhận được gem: " + valueChange);
            }

            CloseBox(sceneChangeIsHome);
        });


        getRewardX2Button.onClick.AddListener(() =>
        {
            audioManager.PlaySFXButton();
            if (isMinus == true)
            {
                GameData.MinusGem(valueChange);
            }
            else {
                GameData.AddGem(valueChange * 2);
                gemText.text = "+" + (valueChange * 2).ToString();
                Debug.Log("Nhận được gem x2: " + (valueChange * 2));
            }

            CloseBox(sceneChangeIsHome);
        });
    }

    private void CloseBox(bool sceneChangeIsHome = false)
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.2f);
        seq.Append(gemText.transform.DOPunchScale(Vector3.one * 0.5f, 0.3f, 8, 1));
        seq.JoinCallback(() => audioManager.PlaySFXGemReceive());
        seq.JoinCallback(() => audioManager.VibrateClassic());

        seq.AppendInterval(0.3f);
        seq.Append(gemText.transform.DOScale(0, 0.2f).SetEase(Ease.InBack));
        seq.JoinCallback(() => audioManager.PlaySFXFailClick());
        seq.Append(gemIcon.transform.DOScale(0, 0.2f).SetEase(Ease.InBack));
        seq.JoinCallback(() => audioManager.PlaySFXFailClick());
        seq.Join(getRewardButton.transform.DOScale(0, 0.2f).SetEase(Ease.InBack));
        seq.Join(getRewardX2Button.transform.DOScale(0, 0.2f).SetEase(Ease.InBack));
        seq.AppendCallback(() =>
        {
            boxUI.HideBox();
            LevelManager levelManager = FindObjectOfType<LevelManager>();

            if (sceneChangeIsHome) 
                levelManager.ChangeToHomeScene();
            else
                levelManager.NextLevel();
        });


        seq.Play();
    }

    // --- Booster ---
    public void ShowBoxBuy(int value, BoosterBase booster)
    {
        if (boxUI == null) SetUp();
        boxUI.ShowBox();

        if (audioManager == null)
            audioManager = AudioManager.Instance;

        SetLanguageForBuyBox();

        audioManager.PlaySFXOpenGemBox();

        gemIcon.gameObject.SetActive(true);
        gemIcon.transform.localScale = Vector3.zero;
        gemText.transform.localScale = Vector3.one;
        getRewardButton.transform.localScale = Vector3.zero;
        getRewardX2Button.transform.localScale = Vector3.zero;

        getRewardButton.onClick.RemoveAllListeners();
        getRewardX2Button.onClick.RemoveAllListeners();

        // int valueChange = 10;
        // Debug.Log("Giá trị thay đổi: " + valueChange);
        gemText.text = value.ToString();

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.5f);
        seq.Append(gemIcon.transform.DOScale(1, 0.2f).SetEase(Ease.OutBack));

        seq.Append(getRewardX2Button.transform.DOScale(1, 0.2f).SetEase(Ease.OutBack));
        seq.Append(getRewardButton.transform.DOScale(1, 0.2f).SetEase(Ease.OutBack));
        seq.AppendCallback(() =>
        {
            SetUpBuyButton(value, booster);

            
        });
        seq.Play();
    }

    private void SetUpBuyButton(int value, BoosterBase booster)
    {

        getRewardButton.onClick.AddListener(() => // Close
        {
            audioManager.PlaySFXButton();
            CloseBuyBox();
        });

        if (GameData.GetCurrentGem() > value)
            getRewardX2Button.onClick.AddListener(() => // Buy
            {
                audioManager.PlaySFXButton();

                GameData.MinusGem(value);
                FindObjectOfType<UIManager>().UpdateGemUI();

                booster.AddBoosterNumber(1);

                CloseBuyBox();
            });
        else
        {
            getRewardX2Button.GetComponent<Image>().color = Color.gray;
            
            getRewardX2Button.onClick.AddListener(() => // Buy
            {
                audioManager.PlaySFXButton();
            });
        }
    }

    private void CloseBuyBox()
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.3f);
        seq.Append(gemText.transform.DOScale(0, 0.2f).SetEase(Ease.InBack));
        seq.JoinCallback(() => audioManager.PlaySFXFailClick());
        seq.Append(gemIcon.transform.DOScale(0, 0.2f).SetEase(Ease.InBack));
        seq.JoinCallback(() => audioManager.PlaySFXFailClick());
        seq.Join(getRewardButton.transform.DOScale(0, 0.2f).SetEase(Ease.InBack));
        seq.Join(getRewardX2Button.transform.DOScale(0, 0.2f).SetEase(Ease.InBack));
        seq.AppendCallback(() =>
        {
            getRewardX2Button.GetComponent<Image>().color = Color.white;
    
            boxUI.HideBox();
        });


        seq.Play();
    }

    // --- Language ---
    private void SetLanguageForGemBox()
    {
        if (getRewardButton == null || getRewardX2Button == null) return;

        TMP_Text rewardText = getRewardButton.GetComponentInChildren<TMP_Text>();
        TMP_Text rewardX2Text = getRewardX2Button.GetComponentInChildren<TMP_Text>();
        
        if (languageManager == null)
            languageManager = LanguageManager.Instance;

        switch (languageManager.currentLanguage)
        {
            case "English":
                languageManager.DisplayEnglishText(rewardText, "Claim & Continue");
                languageManager.DisplayEnglishText(rewardX2Text, "Claim x2");
                break;

            case "Vietnamese":
                languageManager.DisplayEnglishText(rewardText, "Nhận và tiếp tục");
                languageManager.DisplayEnglishText(rewardX2Text, "Nhận x2");
                break;

            case "Chinese":
                languageManager.DisplayChineseText(rewardText, "获得并继续");
                languageManager.DisplayChineseText(rewardX2Text, "获得 x2");
                break;

            case "Japanese":
                languageManager.DisplayJapaneseText(rewardText, "受け取って続行");
                languageManager.DisplayJapaneseText(rewardX2Text, "2倍受け取る");
                break;

            case "Korean":
                languageManager.DisplayKoreanText(rewardText, "받기 및 계속");
                languageManager.DisplayKoreanText(rewardX2Text, "2배 보상 받기");
                break;

            case "Spanish":
                languageManager.DisplayEnglishText(rewardText, "Obtener y continuar");
                languageManager.DisplayEnglishText(rewardX2Text, "Obtener x2");
                break;

            case "Portuguese":
                languageManager.DisplayEnglishText(rewardText, "Receber e continuar");
                languageManager.DisplayEnglishText(rewardX2Text, "Receber x2");
                break;

            case "French":
                languageManager.DisplayEnglishText(rewardText, "Obtenir et continuer");
                languageManager.DisplayEnglishText(rewardX2Text, "Obtenir x2");
                break;

            case "German":
                languageManager.DisplayEnglishText(rewardText, "Erhalten und fortfahren");
                languageManager.DisplayEnglishText(rewardX2Text, "x2 erhalten");
                break;

            case "Russian":
                languageManager.DisplayEnglishText(rewardText, "Получить и продолжить");
                languageManager.DisplayEnglishText(rewardX2Text, "Получить x2");
                break;

            case "Thai":
                languageManager.DisplayThaiText(rewardText, "รับและดำเนินต่อ");
                languageManager.DisplayThaiText(rewardX2Text, "รับ x2");
                break;

            default:
                languageManager.DisplayEnglishText(rewardText, "Claim & Continue");
                languageManager.DisplayEnglishText(rewardX2Text, "Claim x2");
                break;
        }
    }

    private void SetLanguageForBuyBox()
    {
        if (getRewardButton == null || getRewardX2Button == null) return;

        TMP_Text rewardText = getRewardButton.GetComponentInChildren<TMP_Text>();
        TMP_Text rewardX2Text = getRewardX2Button.GetComponentInChildren<TMP_Text>();
        
        if (languageManager == null)
            languageManager = LanguageManager.Instance;

        switch (languageManager.currentLanguage)
        {
            case "English":
                languageManager.DisplayEnglishText(rewardText, "Buy");
                languageManager.DisplayEnglishText(rewardX2Text, "Exit");
                break;

            case "Vietnamese":
                languageManager.DisplayEnglishText(rewardText, "Mua");
                languageManager.DisplayEnglishText(rewardX2Text, "Thoát");
                break;

            case "Chinese":
                languageManager.DisplayChineseText(rewardText, "购买");
                languageManager.DisplayChineseText(rewardX2Text, "退出");
                break;

            case "Japanese":
                languageManager.DisplayJapaneseText(rewardText, "購入");
                languageManager.DisplayJapaneseText(rewardX2Text, "終了");
                break;

            case "Korean":
                languageManager.DisplayKoreanText(rewardText, "구매");
                languageManager.DisplayKoreanText(rewardX2Text, "종료");
                break;

            case "Spanish":
                languageManager.DisplayEnglishText(rewardText, "Comprar");
                languageManager.DisplayEnglishText(rewardX2Text, "Salir");
                break;

            case "Portuguese":
                languageManager.DisplayEnglishText(rewardText, "Comprar");
                languageManager.DisplayEnglishText(rewardX2Text, "Sair");
                break;

            case "French":
                languageManager.DisplayEnglishText(rewardText, "Acheter");
                languageManager.DisplayEnglishText(rewardX2Text, "Quitter");
                break;

            case "German":
                languageManager.DisplayEnglishText(rewardText, "Kaufen");
                languageManager.DisplayEnglishText(rewardX2Text, "Beenden");
                break;

            case "Russian":
                languageManager.DisplayEnglishText(rewardText, "Купить");
                languageManager.DisplayEnglishText(rewardX2Text, "Выйти");
                break;

            case "Thai":
                languageManager.DisplayThaiText(rewardText, "ซื้อ");
                languageManager.DisplayThaiText(rewardX2Text, "ออกจาก");
                break;

            default:
                languageManager.DisplayEnglishText(rewardText, "Buy");
                languageManager.DisplayEnglishText(rewardX2Text, "Exit");
                break;
        }
    }
}
