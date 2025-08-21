using UnityEngine;

public class FlowerFieldTile : GridTileBase
{
    public override string GetTileType() => "Flower Field";


    public override void HandleLongPress()
    {
        missionManager.ReduceMoveStep();
        ReplaceTile(TileType.T04_Green_Tree, gridManager.GetTilePrefab(TileType.T04_Green_Tree));
    }
}
