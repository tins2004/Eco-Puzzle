using UnityEngine;

public class GreenTreeTile : GridTileBase
{
    public override string GetTileType() => "Cây xanh";

    public override void HandleLongPress()
    {
        Debug.Log("Đã nhấn giữ trên Cây xanh");
    }
}
