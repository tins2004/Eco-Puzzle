using UnityEngine;

public class XavanTile : GridTileBase
{
    public override string GetTileType() => "Xavan";


    public override void HandleLongPress()
    {
        missionManager.ReduceMoveStep();
        ReplaceTile(TileType.T04_Green_Tree, gridManager.GetTilePrefab(TileType.T04_Green_Tree));
    }
}
