using UnityEngine;

public class BarrenLandTile : GridTileBase
{

    public override TileType GetTileType() => TileType.T01_Barren_Land;

    public override void HandleLongPress()
    {
        missionManager.ReduceMoveStep();
        ReplaceTile(TileType.T02_Water_Lake, gridManager.GetTilePrefab(TileType.T02_Water_Lake), playSound: true);
    }
}
