using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Region
{
    public int Id;                        // Mã định danh vùng
    public TileType Type;                 // Loại vùng
    public List<GridTileBase> Tiles = new List<GridTileBase>();  // Danh sách tile trong vùng

    // Constructor 
    public Region(int id, TileType type)
    {
        Id = id;
        Type = type;
        Tiles = new List<GridTileBase>();
    }

    // Hàm thêm tile vào vùng 
    public void AddTile(GridTileBase tile)
    {
        if (tile != null && !Tiles.Contains(tile))
        {
            Tiles.Add(tile);
        }
    }

    // Hàm xóa tile ra khỏi vùng 
    public bool RemoveTile(GridTileBase tile)
    {
        return Tiles.Remove(tile);
    }

    // Kiểm tra tile có trong vùng không
    public bool ContainsTile(GridTileBase tile)
    {
        return Tiles.Contains(tile);
    }

    // Lấy số lượng tile 
    public int TileCount()
    {
        return Tiles.Count;
    }

    // Xóa toàn bộ tile
    public void ClearTiles()
    {
        Tiles.Clear();
    }

    // Debug thông tin vùng
    public override string ToString()
    {
        return $"Region {Id} | Type: {Type} | Tiles: {Tiles.Count}";
    }
}
