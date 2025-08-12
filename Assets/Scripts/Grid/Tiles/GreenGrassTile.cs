using UnityEngine;

public class GreenGrassTile : GridTileBase
{
    public override string GetTileType() => "Cỏ xanh";

    public override void HandleLongPress()
    {
        missionManager.ReduceMoveStep();
        ReplaceTile(TileType.T04_Green_Tree, gridManager.GetTilePrefab(TileType.T04_Green_Tree));

        // Sau khi thay, delay 1 frame để tile mới tồn tại rồi xử lý
        // StartCoroutine(AfterTreeCreated());
    }
}

