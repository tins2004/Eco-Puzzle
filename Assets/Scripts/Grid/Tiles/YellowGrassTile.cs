using UnityEngine;

public class YellowGrassTile : GridTileBase
{
    public override string GetTileType() => "Yellow Grass";


    public override void HandleLongPress()
    {
        missionManager.ReduceMoveStep();
        ReplaceTile(TileType.T09_Xavan, gridManager.GetTilePrefab(TileType.T09_Xavan));
    }
}
