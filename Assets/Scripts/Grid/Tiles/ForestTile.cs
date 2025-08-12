// ForestTile: Đại diện cho loại tile rừng trong lưới, kế thừa từ GridTileBase.

using UnityEngine;

public class ForestTile : GridTileBase
{
    public override string GetTileType() => "Rừng cây";

    public override void HandleLongPress()
    {
        // Xử lý sự kiện nhấn giữ cho ForestTile
        Debug.Log("Đã nhấn giữ trên Rừng cây");
        // Thêm logic xử lý tại đây nếu cần
    }
}