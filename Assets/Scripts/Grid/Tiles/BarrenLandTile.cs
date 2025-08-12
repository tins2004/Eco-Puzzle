using UnityEngine;

public class BarrenLandTile : GridTileBase
{

    public override string GetTileType() => "Đất khô cằn";
    
    public override void HandleLongPress()
    {
        missionManager.ReduceMoveStep();
        ReplaceTile(TileType.T02_Water_Lake, gridManager.GetTilePrefab(TileType.T02_Water_Lake));
    }
}
