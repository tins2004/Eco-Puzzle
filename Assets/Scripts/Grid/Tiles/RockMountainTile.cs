using UnityEngine;

public class RockMountainTile : GridTileBase
{
    public override string GetTileType() => "Rock Mountain";


    public override void HandleLongPress()
    {
        // ReplaceTile(TileType.T01_Barren_Land, gridManager.GetTilePrefab(TileType.T01_Barren_Land));
    }
}
