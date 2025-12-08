using UnityEngine;

public class XavanTile : GridTileBase
{
    public override TileType GetTileType() => TileType.T09_Xavan;

    protected override void DisplaySpriteTile()
    {
        spriteRenderer.sprite = gridManager.themeData.listTheme[GameData.GetIdTheme()].T09;
    }

    public override void UpgradeTile()
    {
        missionManager.ReduceMoveStep();
        ReplaceTile(TileType.T04_Green_Tree, gridManager.GetTilePrefab(TileType.T04_Green_Tree), playSound: true);
    }
}
