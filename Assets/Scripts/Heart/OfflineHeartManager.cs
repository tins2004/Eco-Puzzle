using UnityEngine;
using System;
using TMPro;

public class OfflineHeartManager : MonoBehaviour
{
    // [Header("UI")]
    // [SerializeField] private TMP_Text heartText;          // Text hiển thị số tim
    // [SerializeField] private TMP_Text timerText;          // Text hiển thị thời gian hồi tim
    // private TMP_Text heartText;

    // [Header("Heart Settings")]
    // [SerializeField] private int maxHearts = 5;                 // Số tim tối đa
    // [SerializeField] private int heartRegenMinutes = 15;        // Bao lâu hồi 1 tim (phút)
    // private int currentHeart;

    // private long startRealTime = 0;
    // private long startTick = 0;
    // private long lastTimeMinusHeart;                    
    // private long msPerHeart; // Số tick tương ứng thời gian hồi 1 tim

    // void Start()
    // {
    //     startRealTime = GameData.GetRealTime();
    //     startTick = Environment.TickCount;
    //     GameData.UpdateStartValue(startRealTime, startTick);

    //     msPerHeart = heartRegenMinutes * 60 * 1000; // phút -> ms

    //     heartText = GetComponent<HomeManager>().heartText;

    //     // Lấy dữ liệu cũ
    //     currentHeart = GameData.GetCurrentHeart();
    //     lastTimeMinusHeart = GameData.GetLastTimeMinusHeartTime();

    //     Debug.Log(
    //         $"Thời gian cuối {new DateTime(1970, 1, 1).AddMilliseconds(lastTimeMinusHeart).ToLocalTime():HH:mm:ss dd/MM/yyyy} " +
    //         $"Thời gian bắt đầu {new DateTime(1970, 1, 1).AddMilliseconds(startRealTime + (Environment.TickCount - startTick)).ToLocalTime():HH:mm:ss dd/MM/yyyy} "
    //     );

    //     UpdateHeartRecovery();
    // }

    // private void Update()
    // {
    //     UpdateHeartRecovery();
    //     UpdateUI();

    // }

    // void OnApplicationPause(bool pause)
    // {
    //     if (pause) SaveDataHeart(); // Khi dừng
    //     else // Khi quay lại
    //     {
    //         if (startRealTime != 0 && startTick != 0) //Chạy start trước
    //             UpdateHeartRecovery();
    //     }
    // }

    // void OnApplicationQuit()
    // {
    //     SaveDataHeart();
    // }

    // void SaveDataHeart()
    // {
    //     GameData.UpdateLastRealTime();
    // }

    // void UpdateHeartRecovery()
    // {
    //     if (currentHeart >= maxHearts) return;

    //     long currentTime = startRealTime + (Environment.TickCount - startTick);
    //     long timeDiff = currentTime - lastTimeMinusHeart;

    //     // Nếu tick âm (reboot máy), bỏ qua
    //     if (timeDiff < 0)
    //     {
    //         lastTimeMinusHeart = currentTime;
    //         return;
    //     }


    //     int heartsToRegen = (int)((timeDiff / 60000.0) / heartRegenMinutes);

    //     if (heartsToRegen > 0)
    //     {
    //         Debug.Log(
    //             $"Trôi qua {timeDiff / 60000.0:F1} phút " +
    //             $"từ {new DateTime(1970, 1, 1).AddMilliseconds(lastTimeMinusHeart).ToLocalTime():HH:mm:ss dd/MM/yyyy} " +
    //             $"đến {new DateTime(1970, 1, 1).AddMilliseconds(currentTime).ToLocalTime():HH:mm:ss dd/MM/yyyy}, " +
    //             $"hồi {heartsToRegen} tim."
    //         );

    //         GameData.AddHeart(heartsToRegen);
    //         currentHeart = GameData.GetCurrentHeart();
    //         lastTimeMinusHeart = currentTime;
    //         GameData.UpdateLastTimeMinusHeart();
    //         SaveDataHeart();
    //     }
    // }

    // private void UpdateUI()
    // {
    //     heartText.text = $"{currentHeart}/{maxHearts}";

    //     if (currentHeart < maxHearts)
    //     {
    //         long currentTime = startRealTime + (Environment.TickCount - startTick);

    //         long remaining = msPerHeart - (currentTime - lastTimeMinusHeart);
    //         if (remaining < 0) remaining = 0;

    //         TimeSpan t = TimeSpan.FromMilliseconds(remaining);
    //         heartText.text += $": {t.Minutes:D2}:{t.Seconds:D2}";
    //     }
    //     else
    //     {
    //         heartText.text += ": Full";
    //     }
    // }

    // public bool UseHeart()
    // {
    //     if (GameData.GetCurrentHeart() > 0)
    //     {
    //         GameData.MinusHeart();
    //         SaveDataHeart();
    //         return true;
    //     }
    //     return false;
    // }
}
