using UnityEngine;

public class GreenTreeTile : GridTileBase
{
    public override TileType GetTileType() => TileType.T04_Green_Tree;

    protected override void DisplaySpriteTile()
    {
        spriteRenderer.sprite = gridManager.themeData.listTheme[GameData.GetIdTheme()].T04;
    }

    public override void UpgradeTile()
    {
        // Debug.Log("Đã nhấn giữ trên Cây xanh");
        ResetTilePosition();
    }
}
