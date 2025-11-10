using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using DG.Tweening;

[System.Serializable]
public class MissionCache
{
    public GameObject item;   // prefab item mission
    public TMP_Text text;     // text số lượng
    public Image icon;        // icon
}

public class MissionManager : MonoBehaviour
{
    // ----- Data -----
    private LevelData levelData;
    [HideInInspector] public UIManager uiManager;

    // ----- Mission -----
    private Dictionary<TileType, int> tileMissionCounts = new Dictionary<TileType, int>();
    private Dictionary<AnimalType, int> animalMissionCounts = new Dictionary<AnimalType, int>();

    // ----- Limit -----
    private int limitValue;

    public void SetLevelData(LevelData data, UIManager uiManager)
    {
        levelData = data;
        this.uiManager = uiManager;
    }

    public void StartMission()
    {
        Debug.Log("Bắt đầu nhiệm vụ cho level: " + levelData.name);

        if (levelData == null)
        {
            Debug.LogError("Chưa có dữ liệu của level hiện tại!");
            return;
        }

        if (levelData.limitType != LimitType.None)
        {
            Debug.Log($"Nhiệm vụ giới hạn: {levelData.limitType} với giá trị {levelData.limitValue}");
            limitValue = levelData.limitValue;
        }
        else
        {
            Debug.LogError("Không có giới hạn nhiệm vụ cho level này.");
        }

        // Debug.Log("Level loaded: " + currentLevel.name);
        foreach (var mission in levelData.tileMissions)
        {
            // Debug.Log($"Tile mission: {mission.tileType} x {mission.requiredCount}");
            tileMissionCounts[mission.tileType] = mission.requiredCount;
        }
        foreach (var mission in levelData.animalMissions)
        {
            // Debug.Log($"Animal mission: {mission.animalType} x {mission.requiredCount}");
            animalMissionCounts[mission.animalType] = mission.requiredCount;
        }

        if (tileMissionCounts.Count != 0)
            Debug.Log("Nhiệm vụ tile: " + string.Join(", ", tileMissionCounts));
        if (animalMissionCounts.Count != 0)
            Debug.Log("Nhiệm vụ động vật: " + string.Join(", ", animalMissionCounts));

        uiManager.UpdateUI(tileMissionCounts, animalMissionCounts, limitValue);
    }

    public void SetLimitValue(int value)
    {
        limitValue = value;
        uiManager.UpdateLimitUI(limitValue);
    }

    public (Dictionary<TileType, int>, Dictionary<AnimalType, int>) GetCurrentMission()
    {
        return (tileMissionCounts, animalMissionCounts);
    }

    public void ReduceMoveStep()
    {
        if (levelData.limitType != LimitType.MoveLimit)
            Debug.LogError("Không phải loại giới hạn MoveLimit!");


        limitValue--;
        // Debug.Log($"Giảm bước di chuyển, còn lại: {limitValue}");

        // if (limitValue <= 0)
        // {
        //     Debug.Log("Đã hết bước di chuyển!");
        //     // Kết thúc nhiệm vụ hoặc xử lý thua cuộc
        // }

        uiManager.UpdateLimitUI(limitValue);
    }

    

    public void CollectTile(TileType tileType, Vector3 position)
    {
        // Kiểm tra tileType có trong nhiệm vụ không
        if (tileMissionCounts.ContainsKey(tileType))
        {
            // Spawn hiệu ứng bay
            uiManager.SpawnMissionTileEffect(tileType, position);

            // Giảm đi 1
            tileMissionCounts[tileType]--;

            // Không cho xuống dưới 0
            if (tileMissionCounts[tileType] < 0)
                tileMissionCounts[tileType] = 0;

            // Debug.Log($"Thu thập {tileType}, còn lại: {tileMissionCounts[tileType]}");

            // Kiểm tra hoàn thành nhiệm vụ tileType
            // if (tileMissionCounts[tileType] == 0)
            // {
            //     Debug.Log($"Hoàn thành nhiệm vụ: {tileType}");
            // }

            uiManager.AddScore(50);
            uiManager.UpdateMissionUI(tileMissionCounts, animalMissionCounts);
        }
    }

    public void CollectAnimal(AnimalType animalType, Vector3 position)
    {
        // Kiểm tra animalType có trong nhiệm vụ không
        if (animalMissionCounts.ContainsKey(animalType))
        {
            uiManager.SpawnMissionAnimalEffect(animalType, position);
            // Giảm đi 1
            animalMissionCounts[animalType]--;

            // Không cho xuống dưới 0
            if (animalMissionCounts[animalType] < 0)
                animalMissionCounts[animalType] = 0;

            // Debug.Log($"Thu thập {animalType}, còn lại: {animalMissionCounts[animalType]}");

            // Kiểm tra hoàn thành nhiệm vụ animalType
            // if (animalMissionCounts[animalType] == 0)
            // {
            //     Debug.Log($"Hoàn thành nhiệm vụ: {animalType}");
            // }

            uiManager.AddScore(50);
            uiManager.UpdateMissionUI(tileMissionCounts, animalMissionCounts);
        }
    }
}
