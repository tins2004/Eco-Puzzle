using UnityEngine;

public class XavanTile : GridTileBase
{
    public override TileType GetTileType() => TileType.T09_Xavan;


    public override void HandleLongPress()
    {
        missionManager.ReduceMoveStep();
        ReplaceTile(TileType.T04_Green_Tree, gridManager.GetTilePrefab(TileType.T04_Green_Tree));
    }
}
