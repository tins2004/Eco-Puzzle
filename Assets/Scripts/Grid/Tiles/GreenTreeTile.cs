using UnityEngine;

public class GreenTreeTile : GridTileBase
{
    public override TileType GetTileType() => TileType.T04_Green_Tree;

    public override void UpgradeTile()
    {
        // Debug.Log("Đã nhấn giữ trên Cây xanh");
    }
}
