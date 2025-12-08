using UnityEngine;

public class BarrenLandTile : GridTileBase
{

    public override TileType GetTileType() => TileType.T01_Barren_Land;

    protected override void DisplaySpriteTile()
    {
        spriteRenderer.sprite = gridManager.themeData.listTheme[GameData.GetIdTheme()].T01;
    }

    public override void UpgradeTile()
    {
        missionManager.ReduceMoveStep();
        ReplaceTile(TileType.T02_Water_Lake, gridManager.GetTilePrefab(TileType.T02_Water_Lake), playSound: true);
    }
}
