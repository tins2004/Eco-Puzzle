using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    [Header("Button")]
    [SerializeField] private Button musicButton;
    [SerializeField] private Button sfxButton;
    [SerializeField] private Button vibrationButton;
    [SerializeField] private Button languageButton;

    
    private HomeManager homeManager;

    void Start()
    {
        homeManager = GetComponent<HomeManager>();
    }

    public void SetupSettingStatus()
    {
        musicButton.image.color = homeManager.audioManager.musicOn == true ? Color.white : Color.gray;
        sfxButton.image.color = homeManager.audioManager.sfxOn == true ? Color.white : Color.gray;
        vibrationButton.image.color = homeManager.audioManager.vibrationOn == true ? Color.white : Color.gray;

        musicButton.onClick.AddListener(() =>
        {
            homeManager.audioManager.SetMusic(!homeManager.audioManager.musicOn);
            musicButton.image.color = homeManager.audioManager.musicOn == true ? Color.white : Color.gray;
            homeManager.audioManager.PlaySFXButton();
            homeManager.audioManager.PlayMusicBackground();
        });

        sfxButton.onClick.AddListener(() =>
        {
            homeManager.audioManager.SetSFX(!homeManager.audioManager.sfxOn);
            sfxButton.image.color = homeManager.audioManager.sfxOn == true ? Color.white : Color.gray;
            homeManager.audioManager.PlaySFXButton();
        });

        vibrationButton.onClick.AddListener(() =>
        {
            homeManager.audioManager.SetVibration(!homeManager.audioManager.vibrationOn);
            vibrationButton.image.color = homeManager.audioManager.vibrationOn == true ? Color.white : Color.gray;
            homeManager.audioManager.PlaySFXButton();
            homeManager.audioManager.VibrateClassic();
        });


        // ----- language ------
        homeManager.languageManager.currentLanguage = GameData.GetLanguage();

        TMP_Text buttonText = languageButton.GetComponentInChildren<TMP_Text>();
        homeManager.languageManager.DisplayNameLanguage(buttonText);

        languageButton.onClick.AddListener(() =>
        {
            homeManager.languageManager.ChangeAndSetLanguage();
            homeManager.languageManager.DisplayNameLanguage(buttonText);
            homeManager.audioManager.PlaySFXButton();
        });
    }
}
