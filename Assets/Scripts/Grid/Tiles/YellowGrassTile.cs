using UnityEngine;

public class YellowGrassTile : GridTileBase
{
    public override TileType GetTileType() => TileType.T08_Yellow_Grass;


    public override void HandleLongPress()
    {
        missionManager.ReduceMoveStep();
        ReplaceTile(TileType.T09_Xavan, gridManager.GetTilePrefab(TileType.T09_Xavan), playSound: true);
    }
}
