using System;
using System.Globalization;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class GameData
{
    private const string MaxLevelKey = "MaxLevel";
    private const string StarsKey = "Stars_"; // ví dụ: Stars_1, Stars_2...

    private const string GemKey = "Gem";

    private const string LanguageKey = "Language";
    private const string MusicKey = "Music";
    private const string SFXKey = "SFX";
    private const string VibrationKey = "Vibration";

    private const string BoosterSwapKey = "BoosterSwap";
    private const string BoosterUpgradeKey = "BoosterUpgrade";

    private const string ADSStatusKey = "ADSStatus";

    private const string LastTimeKey = "LastTimeStatus";

    // ----- Level Progress -----
    public static int GetMaxLevel() => PlayerPrefs.GetInt(MaxLevelKey, 0);

    public static void SetMaxLevel(int level)
    {
        PlayerPrefs.SetInt(MaxLevelKey, level);
    }

    public static int GetStars(int levelIndex)
    {
        return PlayerPrefs.GetInt(StarsKey + levelIndex, 0);
    }

    public static void SetStars(int levelIndex, int stars)
    {
        PlayerPrefs.SetInt(StarsKey + levelIndex, stars);
    }

    public static void ResetData()
    {
        PlayerPrefs.DeleteAll();
    }

    public static int GetCurrentLevel()
    {
        return PlayerPrefs.GetInt("CurrentLevel", 1);
    }

    // ----- Gem -----
    public static int GetCurrentGem() => PlayerPrefs.GetInt(GemKey, 0);

    public static void MinusGem(int value)
    {
        int currentGem = GetCurrentGem();
        PlayerPrefs.SetInt(GemKey, (currentGem - value) < -9999 ? -9999 : (currentGem - value));
    }

    public static void AddGem(int value)
    {
        int currentGem = GetCurrentGem();
        PlayerPrefs.SetInt(GemKey, (currentGem + value) > 99999 ? 99999 : (currentGem + value));
    }
    
    // ----- Setting -----
    // --- Language ---
    public static string GetLanguage() => PlayerPrefs.GetString(LanguageKey, "");

    public static void SetLanguage(string language)
    {
        PlayerPrefs.SetString(LanguageKey, language);
    }

    // --- Music ---
    public static int GetMusic() => PlayerPrefs.GetInt(MusicKey, 1);

    public static void SetMusic(int value)
    {
        PlayerPrefs.SetInt(MusicKey, value);
    }

    // --- SFX ---
    public static int GetSFX() => PlayerPrefs.GetInt(SFXKey, 1);

    public static void SetSFX(int value)
    {
        PlayerPrefs.SetInt(SFXKey, value);
    }

    // --- Vibration ---
    public static int GetVibration() => PlayerPrefs.GetInt(VibrationKey, 1);

    public static void SetVibration(int value)
    {
        PlayerPrefs.SetInt(VibrationKey, value);
    }

    // ----- Booster -----
    public static string GetBoosterUpgradeKey() => BoosterUpgradeKey;
    public static int GetNumberOfBoosterUpgrade() => PlayerPrefs.GetInt(BoosterUpgradeKey, 0);
    public static void SetNumberOfBoosterUpgrade(int value)
    {
        int currentValue = GetNumberOfBoosterUpgrade();
        PlayerPrefs.SetInt(BoosterUpgradeKey, (currentValue + value) > 99 ? 99 : currentValue + value);
    }

    public static string GetBoosterSwapKey() => BoosterSwapKey;
    public static int GetNumberOfBoosterSwap() => PlayerPrefs.GetInt(BoosterSwapKey, 0);
    public static void SetNumberOfBoosterSwap(int value)
    {
        int currentValue = GetNumberOfBoosterSwap();
        PlayerPrefs.SetInt(BoosterSwapKey, (currentValue + value) > 99 ? 99 : currentValue + value);
    }

    // ----- ADS -----
    public static int GetADSStatus() => PlayerPrefs.GetInt(ADSStatusKey, 1);
    public static void SetADSStatus(int value)
    {
        PlayerPrefs.SetInt(ADSStatusKey, value);
    }

    // ----- Time -----
    public static string GetLastTime() => PlayerPrefs.GetString(LastTimeKey, " ");

    public static void SetLastTime(string currentTime)
    {
        PlayerPrefs.SetString(LastTimeKey, currentTime);
    }


    public static async Task<string?> GetInternetTime()
    {
        try
        {
            using (UnityWebRequest req = UnityWebRequest.Get("https://www.microsoft.com"))
            {
                await req.SendWebRequest();

                // Kiểm tra lỗi mạng hoặc không có kết nối
                if (req.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogWarning("Không thể kết nối mạng: " + req.error);
                    return null;
                }

                // Lấy header "Date" (giờ GMT)
                string netTime = req.GetResponseHeader("date");

                if (string.IsNullOrEmpty(netTime))
                {
                    Debug.LogWarning("Không lấy được header thời gian.");
                    return null;
                }

                // Parse chuỗi thành thời gian UTC
                DateTime utcTime = DateTime.Parse(netTime).ToUniversalTime();

                // Cộng offset theo múi giờ máy người chơi
                DateTime localTime = utcTime.ToLocalTime();

                Debug.Log("Thời gian nội địa: " + localTime.ToString("yyyy-MM-dd HH:mm:ss"));
                SetLastTime(localTime.ToString("yyyy-MM-dd HH:mm:ss"));
                return localTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Lỗi khi lấy thời gian: " + ex.Message);
            return null;
        }
    }

}
