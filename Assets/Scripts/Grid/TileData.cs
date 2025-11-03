using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileData
{
    public bool active = false;
    public TileType type = TileType.T00_Null;
}

[Serializable]
public class TileDataSprite
{
    public TileType type;       // Loại tile
    public Sprite icon;         // Icon hiển thị
}

public enum TileType
{
    T00_Null,
    T01_Barren_Land,
    T02_Water_Lake,
    T03_Green_Grass,
    T04_Green_Tree,
    T05_Forest,
    T06_Flower_Field,
    T07_Rock_Mountain,
    T08_Yellow_Grass,
    T09_Xavan,
}

public enum TileScore
{
    T00_Null = 0,
    T01_Barren_Land = 0,
    T02_Water_Lake = 5,
    T03_Green_Grass = 10,
    T04_Green_Tree = 20,
    T05_Forest = 30,
    T06_Flower_Field = 10,
    T07_Rock_Mountain = 0,
    T08_Yellow_Grass = 10,
    T09_Xavan = 20
}
