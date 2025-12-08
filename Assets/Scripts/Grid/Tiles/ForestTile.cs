// ForestTile: Đại diện cho loại tile rừng trong lưới, kế thừa từ GridTileBase.

using UnityEngine;

public class ForestTile : GridTileBase
{
    public override TileType GetTileType() => TileType.T05_Forest;

    protected override void DisplaySpriteTile()
    {
        spriteRenderer.sprite = gridManager.themeData.listTheme[GameData.GetIdTheme()].T05;
    }

    public override void UpgradeTile()
    {
        // Xử lý sự kiện nhấn giữ cho ForestTile
        // Debug.Log("Đã nhấn giữ trên Rừng cây");
        // Thêm logic xử lý tại đây nếu cần
        ResetTilePosition();
    }
}