using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileMission
{
    public TileType tileType;
    public int requiredCount;

    // Thêm danh sách các vị trí (x, y)
    public List<Vector2Int> positions = new List<Vector2Int>();
}
[Serializable]
public class TileLock
{
    public Vector2Int position { get; private set; }
    public int requiredCount { get; private set; }



    public Vector2Int GetPositon() => position;
    public int GetRequiredCount() => requiredCount;

    public void SetPosition(Vector2Int pos)
    {
        position = pos;
    }

    public void SetRequiredCount(int count)
    {
        requiredCount = count;
    }
}


public enum LimitType
{
    None,
    TimeLimit,
    MoveLimit
}

[CreateAssetMenu(fileName = "LevelData", menuName = "Data/Level Data")]
public class LevelData : ScriptableObject
{
    public int width = 11;
    public int height = 11;
    public float gridSize = 1f;

    public TileData[] tiles;
    public List<TileMission> tileMissions = new List<TileMission>();
    public List<AnimalMission> animalMissions = new List<AnimalMission>();
    public List<TileLock> tileLocks = new List<TileLock>();

    [Header("Level Limit Settings")]
    public LimitType limitType = LimitType.None;
    public int limitValue = 0; // seconds nếu TimeLimit, moves nếu MoveLimit

    [Header("Level Scoring")]
    public int maxScore = 0; // Điểm tối đa có thể đạt được

    public void Init()
    {
        tiles = new TileData[width * height];
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i] = new TileData();
        }
    }

    public TileData GetTile(int x, int y)
    {
        int index = y * width + x;
        if (index >= 0 && index < tiles.Length)
            return tiles[index];
        return null;
    }
}
