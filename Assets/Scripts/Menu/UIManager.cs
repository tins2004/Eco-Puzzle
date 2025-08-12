using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // private LevelData levelData;

    [Header("UI Object")]
    [SerializeField] private TMP_Text missionText;
    [SerializeField] private TMP_Text limitText;
    [SerializeField] private TMP_Text scoreText;

    // ----- Score and Star -----
    private int currentScore = 0;
    private int maxScore = 0;
    private int[] starRating = new int[3];

    // ----- Limit -----
    private int limitValue = 0;

    // public void SetLevelData(LevelData data)
    // {
    //     levelData = data;
    // }
    public void SetScoreData(LevelData levelData)
    {
        maxScore = levelData.maxScore;
        starRating[0] = (int)(maxScore * 0.25); // 1 sao = 25% điểm tối đa
        starRating[1] = (int)(maxScore * 0.5);  // 2 sao = 50% điểm tối đa
        starRating[2] = (int)(maxScore * 0.8);  // 3 sao = 80% điểm tối đa
    }

    public void UpdateUI(Dictionary<TileType, int> tileMissionCounts, Dictionary<AnimalType, int> animalMissionCounts, int limitValue)
    {
        // if (levelData == null)
        // {
        //     Debug.LogError("Chưa có dữ liệu của level hiện tại!");
        //     return;
        // }

        UpdateMissionUI(tileMissionCounts, animalMissionCounts);
        UpdateLimitUI(limitValue);
        UpdateScoreUI();
    }

    public void UpdateMissionUI(Dictionary<TileType, int> tileMissionCounts, Dictionary<AnimalType, int> animalMissionCounts)
    {
        if (missionText == null)
        {
            Debug.LogError("Mission Text chưa được gán!");
            return;
        }

        missionText.text = "Missions:\n";

        // Nhiệm vụ tile
        foreach (var kvp in tileMissionCounts)
        {
            if (kvp.Value > 0)
                missionText.text += $"{kvp.Key}: {kvp.Value}\n";
            else
                missionText.text += $"{kvp.Key}: Hoàn thành!\n";
        }

        // Nhiệm vụ động vật
        foreach (var kvp in animalMissionCounts)
        {
            if (kvp.Value > 0)
                missionText.text += $"{kvp.Key}: {kvp.Value}\n";
            else
                missionText.text += $"{kvp.Key}: Hoàn thành!\n";
        }

        if (tileMissionCounts.Values.All(v => v <= 0) && animalMissionCounts.Values.All(v => v <= 0))
        {
            missionText.text = "Tất cả nhiệm vụ đã hoàn thành!";
            FinishAllMissions();
        }
    }

    public void UpdateLimitUI(int value)
    {
        if (limitText == null)
        {
            Debug.LogError("Limit Text chưa được gán!");
            return;
        }

        limitValue = value;

        if (limitValue > 0)
            limitText.text = $"Limit: {limitValue}";
        else
        {
            limitValue = 0;
            limitText.text = "Đã hết lượt di chuyển!";
        }
    }

    public void AddScore(int score)
    {
        currentScore += score;

        if (currentScore > maxScore)
        {
            currentScore = maxScore;
        }

        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText == null)
        {
            Debug.LogError("Score Text chưa được gán!");
            return;
        }

        scoreText.text = $"Score: {currentScore}/{maxScore}\n";
        if (currentScore >= starRating[2])
        {
            scoreText.text += $"3 Stars! {starRating[0]} - {starRating[1]} - [{starRating[2]}]";
        }
        else if (currentScore >= starRating[1])
        {
            scoreText.text += $"2 Stars! {starRating[0]} - [{starRating[1]}] - {starRating[2]}";
        }
        else if (currentScore >= starRating[0])
        {
            scoreText.text += $"1 Star! [{starRating[0]}] - {starRating[1]} - {starRating[2]}";
        }
        else
        {
            scoreText.text += $"No Stars {starRating[0]} - {starRating[1]} - {starRating[2]}";
        }
    }

    private void FinishAllMissions()
    {
        if (limitValue > 0)
        {
            AddScore(limitValue * 15);
            UpdateLimitUI(0);
        }

        FindObjectOfType<LevelManager>().NextLevel();
    }
}
