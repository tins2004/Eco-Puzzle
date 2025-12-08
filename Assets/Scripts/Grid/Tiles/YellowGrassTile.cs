using UnityEngine;

public class YellowGrassTile : GridTileBase
{
    public override TileType GetTileType() => TileType.T08_Yellow_Grass;

    protected override void DisplaySpriteTile()
    {
        spriteRenderer.sprite = gridManager.themeData.listTheme[GameData.GetIdTheme()].T08;
    }

    public override void UpgradeTile()
    {
        missionManager.ReduceMoveStep();
        ReplaceTile(TileType.T09_Xavan, gridManager.GetTilePrefab(TileType.T09_Xavan), playSound: true);
    }
}
