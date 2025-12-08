using UnityEngine;

public class WaterLakeTile : GridTileBase
{
    public override TileType GetTileType() => TileType.T02_Water_Lake;

    protected override void DisplaySpriteTile()
    {
        spriteRenderer.sprite = gridManager.themeData.listTheme[GameData.GetIdTheme()].T02;
    }

    public override void UpgradeTile()
    {
        // ReplaceTile(TileType.T01_Barren_Land, gridManager.GetTilePrefab(TileType.T01_Barren_Land));
        ResetTilePosition();
    }
}
