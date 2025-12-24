using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HomeManager : MonoBehaviour
{
    [SerializeField] public SceneTransition sceneTransition;

    [Header("Level map")]
    [SerializeField] private Transform levelMapParent;
    [SerializeField] private Transform levelMapView;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button shopButton;
    [SerializeField] private Button battlePassButton;
    [SerializeField] private Button event7DaysButton;
    [SerializeField] private Button themeButton;
    [SerializeField] private TMP_Text[] gemText;

    [Header("Settings")]
    [SerializeField] private Transform settingsParent;
    [SerializeField] private Transform settingsView;
    [SerializeField] private Button closeSettingButton;
    [SerializeField] private Button backgroundMusicButton;
    [SerializeField] private Button musicVolumeButton;
    [SerializeField] private Button musicToggleButton;
    [SerializeField] private Button languageButton;

    [Header("Shop")]
    [SerializeField] private Transform shopParent;
    [SerializeField] private Transform shopView;
    [SerializeField] private Button closeShopButton;

    [Header("Events")]
    [SerializeField] private Transform event7DaysParent;
    [SerializeField] private Transform event7DaysView;
    [SerializeField] private Button closeEvent7DaysButton;

    [Header("Battle Pass")]
    [SerializeField] private Transform battlePassParent;
    [SerializeField] private Transform battlePassView;
    [SerializeField] private Transform vipBattlePassButton;
    [SerializeField] private Button closeBattlePassButton;

    [Header("Theme")]
    [SerializeField] private Transform themeParent;
    [SerializeField] private Transform themeView;
    [SerializeField] private Button closeThemeButton;

    [Header("Chest UI")]
    [SerializeField] private Transform rewardBackground;
    [SerializeField] private ObjectPool itemRewardPool;
    [SerializeField] private TMP_Text countdownText;
    private float countdownTime = 3f;
    private float currentTime;

    [Header("Manager Link")]
    [HideInInspector] public LanguageManager languageManager;
    [HideInInspector] public AudioManager audioManager;
    private SettingManager settingManager;
    private ThemeManager themeManager;

    [HideInInspector] public string currentDate;

    private void Awake() {
        audioManager = AudioManager.Instance;
        languageManager = LanguageManager.Instance;
    }

    public async void Start()
    {
        levelMapParent.gameObject.SetActive(false);
        settingsParent.gameObject.SetActive(false);
        event7DaysParent.gameObject.SetActive(false);
        battlePassParent.gameObject.SetActive(false);
        themeParent.gameObject.SetActive(false);
        rewardBackground.gameObject.SetActive(false);


        sceneTransition.CloseEffect();

        currentDate = await GameData.GetInternetTime();

        // ----- Setup And Open ------
        settingManager = GetComponent<SettingManager>();
        settingManager.SetupSettingStatus();

        StartHomeScene();
        audioManager.PlayMusicBackground();

        // ----- Button listeners -----
        // --- Settings ---
        settingsButton.onClick.AddListener(() =>
        {
            audioManager.PlaySFXButton();
            CloseLevelMap(0f);
            OpenSettings(0.7f);
        });
        closeSettingButton.onClick.AddListener(() =>
        {
            audioManager.PlaySFXButton();
            CloseSettings(0f);
            OpenLevelMap(0.7f);
        });

        // --- Shop ---
        shopButton.onClick.AddListener(() =>
        {
            audioManager.PlaySFXButton();
            CloseLevelMap(0f);
            OpenShop(0.7f);
        });
        closeShopButton.onClick.AddListener(() =>
        {
            audioManager.PlaySFXButton();
            CloseShop(0f);
            OpenLevelMap(0.7f);
        });

        // --- Events ---
        event7DaysButton.onClick.AddListener(() =>
        {
            audioManager.PlaySFXButton();
            CloseLevelMap(0f);
            OpenEvent7Days(0.7f);
        });
        closeEvent7DaysButton.onClick.AddListener(() =>
        {
            audioManager.PlaySFXButton();
            CloseEvent7Days(0f);
            if (GameData.GetCurrentScene() == "Begin Scene")
            {
                GameData.SetCurrentScene("Home Scene");
                OpenBattlePass(0.7f);
            }
            else
                OpenLevelMap(0.7f);
        });

        // --- Daily Reward ---
        battlePassButton.onClick.AddListener(() =>
        {
            audioManager.PlaySFXButton();
            CloseLevelMap(0f);
            OpenBattlePass(0.7f);
        });

        closeBattlePassButton.onClick.AddListener(() =>
        {
            audioManager.PlaySFXButton();
            CloseBattlePass(0f);
            OpenLevelMap(0.7f);
        });


        // --- Theme ---
        themeButton.onClick.AddListener(() =>
        {
            audioManager.PlaySFXButton();
            CloseLevelMap(0f);
            OpenTheme(0.7f);
        });

        closeThemeButton.onClick.AddListener(() =>
        {
            audioManager.PlaySFXButton();
            CloseTheme(0f);
            OpenLevelMap(0.7f);
        });
    }

    public void ResetData()
    {
        PlayerPrefs.DeleteAll();
    }

    // ----- Level map animations -----
    private void OpenLevelMap(float waitTime)
    {
        foreach (var text in gemText)
        {
            // text.text = GameData.GetCurrentGem().ToString() + "\n"  + FireBaseAnalytics.Instance.IsFirebaseReady()  + "\n" + currentDate;
            text.text = "[" + GameData.GetCurrentGem().ToString() + "]";
        }
        

        levelMapParent.gameObject.SetActive(true);

        levelMapView.localScale = Vector3.zero;
        battlePassButton.transform.localScale = Vector3.zero;
        event7DaysButton.transform.localScale = Vector3.zero;
        themeButton.transform.localScale = Vector3.zero;
        settingsButton.transform.localScale = Vector3.zero;
        shopButton.transform.localScale = Vector3.zero;


        Sequence seq = DOTween.Sequence();
        seq.PrependInterval(waitTime);

        seq.Append(battlePassButton.transform.DOScale(1, 0.4f).SetEase(Ease.OutBack));
        seq.JoinCallback(() => audioManager.PlaySFXPop());

        seq.Append(themeButton.transform.DOScale(1, 0.2f).SetEase(Ease.OutBack));
        seq.Join(settingsButton.transform.DOScale(1, 0.2f).SetEase(Ease.OutBack));
        seq.JoinCallback(() => audioManager.PlaySFXPop());

        seq.Append(event7DaysButton.transform.DOScale(1, 0.2f).SetEase(Ease.OutBack));
        seq.Join(shopButton.transform.DOScale(1, 0.2f).SetEase(Ease.OutBack));
        seq.JoinCallback(() => audioManager.PlaySFXPop());

        seq.Append(levelMapView.DOScale(1, 0.2f).SetEase(Ease.OutBack));
        seq.JoinCallback(() => audioManager.PlaySFXPop());

        seq.Play();
    }

    public void CloseLevelMap(float waitTime)
    {
        Sequence seq = DOTween.Sequence();
        seq.PrependInterval(waitTime);

        seq.Append(levelMapView.DOScale(0, 0.2f).SetEase(Ease.InBack));
        seq.JoinCallback(() => audioManager.PlaySFXPop());

        seq.Append(event7DaysButton.transform.DOScale(0, 0.2f).SetEase(Ease.InBack));
        seq.Join(settingsButton.transform.DOScale(0, 0.2f).SetEase(Ease.InBack));
        seq.Join(themeButton.transform.DOScale(0, 0.2f).SetEase(Ease.InBack));
        seq.Join(shopButton.transform.DOScale(0, 0.2f).SetEase(Ease.InBack));
        seq.JoinCallback(() => audioManager.PlaySFXPop());

        seq.Append(battlePassButton.transform.DOScale(0, 0.2f).SetEase(Ease.InBack));
        seq.JoinCallback(() => audioManager.PlaySFXPop());

        seq.AppendCallback(() =>
        {
            levelMapParent.gameObject.SetActive(false);
        });

        seq.Play();
    }

    // ----- Settings animations -----
    private void OpenSettings(float waitTime)
    {
        settingsParent.gameObject.SetActive(true);
        // settingsView.localScale = Vector3.zero;
        closeSettingButton.transform.localScale = Vector3.zero;
        backgroundMusicButton.transform.localScale = Vector3.zero;
        musicVolumeButton.transform.localScale = Vector3.zero;
        musicToggleButton.transform.localScale = Vector3.zero;
        languageButton.transform.localScale = Vector3.zero;


        Sequence seq = DOTween.Sequence();
        seq.PrependInterval(waitTime);

        seq.Append(closeSettingButton.transform.DOScale(1, 0.4f).SetEase(Ease.OutBack));
        seq.JoinCallback(() => audioManager.PlaySFXPop());

        seq.Append(languageButton.transform.DOScale(1, 0.2f).SetEase(Ease.OutBack));
        seq.JoinCallback(() => audioManager.PlaySFXPop());

        seq.Append(backgroundMusicButton.transform.DOScale(1, 0.2f).SetEase(Ease.OutBack));
        seq.JoinCallback(() => audioManager.PlaySFXPop());

        seq.Append(musicVolumeButton.transform.DOScale(1, 0.2f).SetEase(Ease.OutBack));
        seq.JoinCallback(() => audioManager.PlaySFXPop());

        seq.Append(musicToggleButton.transform.DOScale(1, 0.2f).SetEase(Ease.OutBack));
        seq.JoinCallback(() => audioManager.PlaySFXPop());

        // seq.Append(settingsView.DOScale(1, 0.4f).SetEase(Ease.OutBack));
        // seq.JoinCallback(() => audioManager.PlaySFXPop());

        seq.Play();
    }

    private void CloseSettings(float waitTime)
    {
        Sequence seq = DOTween.Sequence();
        seq.PrependInterval(waitTime);

        // seq.Append(settingsView.DOScale(0, 0.2f).SetEase(Ease.InBack));
        // seq.JoinCallback(() => audioManager.PlaySFXPop());

        seq.Append(languageButton.transform.DOScale(0, 0.2f).SetEase(Ease.InBack));
        seq.Join(backgroundMusicButton.transform.DOScale(0, 0.2f).SetEase(Ease.InBack));
        seq.Join(musicVolumeButton.transform.DOScale(0, 0.2f).SetEase(Ease.InBack));
        seq.Join(musicToggleButton.transform.DOScale(0, 0.2f).SetEase(Ease.InBack));
        seq.JoinCallback(() => audioManager.PlaySFXPop());

        seq.Append(closeSettingButton.transform.DOScale(0, 0.2f).SetEase(Ease.InBack));
        seq.JoinCallback(() => audioManager.PlaySFXPop());

        seq.AppendCallback(() =>
        {
            settingsParent.gameObject.SetActive(false);
        });

        seq.Play();
    }

    // ----- Shop animations -----
    private void OpenShop(float waitTime)
    {
        shopParent.gameObject.SetActive(true);
        shopView.localScale = Vector3.one;
        closeShopButton.transform.localScale = Vector3.zero;

        foreach (Transform item in shopView)
        {
            item.localScale = Vector3.zero;
        }

        Sequence seq = DOTween.Sequence();
        seq.PrependInterval(waitTime);

        seq.Append(closeShopButton.transform.DOScale(1, 0.4f).SetEase(Ease.OutBack));
        seq.JoinCallback(() => audioManager.PlaySFXPop());

        foreach (Transform item in shopView)
        {
            item.localScale = Vector3.zero;
            seq.Append(item.DOScale(1, 0.2f).SetEase(Ease.OutBack));
            seq.JoinCallback(() => audioManager.PlaySFXPop());

        }
        seq.Play();
    }

    private void CloseShop(float waitTime)
    {
        Sequence seq = DOTween.Sequence();
        seq.PrependInterval(waitTime);

        seq.Append(shopView.DOScale(0, 0.2f).SetEase(Ease.InBack));
        seq.JoinCallback(() => audioManager.PlaySFXPop());

        seq.Append(closeShopButton.transform.DOScale(0, 0.2f).SetEase(Ease.InBack));
        seq.JoinCallback(() => audioManager.PlaySFXPop());

        seq.AppendCallback(() =>
        {
            shopParent.gameObject.SetActive(false);
        });

        seq.Play();
    }

    // ----- Events animations -----
    private void OpenEvent7Days(float waitTime)
    {
        event7DaysParent.gameObject.SetActive(true);
        event7DaysView.localScale = Vector3.zero;
        closeEvent7DaysButton.transform.localScale = Vector3.zero;

        Sequence seq = DOTween.Sequence();
        seq.PrependInterval(waitTime);

        seq.Append(closeEvent7DaysButton.transform.DOScale(1, 0.4f).SetEase(Ease.OutBack));
        seq.JoinCallback(() => audioManager.PlaySFXPop());
        seq.Append(event7DaysView.DOScale(1, 0.4f).SetEase(Ease.OutBack));
        seq.JoinCallback(() => audioManager.PlaySFXPop());
        seq.Play();
    }

    private void CloseEvent7Days(float waitTime)
    {
        Sequence seq = DOTween.Sequence();
        seq.PrependInterval(waitTime);

        seq.Append(event7DaysView.DOScale(0, 0.2f).SetEase(Ease.InBack));
        seq.JoinCallback(() => audioManager.PlaySFXPop());
        seq.Append(closeEvent7DaysButton.transform.DOScale(0, 0.2f).SetEase(Ease.InBack));
        seq.JoinCallback(() => audioManager.PlaySFXPop());

        seq.AppendCallback(() =>
        {
            event7DaysParent.gameObject.SetActive(false);
        });

        seq.Play();
    }

    // ----- Battle Pass animations -----
    private void OpenBattlePass(float waitTime)
    {
        battlePassParent.gameObject.SetActive(true);
        battlePassView.localScale = Vector3.zero;
        closeBattlePassButton.transform.localScale = Vector3.zero;
        vipBattlePassButton.transform.localScale = Vector3.zero;

        Sequence seq = DOTween.Sequence();
        seq.PrependInterval(waitTime);

        seq.Append(closeBattlePassButton.transform.DOScale(1, 0.4f).SetEase(Ease.OutBack));
        seq.JoinCallback(() => audioManager.PlaySFXPop());
        if (GameData.GetVIPBattlePass() == 0)
        {
            seq.Append(vipBattlePassButton.transform.DOScale(1, 0.4f).SetEase(Ease.OutBack));
            seq.JoinCallback(() => audioManager.PlaySFXPop());
        }
        seq.Append(battlePassView.DOScale(1, 0.4f).SetEase(Ease.OutBack));
        seq.JoinCallback(() => audioManager.PlaySFXPop());

        seq.Play();
    }

    private void CloseBattlePass(float waitTime)
    {
        Sequence seq = DOTween.Sequence();
        seq.PrependInterval(waitTime);

        seq.Append(battlePassView.DOScale(0, 0.2f).SetEase(Ease.InBack));
        seq.JoinCallback(() => audioManager.PlaySFXPop());
        if (GameData.GetVIPBattlePass() == 0)
        {
            seq.Append(vipBattlePassButton.transform.DOScale(0, 0.2f).SetEase(Ease.InBack));
            seq.JoinCallback(() => audioManager.PlaySFXPop());
        }
        seq.Append(closeBattlePassButton.transform.DOScale(0, 0.2f).SetEase(Ease.InBack));
        seq.JoinCallback(() => audioManager.PlaySFXPop());

        seq.AppendCallback(() =>
        {
            battlePassParent.gameObject.SetActive(false);
        });

        seq.Play();
    }


    // ----- Theme animations -----
    private void OpenTheme(float waitTime)
    {
        themeParent.gameObject.SetActive(true);
        themeView.localScale = Vector3.zero;
        closeThemeButton.transform.localScale = Vector3.zero;

        if (themeManager == null)
            themeManager = GetComponent<ThemeManager>();

        themeManager.DisplayTheme();

        Sequence seq = DOTween.Sequence();
        seq.PrependInterval(waitTime);

        seq.Append(closeThemeButton.transform.DOScale(1, 0.4f).SetEase(Ease.OutBack));
        seq.JoinCallback(() => audioManager.PlaySFXPop());
        seq.Append(themeView.DOScale(1, 0.4f).SetEase(Ease.OutBack));
        seq.JoinCallback(() => audioManager.PlaySFXPop());
        seq.Play();
    }

    private void CloseTheme(float waitTime)
    {
        Sequence seq = DOTween.Sequence();
        seq.PrependInterval(waitTime);

        seq.Append(themeView.DOScale(0, 0.2f).SetEase(Ease.InBack));
        seq.JoinCallback(() => audioManager.PlaySFXPop());
        seq.Append(closeThemeButton.transform.DOScale(0, 0.2f).SetEase(Ease.InBack));
        seq.JoinCallback(() => audioManager.PlaySFXPop());

        seq.AppendCallback(() =>
        {
            themeParent.gameObject.SetActive(false);
        });

        seq.Play();
    }

    private void StartHomeScene()
    {
        if (GameData.GetCurrentScene() == "Begin Scene")
        {
            OpenEvent7Days(0.7f);
        }
        else
        {
            OpenLevelMap(0.7f);
            GameData.SetCurrentScene("Home Scene");
        }

    }

    // ----- Chest reward UI -----
    public void ShowItemReward(List<ChestData> items, ChestType chestType, RewardData rewardData)
    {
        rewardBackground.gameObject.SetActive(true);
        rewardBackground.GetComponent<CanvasGroup>().alpha = 0;

        currentTime = countdownTime;
        countdownText.text = $"[{Mathf.Ceil(currentTime)}]";

        Sequence seq = DOTween.Sequence();
        seq.PrependInterval(0.2f);

        seq.Append(rewardBackground.GetComponent<CanvasGroup>().DOFade(1, 0.4f).SetEase(Ease.OutBack));
        // seq.JoinCallback(() => audioManager.PlaySFXOpenBox());

        foreach (var item in items)
        {
            Debug.Log(chestType + " Chest Data: " + item.rewardType + " - " + item.numberBetween.x + " to " + item.numberBetween.y);
            
            int value = Random.Range(item.numberBetween.x, item.numberBetween.y);
            if (value <= 0)
                continue;

            GameObject itemRewardObj = itemRewardPool.GetObject();
            itemRewardObj.GetComponentInChildren<Image>().sprite = rewardData.GetRewardIconByType(item.rewardType, chestType);
            itemRewardObj.GetComponentInChildren<TMP_Text>().text = "x" + value.ToString();
            // itemRewardObj.GetComponentInChildren<TMP_Text>().transform.localScale = Vector3.zero;
            itemRewardObj.transform.localScale = Vector3.zero;

            seq.Append(itemRewardObj.transform.DOScale(1, 0.4f).SetEase(Ease.OutBack));
            seq.JoinCallback(() => audioManager.PlaySFXPop());
        }

        seq.AppendCallback(() => StartCoroutine(CountdownRoutine()));
        

        seq.Play();
    }
    private IEnumerator CountdownRoutine()
    {
        while (currentTime > 0)
        {
            countdownText.text = $"[{Mathf.Ceil(currentTime)}]";
            audioManager.PlaySFXAddStar();
            yield return new WaitForSeconds(1f);
            currentTime -= 1f;
        }

        countdownText.text = " ";

        // Ẩn text hoặc gọi sự kiện sau khi đếm xong

        Sequence seq = DOTween.Sequence();

        seq.Append(itemRewardPool.transform.DOPunchScale(Vector3.one * 0.5f, 0.3f, 8, 1));
        // seq.JoinCallback(() => audioManager.PlaySFXGemReceive());
        seq.JoinCallback(() => audioManager.PlaySFXAddStar());

        seq.AppendInterval(0.5f);

        seq.Append(rewardBackground.GetComponent<CanvasGroup>().DOFade(0, 0.4f).SetEase(Ease.OutBack));

        seq.AppendCallback(() => {
            rewardBackground.gameObject.SetActive(false);
            itemRewardPool.ReturnAllObjects();
        });
        

        seq.Play();
    }
}

