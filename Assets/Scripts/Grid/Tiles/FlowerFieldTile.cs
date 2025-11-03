using UnityEngine;

public class FlowerFieldTile : GridTileBase
{
    public override TileType GetTileType() => TileType.T06_Flower_Field;


    public override void HandleLongPress()
    {
        missionManager.ReduceMoveStep();
        ReplaceTile(TileType.T04_Green_Tree, gridManager.GetTilePrefab(TileType.T04_Green_Tree), playSound: true);
    }
}
