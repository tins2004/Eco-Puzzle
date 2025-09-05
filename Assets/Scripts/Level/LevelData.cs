using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileMission
{
    public TileType tileType;
    public int requiredCount;
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
