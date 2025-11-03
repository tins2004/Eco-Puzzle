using UnityEngine;

public class GreenGrassTile : GridTileBase
{
    public override TileType GetTileType() => TileType.T03_Green_Grass;

    public override void HandleLongPress()
    {
        missionManager.ReduceMoveStep();
        ReplaceTile(TileType.T04_Green_Tree, gridManager.GetTilePrefab(TileType.T04_Green_Tree), playSound: true);
    }
}

