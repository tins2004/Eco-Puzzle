using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    [HideInInspector] public static LanguageManager Instance;

    [HideInInspector] public string currentLanguage;

    private string[] listLanguages = new string[] { "English", "Vietnamese", "Chinese", "Japanese", "Korean", "Spanish", "Portuguese", "French", "German", "Russian", "Thai"};
    [HideInInspector] public string[] listLanguagesShow = new string[] { "English", "Tiếng việt", "中文简体", "日本語", "한국어", "Español", "Português", "Français", "Deutsch", "Русский", "ไทย"};

    [SerializeField] private TMP_FontAsset englishFontAsset;
    [SerializeField] private TMP_FontAsset chineseFontAsset;
    [SerializeField] private TMP_FontAsset japaneseFontAsset;
    [SerializeField] private TMP_FontAsset koreanFontAsset;
    [SerializeField] private TMP_FontAsset thaiFontAsset;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        currentLanguage = GameData.GetLanguage();

        if (string.IsNullOrEmpty(currentLanguage)) // Lần đầu mở app;
        {
            string systemLanguage = Application.systemLanguage.ToString();


            switch (Application.systemLanguage)
            {
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional:
                case SystemLanguage.Chinese:
                    systemLanguage = "Chinese";
                    break;
                default:
                    break;
            }

            for (int i = 0; i < listLanguages.Length; i++)
            {
                if (listLanguages[i].Equals(systemLanguage, StringComparison.OrdinalIgnoreCase))
                {
                    currentLanguage = systemLanguage;
                    break;
                }
            }

            if (string.IsNullOrEmpty(currentLanguage))
                currentLanguage = listLanguages[0];

            GameData.SetLanguage(currentLanguage);
        }
    }

    public int GetLanguageIndex(string code)
    {
        for (int i = 0; i < listLanguages.Length; i++)
        {
            if (listLanguages[i] == code)
                return i;
        }

        // Không tìm thấy thì trả về 0
        return 0;
    }

    public void ChangeAndSetLanguage()
    {
        string nextLanguage = GetLanguageIndex(currentLanguage) >= listLanguages.Length - 1 ? listLanguages[0] : listLanguages[GetLanguageIndex(currentLanguage) + 1];
        currentLanguage = nextLanguage;

        GameData.SetLanguage(currentLanguage);
    }

    public void DisplayNameLanguage(TMP_Text tmp_Text)
    {
        switch (currentLanguage)
        {
            case "Chinese":
                DisplayChineseText(tmp_Text, listLanguagesShow[GetLanguageIndex(currentLanguage)]);
                break;
            case "Japanese":
                DisplayJapaneseText(tmp_Text, listLanguagesShow[GetLanguageIndex(currentLanguage)]);
                break;
            case "Korean":
                DisplayKoreanText(tmp_Text, listLanguagesShow[GetLanguageIndex(currentLanguage)]);
                break;
            case "Thai":
                DisplayThaiText(tmp_Text, listLanguagesShow[GetLanguageIndex(currentLanguage)]);
                break;
            default:
                DisplayEnglishText(tmp_Text, listLanguagesShow[GetLanguageIndex(currentLanguage)]);
                break;
        }
    }

    public void DisplayEnglishText(TMP_Text tmp_Text, string messenger)
    {
        tmp_Text.font = englishFontAsset;
        tmp_Text.text = messenger;
    }

    public void DisplayChineseText(TMP_Text tmp_Text, string messenger)
    {
        tmp_Text.font = chineseFontAsset;
        tmp_Text.text = messenger;
    }

    public void DisplayJapaneseText(TMP_Text tmp_Text, string messenger)
    {
        tmp_Text.font = japaneseFontAsset;
        tmp_Text.text = messenger;
    }

    public void DisplayKoreanText(TMP_Text tmp_Text, string messenger)
    {
        tmp_Text.font = koreanFontAsset;
        tmp_Text.text = messenger;
    }

    public void DisplayThaiText(TMP_Text tmp_Text, string messenger)
    {
        tmp_Text.font = thaiFontAsset;
        tmp_Text.text = messenger;
    }
}
