using UnityEngine;

public class RockMountainTile : GridTileBase
{
    public override TileType GetTileType() => TileType.T07_Rock_Mountain;


    public override void UpgradeTile()
    {
        // ReplaceTile(TileType.T01_Barren_Land, gridManager.GetTilePrefab(TileType.T01_Barren_Land));
        ResetTilePosition();
    }
}
