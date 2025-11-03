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
    [SerializeField] private Button informationButton;
    [SerializeField] private Button eventButton;
    [SerializeField] private Button commentButton;
    [SerializeField] private TMP_Text gemText;

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
    [SerializeField] private Transform eventsParent;
    [SerializeField] private Transform eventsView;
    [SerializeField] private Button closeEventsButton;

    [Header("Comment")]
    [SerializeField] private string commentURL = "https://github.com/";

    [Header("Manager Link")]
    [HideInInspector] public LanguageManager languageManager;
    [HideInInspector] public AudioManager audioManager;
    private SettingManager settingManager;

    private string timeTest;


    public async void Start()
    {
        levelMapParent.gameObject.SetActive(false);
        settingsParent.gameObject.SetActive(false);

        sceneTransition.CloseEffect();

        timeTest = await GameData.GetInternetTime();

        // ----- Setup And Open ------
        audioManager = AudioManager.Instance;
        languageManager = LanguageManager.Instance;

        settingManager = GetComponent<SettingManager>();
        settingManager.SetupSettingStatus();

        OpenLevelMap(1.2f);
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
        eventButton.onClick.AddListener(() =>
        {
            audioManager.PlaySFXButton();
            CloseLevelMap(0f);
            OpenEvents(0.7f);
        });
        closeEventsButton.onClick.AddListener(() =>
        {
            audioManager.PlaySFXButton();
            CloseEvents(0f);
            OpenLevelMap(0.7f);
        });

        // --- Comment ---
        commentButton.onClick.AddListener(() =>
        {
            audioManager.PlaySFXButton();
            Application.OpenURL(commentURL);
        });
    }

    public void ResetData()
    {
        PlayerPrefs.DeleteAll();
    }

    // ----- Level map animations -----
    private void OpenLevelMap(float waitTime)
    {
        gemText.text = GameData.GetCurrentGem().ToString() + "\n" + timeTest;
        

        levelMapParent.gameObject.SetActive(true);

        levelMapView.localScale = Vector3.zero;
        informationButton.transform.localScale = Vector3.zero;
        eventButton.transform.localScale = Vector3.zero;
        commentButton.transform.localScale = Vector3.zero;
        settingsButton.transform.localScale = Vector3.zero;
        shopButton.transform.localScale = Vector3.zero;


        Sequence seq = DOTween.Sequence();
        seq.PrependInterval(waitTime);

        seq.Append(informationButton.transform.DOScale(1, 0.4f).SetEase(Ease.OutBack));
        seq.JoinCallback(() => audioManager.PlaySFXPop());

        seq.Append(eventButton.transform.DOScale(1, 0.2f).SetEase(Ease.OutBack));
        seq.Join(settingsButton.transform.DOScale(1, 0.2f).SetEase(Ease.OutBack));
        seq.JoinCallback(() => audioManager.PlaySFXPop());

        seq.Append(commentButton.transform.DOScale(1, 0.2f).SetEase(Ease.OutBack));
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

        seq.Append(eventButton.transform.DOScale(0, 0.2f).SetEase(Ease.InBack));
        seq.Join(settingsButton.transform.DOScale(0, 0.2f).SetEase(Ease.InBack));
        seq.Join(commentButton.transform.DOScale(0, 0.2f).SetEase(Ease.InBack));
        seq.Join(shopButton.transform.DOScale(0, 0.2f).SetEase(Ease.InBack));
        seq.JoinCallback(() => audioManager.PlaySFXPop());

        seq.Append(informationButton.transform.DOScale(0, 0.2f).SetEase(Ease.InBack));
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
        settingsView.localScale = Vector3.zero;
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

        seq.Append(settingsView.DOScale(1, 0.4f).SetEase(Ease.OutBack));
        seq.JoinCallback(() => audioManager.PlaySFXPop());

        seq.Play();
    }

    private void CloseSettings(float waitTime)
    {
        Sequence seq = DOTween.Sequence();
        seq.PrependInterval(waitTime);

        seq.Append(settingsView.DOScale(0, 0.2f).SetEase(Ease.InBack));
        seq.JoinCallback(() => audioManager.PlaySFXPop());

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
    private void OpenEvents(float waitTime)
    {
        eventsParent.gameObject.SetActive(true);
        eventsView.localScale = Vector3.zero;
        closeEventsButton.transform.localScale = Vector3.zero;

        Sequence seq = DOTween.Sequence();
        seq.PrependInterval(waitTime);

        seq.Append(closeEventsButton.transform.DOScale(1, 0.4f).SetEase(Ease.OutBack));
        seq.Append(eventsView.DOScale(1, 0.4f).SetEase(Ease.OutBack));
        seq.Play();
    }

    private void CloseEvents(float waitTime)
    {
        Sequence seq = DOTween.Sequence();
        seq.PrependInterval(waitTime);

        seq.Append(eventsView.DOScale(0, 0.2f).SetEase(Ease.InBack));
        seq.Append(closeEventsButton.transform.DOScale(0, 0.2f).SetEase(Ease.InBack));

        seq.AppendCallback(() =>
        {
            eventsParent.gameObject.SetActive(false);
        });

        seq.Play();
    }
}
