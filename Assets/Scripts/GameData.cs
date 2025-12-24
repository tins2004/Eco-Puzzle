using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class GameData
{
    private const string SceneKey = "Scene";
    private const string MaxLevelKey = "MaxLevel";
    private const string StarsKey = "Stars_"; // ví dụ: Stars_1, Stars_2...

    private const string GemKey = "Gem";

    private const string LanguageKey = "Language";
    private const string MusicKey = "Music";
    private const string SFXKey = "SFX";
    private const string VibrationKey = "Vibration";

    private const string BoosterSwapKey = "BoosterSwap";
    private const string BoosterUpgradeKey = "BoosterUpgrade";
    private const string BoosterLockerKey = "BoosterLocker";

    private const string ADSStatusKey = "ADSStatus";

    private const string LastTimeKey = "LastTimeStatus";

    private const string VIPBattlePassKey = "VIPBattlePass";
    private const string BattlePassDataKey = "BattlePassData";

    private const string IDThemeKey = "IDTheme";
    private const string OwnedThemeKey = "OwnedTheme";

    private const string Events7DaysKey = "Events7Days";
    private const string LastDatePickUpKey = "LastDatePickUp";

    // ----- Scene -----
    public static string GetCurrentScene() => PlayerPrefs.GetString(SceneKey, "Begin Scene");
    public static void SetCurrentScene(string sceneName)
    {
        PlayerPrefs.SetString(SceneKey, sceneName);
    }
    
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
        PlayerPrefs.SetInt(GemKey, (currentGem - value) < 0 ? 0 : (currentGem - value));
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
    public static void AddNumberOfBoosterUpgrade(int value)
    {
        int currentValue = GetNumberOfBoosterUpgrade();
        PlayerPrefs.SetInt(BoosterUpgradeKey, (currentValue + value) > 99 ? 99 : currentValue + value);
    }

    public static string GetBoosterSwapKey() => BoosterSwapKey;
    public static int GetNumberOfBoosterSwap() => PlayerPrefs.GetInt(BoosterSwapKey, 0);
    public static void AddNumberOfBoosterSwap(int value)
    {
        int currentValue = GetNumberOfBoosterSwap();
        PlayerPrefs.SetInt(BoosterSwapKey, (currentValue + value) > 99 ? 99 : currentValue + value);
    }

    public static string GetBoosterLockerKey() => BoosterLockerKey;
    public static int GetNumberOfBoosterLocker() => PlayerPrefs.GetInt(BoosterLockerKey, 0);
    public static void AddNumberOfBoosterLocker(int value)
    {
        int currentValue = GetNumberOfBoosterLocker();
        PlayerPrefs.SetInt(BoosterLockerKey, (currentValue + value) > 99 ? 99 : currentValue + value);
    }

    public static void AddNumberOfBoosterByKey(string keyBooster, int value)
    {
        int currentValue = PlayerPrefs.GetInt(keyBooster, 0);
        PlayerPrefs.SetInt(keyBooster, (currentValue + value) > 99 ? 99 : currentValue + value);
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

    public static bool IsConsecutiveDay(string lastTimeString, string currentTimeString)
    {
        if (string.IsNullOrEmpty(lastTimeString) || string.IsNullOrEmpty(currentTimeString))
            return false;

        DateTime lastTime = DateTime.Parse(lastTimeString);
        DateTime currentTime = DateTime.Parse(currentTimeString);

        DateTime lastDate = lastTime.Date;
        DateTime currentDate = currentTime.Date;

        return (currentDate - lastDate).Days == 1;
    }

    // ----- Battle Pass -----
    public static int GetVIPBattlePass() => PlayerPrefs.GetInt(VIPBattlePassKey, 0);
    public static void SetVIPBattlePass(int value)
    {
        PlayerPrefs.SetInt(VIPBattlePassKey, value);
    }

    public static string GetBattlePassDataString() => PlayerPrefs.GetString(BattlePassDataKey, "");
    public static string[] GetBattlePassDataArray()
    {
        string dataString = GetBattlePassDataString();
        if (string.IsNullOrEmpty(dataString))
        {
            return new string[0];
        }

        return dataString.Split(';');
    }
    public static void AddBattlePassData(string newData)
    {
        string currentData = GetBattlePassDataString();
        if (string.IsNullOrEmpty(currentData))
        {
            PlayerPrefs.SetString(BattlePassDataKey, newData);
        }
        else
        {
            PlayerPrefs.SetString(BattlePassDataKey, currentData + ";" + newData);
        }
    }

    // ----- Theme -----
    public static int GetIdTheme() => PlayerPrefs.GetInt(IDThemeKey, 0);
    public static void SetIdTheme(int value)
    {
        PlayerPrefs.SetInt(IDThemeKey, value);
    }

    public static string GetOwnedThemeString() => PlayerPrefs.GetString(BattlePassDataKey, "");
    public static int[] GetOwnedThemeArray()
    {
        string dataString = GetOwnedThemeString();
        if (string.IsNullOrEmpty(dataString))
        {
            return new int[] { 0 };
        }

        // Convert string[] -> int[]
        return dataString
            .Split(';')
            .Select(s => int.TryParse(s, out int val) ? val : 0)
            .ToArray();
    }

    public static void AddOwnedTheme(int newData)
    {
        if (HasOwnedTheme(newData))
            return;

        string currentData = GetOwnedThemeString();

        if (string.IsNullOrEmpty(currentData))
        {
            PlayerPrefs.SetString(BattlePassDataKey, newData.ToString());
        }
        else
        {
            PlayerPrefs.SetString(BattlePassDataKey, currentData + ";" + newData.ToString());
        }
    }

    public static bool HasOwnedTheme(int themeId)
    {
        int[] ownedThemes = GetOwnedThemeArray();

        for (int i = 0; i < ownedThemes.Length; i++)
        {
            if (ownedThemes[i] == themeId)
                return true;
        }

        return false;
    }

    // ----- Events 7 Days -----
    public static string GetEvent7DaysDataString() => PlayerPrefs.GetString(Events7DaysKey, "");
    public static int[] GetEvent7DaysDataArray()
    {
        string dataString = GetEvent7DaysDataString();
        if (string.IsNullOrEmpty(dataString))
        {
            return new int[0];
        }

        string[] parts = dataString.Split(';');
        int[] result = new int[parts.Length];

        for (int i = 0; i < parts.Length; i++)
        {
            int.TryParse(parts[i], out result[i]);
        }

        return result;
    }

    public static void AddEvent7DaysData(int newData)
    {
        string currentData = GetEvent7DaysDataString();
        if (string.IsNullOrEmpty(currentData))
        {
            PlayerPrefs.SetString(Events7DaysKey, newData.ToString());
        }
        else
        {
            PlayerPrefs.SetString(Events7DaysKey, currentData + ";" + newData.ToString());
        }
    }
    
    public static void ResetEvent7DaysData()
    {
        PlayerPrefs.DeleteKey(Events7DaysKey);
    }

    public static string GetLastDatePickUp()
    {
        return PlayerPrefs.GetString(LastDatePickUpKey, "");
    }
    public static void SetLastDatePickUp(string dateString)
    {
        PlayerPrefs.SetString(LastDatePickUpKey, dateString);
    }

    public static int GetNumberOfWaitingDays(string currentTimeString)
    {
        string lastTimeString = GetLastDatePickUp();

        if (string.IsNullOrEmpty(lastTimeString) || string.IsNullOrEmpty(currentTimeString))
            return 0;

        DateTime lastTime = DateTime.Parse(lastTimeString);
        DateTime currentTime = DateTime.Parse(currentTimeString);

        DateTime lastDate = lastTime.Date;
        DateTime currentDate = currentTime.Date;

        return (currentDate - lastDate).Days;
    }
}
