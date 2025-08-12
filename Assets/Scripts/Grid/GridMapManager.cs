// GridMapManager: Tạo và quản lý lưới tile trong Scene dựa trên dữ liệu từ GridMapData.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Grid))]
public class GridMapManager : MonoBehaviour
{
    [Header("Grid Data")] 
    private LevelData mapData;

    [Header("Tile Prefabs")]
    [SerializeField] private GameObject[] tilePrefabs;

    private Grid grid;
    public Dictionary<Vector3Int, TileType> tempTileStates = new Dictionary<Vector3Int, TileType>();

    public void SetLevelData(LevelData data)
    {
        mapData = data;
    }

    public void StartGrid()
    {   
        Debug.Log("Khởi tạo Grid Map với dữ liệu bản đồ: " + mapData.name);

        grid = GetComponent<Grid>();
        Generate();

        transform.localScale = new Vector3(mapData.gridSize, mapData.gridSize, 1f);
    }

    public GameObject GetTilePrefab(TileType type)
    {
        switch (type)
        {
            case TileType.T01_Barren_Land:
                return tilePrefabs[0];
            case TileType.T02_Water_Lake:
                return tilePrefabs[1];
            case TileType.T03_Green_Grass:
                return tilePrefabs[2];
            case TileType.T04_Green_Tree:
                return tilePrefabs[3];
            case TileType.T05_Forest:
                return tilePrefabs[4];
            case TileType.T06_Flower_Field:
                return tilePrefabs[5];
            case TileType.T07_Rock_Mountain:
                return tilePrefabs[6];
            case TileType.T08_Yellow_Grassland:
                return tilePrefabs[7];
            default:
                return null;
        }
    }

    /// <summary>
    /// Trả về dữ liệu bản đồ hiện tại.
    /// </summary>
    /// <returns> Dữ liệu bản đồ </returns>
    public LevelData GetMapData()
    {
        return mapData;
    }

    /// <summary>
    /// Trả về tile tại tọa độ xác định.
    /// </summary>
    /// <param name="coords">Tọa độ của tile trong lưới.</param>
    /// <returns>Tile tại tọa độ xác định, hoặc null nếu không tìm thấy.</returns>
    public GridTileBase GetTileAt(Vector3Int coords)
    {
        foreach (Transform child in transform)
        {
            GridTileBase tile = child.GetComponent<GridTileBase>();
            if (tile != null && tile.Coordinates == coords)
                return tile;
        }
        return null;
    }

    /// <summary>
    /// Cập nhật trạng thái tạm thời của tile tại tọa độ xác định.
    /// </summary>
    /// <param name="coords">Tọa độ của tile trong lưới.</param>
    /// <param name="type">Loại tile mới.</param>
    public void SetTempTile(Vector3Int coords, TileType type)
    {
        tempTileStates[coords] = type;
    }

    /// <summary>
    /// Tạo bản đồ tile trong Scene dựa trên mapData.
    /// Xóa các tile cũ, sau đó duyệt qua dữ liệu và sinh prefab tương ứng.
    /// </summary>
    private void Generate()
    {
        tempTileStates.Clear(); // reset data ảo

        foreach (Transform child in transform)
            Destroy(child.gameObject);

        if (mapData == null || mapData.tiles == null)
        {
            Debug.LogWarning("Map data is empty!");
            return;
        }

        int offsetX = mapData.height / 2;
        int offsetY = mapData.width / 2;

        for (int x = 0; x < mapData.height; x++) // X = hàng
        {
            for (int y = 0; y < mapData.width; y++) // Y = cột
            {
                TileData tileData = mapData.GetTile(x, y);
                if (tileData == null || !tileData.active)
                    continue;

                GridTileBase prefab = GetPrefabByType(tileData.type).GetComponent<GridTileBase>();

                if (prefab != null)
                {
                    int flippedY = mapData.width - 1 - y;
                    Vector3Int cellPos = new Vector3Int(x - offsetX, flippedY - offsetY, 0);

                    Vector3 worldPos = grid.GetCellCenterWorld(cellPos);
                    var tile = Instantiate(prefab, worldPos, Quaternion.identity, transform);
                    tile.Init(cellPos);

                    // Lưu vào data ảo
                    tempTileStates[cellPos] = tileData.type;
                }
            }
        }
    }

    private GameObject GetPrefabByType(TileType type)
    {
        switch (type)
        {
            case TileType.T01_Barren_Land:
                return GetTilePrefab(TileType.T01_Barren_Land);
            case TileType.T02_Water_Lake:
                return GetTilePrefab(TileType.T02_Water_Lake);
            case TileType.T03_Green_Grass:
                return GetTilePrefab(TileType.T03_Green_Grass);
            // case TileType.T03_Green_Grass:
            //     return Random.value > 0.5f ? grassPrefab : forestPrefab;
            default:
                return null;
        }
    }

    /// <summary>
    /// Cập nhật trạng thái toàn bộ bản đồ dựa trên quy tắc.
    /// </summary>
    public void UpdateMapState(TileType newType, GridTileBase newTile)
    {
        // Debug.Log($"Cập nhật trạng thái bản đồ với loại mới: {newType}");
        // Debug.Log("Cập nhật trạng thái toàn bộ bản đồ...");

        // Lấy danh sách tất cả tile hiện tại
        var allTiles = new List<GridTileBase>();
        foreach (Transform child in transform)
        {
            GridTileBase tile = child.GetComponent<GridTileBase>();
            if (tile != null)
                allTiles.Add(tile);
        }

        // Duyệt toàn bộ tile và áp dụng quy tắc
        foreach (var tile in allTiles)
        {
            // Debug.Log($"Danh sách tile: {tile.GetTileType()} tại {tile.Coordinates}");
            if (tile is WaterLakeTile)
            {
                if (newType != TileType.T02_Water_Lake) continue; // Chỉ xử lý nếu cái mới là hồ nước
                if (newTile != tile) continue; // Chỉ xử lý hồ mới được tạo

                // Debug.Log($"Xử lý tile nước: {tile.GetTileType()} tại {tile.Coordinates}");
                // Quy tắc: Hồ làm đất khô xung quanh thành cỏ
                foreach (var neighbor in tile.GetNeighbors())
                {
                    // Debug.Log($"Xử lý tile lân cận: {neighbor.GetTileType()} tại {neighbor.Coordinates}");
                    if (neighbor is BarrenLandTile barren)
                    {
                        // barren.ReplaceTile(TileType.T03_Green_Grass, tilePrefabs[2]);
                        StartCoroutine(ReplaceTile(barren, TileType.T03_Green_Grass, GetTilePrefab(TileType.T03_Green_Grass)));
                    }
                }
            }
            // else if (tile is GreenGrassTile)
            // {
            //     // Quy tắc: Nếu không còn hồ bên cạnh → biến thành đất khô
            //     bool hasLakeNearby = false;
            //     foreach (var neighbor in tile.GetNeighbors())
            //     {
            //         if (neighbor is WaterLakeTile)
            //         {
            //             // Debug.Log($"{tile.GetTileType()} Có hồ bên cạnh: {neighbor.GetTileType()} tại {neighbor.Coordinates}");
            //             hasLakeNearby = true;
            //             break;
            //         }
            //     }
            //     if (!hasLakeNearby)
            //     {
            //         // tile.ReplaceTile(TileType.T01_Barren_Land, tilePrefabs[0]);
            //         StartCoroutine(ReplaceTile(tile, TileType.T01_Barren_Land, GetTilePrefab(TileType.T01_Barren_Land)));
            //     }
            // }
            else if (tile is GreenTreeTile)
            {
                if (newType != TileType.T04_Green_Tree) continue; // Chỉ xử lý nếu cái mới là cây xanh
                if (newTile != tile) continue; // Chỉ xử lý cây mới được tạo

                var connectedTrees = GetConnectedGreenTrees(newTile);
                // Debug.Log($"{newTile.GetTileType()} có {connectedTrees.Count} cây kết nối với cây mới tại {newTile.Coordinates}");
                if (connectedTrees.Count >= 3)
                {
                    // Debug.Log($"Cây mới loại {newType} tại {newTile.Coordinates} đã kết nối với {connectedTrees.Count} cây khác");
                    // Ô cây mới thành Rừng
                    StartCoroutine(ReplaceTile(newTile, TileType.T05_Forest, GetTilePrefab(TileType.T05_Forest)));

                    if (connectedTrees.Count == 3)
                    {
                        // Các cây khác thành đất khô
                        foreach (var tree in connectedTrees)
                        {
                            if (tree != newTile)
                                StartCoroutine(ReplaceTile(tree, TileType.T01_Barren_Land, GetTilePrefab(TileType.T01_Barren_Land)));
                        }
                    }
                    else if (connectedTrees.Count == 4)
                    {
                        // Các cây khác thành cỏ
                        foreach (var tree in connectedTrees)
                        {
                            if (tree != newTile)
                                StartCoroutine(ReplaceTile(tree, TileType.T03_Green_Grass, GetTilePrefab(TileType.T03_Green_Grass)));
                        }
                    }
                    else if (connectedTrees.Count > 4)
                    {
                        // Tạo danh sách cây, nhưng loại bỏ cây mới ra trước
                        var otherTrees = new List<GridTileBase>(connectedTrees);
                        otherTrees.Remove(newTile);

                        // Sắp xếp otherTrees theo khoảng cách đến cây mới (gần -> xa)
                        otherTrees.Sort((a, b) =>
                            Vector3Int.Distance(a.Coordinates, newTile.Coordinates)
                            .CompareTo(Vector3Int.Distance(b.Coordinates, newTile.Coordinates)));

                        // Cây mới -> rừng
                        // StartCoroutine(ReplaceTile(newTile, TileType.T05_Forest, GetTilePrefab(TileType.T05_Forest)));

                        // 2 cây gần nhất -> rừng
                        for (int i = 0; i < Mathf.Min(2, otherTrees.Count); i++)
                        {
                            StartCoroutine(ReplaceTile(otherTrees[i], TileType.T05_Forest, GetTilePrefab(TileType.T05_Forest)));
                        }

                        // Các cây còn lại -> cỏ
                        for (int i = 2; i < otherTrees.Count; i++)
                        {
                            StartCoroutine(ReplaceTile(otherTrees[i], TileType.T03_Green_Grass, GetTilePrefab(TileType.T03_Green_Grass)));
                        }
                    }
                }
            }
        }
    }

    public IEnumerator ReplaceTile(GridTileBase tile, TileType newType, GameObject prefab)
    {
        tile.LiftTile();
        // Đợi một chút để hiệu ứng nhấc tile có thời gian hiển thị   
        yield return new WaitForSeconds(0.2f);
        tile.ResetTilePosition();
        tile.ReplaceTile(newType, prefab);
    }

    /// <summary>
    /// Trả về danh sách các cây xanh liên kết với tile bắt đầu. Bằng thuật BFS.
    /// </summary>
    /// <param name="startTile">Tile bắt đầu</param>
    /// <returns>Danh sách các cây xanh liên kết</returns>
    public List<GridTileBase> GetConnectedGreenTrees(GridTileBase startTile)
    {
        var visited = new HashSet<GridTileBase>();
        var queue = new Queue<GridTileBase>();
        var result = new List<GridTileBase>();

        queue.Enqueue(startTile);
        visited.Add(startTile);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            result.Add(current);

            foreach (var neighbor in current.GetNeighbors())
            {
                if (!visited.Contains(neighbor) && neighbor is GreenTreeTile)
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }

        return result;
    }

}
