using UnityEngine;

public class WaterLakeTile : GridTileBase
{
    public override TileType GetTileType() => TileType.T02_Water_Lake;


    public override void HandleLongPress()
    {
        // ReplaceTile(TileType.T01_Barren_Land, gridManager.GetTilePrefab(TileType.T01_Barren_Land));
    }
}
