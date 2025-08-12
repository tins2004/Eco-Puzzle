using UnityEngine;

public class WaterLakeTile : GridTileBase
{
    public override string GetTileType() => "Water Lake";


    public override void HandleLongPress()
    {
        // ReplaceTile(TileType.T01_Barren_Land, gridManager.GetTilePrefab(TileType.T01_Barren_Land));
    }
}
