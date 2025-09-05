using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class AnimalData
{
    public AnimalType type;       // Loại animal
    public Sprite icon;           // Icon hiển thị
}

[Serializable]
public class AnimalMission
{
    public AnimalType animalType;
    public int requiredCount;
}

public enum AnimalType
{
    A00_Null,
    A01_WILD_BOAR,
    A02_WOLF,
    A03_BROWN_BEAR,
    A04_JAGUAR,
    A05_ELEPHANT,
    A06_BEE,
    A07_BUTTERFLY
}

public enum AnimalSize
{
    A00_Null = 0,
    A01_WILD_BOAR = 1,
    A02_WOLF = 2,
    A03_BROWN_BEAR = 3,
    A04_JAGUAR = 1,
    A05_ELEPHANT = 3,
    A06_BEE = 1,
    A07_BUTTERFLY = 2
}

public enum AnimalRegion
{
    A00_Null = TileType.T00_Null,
    A01_WILD_BOAR = TileType.T05_Forest,
    A02_WOLF = TileType.T05_Forest,
    A03_BROWN_BEAR = TileType.T05_Forest,
    A04_JAGUAR = TileType.T09_Xavan,
    A05_ELEPHANT = TileType.T09_Xavan,
    A06_BEE = TileType.T06_Flower_Field,
    A07_BUTTERFLY = TileType.T06_Flower_Field
}