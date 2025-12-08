using UnityEngine;

public class FlowerFieldTile : GridTileBase
{
    public override TileType GetTileType() => TileType.T06_Flower_Field;

    protected override void DisplaySpriteTile()
    {
        spriteRenderer.sprite = gridManager.themeData.listTheme[GameData.GetIdTheme()].T06;
    }

    public override void UpgradeTile()
    {
        missionManager.ReduceMoveStep();
        ReplaceTile(TileType.T04_Green_Tree, gridManager.GetTilePrefab(TileType.T04_Green_Tree), playSound: true);
    }
}
