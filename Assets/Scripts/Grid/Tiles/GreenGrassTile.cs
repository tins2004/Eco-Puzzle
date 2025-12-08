using UnityEngine;

public class GreenGrassTile : GridTileBase
{
    public override TileType GetTileType() => TileType.T03_Green_Grass;

    protected override void DisplaySpriteTile()
    {
        spriteRenderer.sprite = gridManager.themeData.listTheme[GameData.GetIdTheme()].T03;
    }

    public override void UpgradeTile()
    {
        missionManager.ReduceMoveStep();
        ReplaceTile(TileType.T04_Green_Tree, gridManager.GetTilePrefab(TileType.T04_Green_Tree), playSound: true);
    }
}

