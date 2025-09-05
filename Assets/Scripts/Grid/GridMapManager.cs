// GridMapManager: Tạo và quản lý lưới tile trong Scene dựa trên dữ liệu từ GridMapData.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Constraints;
using UnityEngine;

[RequireComponent(typeof(Grid))]
public class GridMapManager : MonoBehaviour
{
    [Header("Grid Data")]
    private LevelData mapData;
    private LevelManager levelManager;

    [Header("Tile Prefabs")]
    [SerializeField] private GameObject[] tilePrefabs;

    private Grid grid;
    public Dictionary<Vector3Int, TileType> tempTileStates = new Dictionary<Vector3Int, TileType>();

    // Animal
    private List<GridTileBase> highlightedTiles = new List<GridTileBase>();
    private List<GridTileBase> shadowedTiles = new List<GridTileBase>();

    // Region
    private List<Region> currentRegions = new List<Region>();
    private int nextRegionId = 1; // Tự tăng Id cho vùng mới

    public void SetLevelData(LevelData data, LevelManager levelManager)
    {
        mapData = data;
        this.levelManager = levelManager;
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
            case TileType.T08_Yellow_Grass:
                return tilePrefabs[7];
            case TileType.T09_Xavan:
                return tilePrefabs[8];
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
        {
            if (child.name != "Marker Container")
            {
                Destroy(child.gameObject);
            }
        }


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
            case TileType.T04_Green_Tree:
                return GetTilePrefab(TileType.T04_Green_Tree);
            case TileType.T05_Forest:
                return GetTilePrefab(TileType.T05_Forest);
            case TileType.T06_Flower_Field:
                return GetTilePrefab(TileType.T06_Flower_Field);
            case TileType.T07_Rock_Mountain:
                return GetTilePrefab(TileType.T07_Rock_Mountain);
            case TileType.T08_Yellow_Grass:
                return GetTilePrefab(TileType.T08_Yellow_Grass);
            case TileType.T09_Xavan:
                return GetTilePrefab(TileType.T09_Xavan);
            default:
                return null;
        }
    }

    // ========== Grid Rules =========
    /// <summary>
    /// Cập nhật trạng thái toàn bộ bản đồ dựa trên quy tắc.
    /// </summary>
    public void UpdateMapState(TileType newType, GridTileBase newTile)
    {
        var allTiles = GetAllTiles();

        foreach (var tile in allTiles)
        {
            if (tile is WaterLakeTile)
            {
                HandleWaterLake(tile, newType, newTile);
            }
            else if (tile is GreenTreeTile)
            {
                HandleGreenTree(tile, newType, newTile);
            }
            else if (tile is YellowGrassTile)
            {
                HandleYellowGrass(tile, newType, newTile);
            }
        }

        // Chỉ cập nhật khi có nhiệm vụ động vật đang hoạt động
        if (levelManager.missionManager.GetCurrentMission().Item2.Count > 0)
            UpdateCurrentRegionTiles(allTiles);
    }

    /// <summary>
    /// Lấy toàn bộ tile trong bản đồ.
    /// </summary>
    private List<GridTileBase> GetAllTiles()
    {
        var allTiles = new List<GridTileBase>();
        foreach (Transform child in transform)
        {
            GridTileBase tile = child.GetComponent<GridTileBase>();
            if (tile != null)
                allTiles.Add(tile);
        }
        return allTiles;
    }

    /// <summary>
    /// Xử lý quy tắc khi tạo hồ nước.
    /// </summary>
    private void HandleWaterLake(GridTileBase tile, TileType newType, GridTileBase newTile)
    {
        if (newType != TileType.T02_Water_Lake || newTile != tile) return;

        foreach (var neighbor in tile.GetNeighbors())
        {
            // Nếu gần hồ nước là đất
            if (neighbor is BarrenLandTile barren)
            {
                int rockCount = 0;
                int lakeCount = 0;

                foreach (var subNeighbor in barren.GetNeighbors())
                {
                    if (subNeighbor is RockMountainTile)
                        rockCount++;
                    else if (subNeighbor is WaterLakeTile && subNeighbor != newTile)
                        lakeCount++;
                }

                if (rockCount > 0)
                {
                    // Đất cạnh ít nhất 1 đá -> Cỏ vàng
                    StartCoroutine(ReplaceTile(
                        barren,
                        TileType.T08_Yellow_Grass,
                        GetTilePrefab(TileType.T08_Yellow_Grass)
                    ));
                }
                else if (lakeCount > 0)
                {
                    // Đất cạnh ít nhất 2 hồ nước(kể cả tile mới) -> Hoa
                    StartCoroutine(ReplaceTile(
                        barren,
                        TileType.T06_Flower_Field,
                        GetTilePrefab(TileType.T06_Flower_Field)
                    ));
                }
                else
                {
                    // Các trường hợp còn lại -> Cỏ thường
                    StartCoroutine(ReplaceTile(
                        barren,
                        TileType.T03_Green_Grass,
                        GetTilePrefab(TileType.T03_Green_Grass)
                    ));
                }
            }


            // Nếu gần hồ nước là cỏ thì thành hoa
            if (neighbor is GreenGrassTile greenGrass)
            {
                foreach (var subNeighbor in greenGrass.GetNeighbors())
                {
                    if (subNeighbor is WaterLakeTile lake && subNeighbor != newTile)
                    {
                        StartCoroutine(ReplaceTile(
                            greenGrass,
                            TileType.T06_Flower_Field,
                            GetTilePrefab(TileType.T06_Flower_Field)
                        ));
                        break;
                    }
                }
            }
        }
    }


    /// <summary>
    /// Xử lý quy tắc khi tạo cây xanh.
    /// </summary>
    private void HandleGreenTree(GridTileBase tile, TileType newType, GridTileBase newTile)
    {
        if (newType != TileType.T04_Green_Tree || newTile != tile) return;

        var connectedTrees = GetConnectedTiles(newTile, t => t is GreenTreeTile);

        if (connectedTrees.Count < 3) return;

        // Cây mới thành rừng
        StartCoroutine(ReplaceTile(newTile, TileType.T05_Forest, GetTilePrefab(TileType.T05_Forest)));

        if (connectedTrees.Count == 3)
        {
            ReplaceOtherTrees(connectedTrees, newTile, TileType.T01_Barren_Land);
        }
        else if (connectedTrees.Count == 4)
        {
            ReplaceOtherTrees(connectedTrees, newTile, TileType.T03_Green_Grass);
        }
        else if (connectedTrees.Count > 4)
        {
            HandleLargeTreeCluster(newTile, connectedTrees);
        }
    }

    /// <summary>
    /// Thay thế toàn bộ cây kết nối trừ cây mới.
    /// </summary>
    private void ReplaceOtherTrees(List<GridTileBase> connectedTrees, GridTileBase newTile, TileType type)
    {
        foreach (var tree in connectedTrees)
        {
            if (tree != newTile)
                StartCoroutine(ReplaceTile(tree, type, GetTilePrefab(type)));
        }
    }

    /// <summary>
    /// Xử lý khi số lượng cây kết nối > 4.
    /// </summary>
    private void HandleLargeTreeCluster(GridTileBase newTile, List<GridTileBase> connectedTrees)
    {
        var otherTrees = new List<GridTileBase>(connectedTrees);
        otherTrees.Remove(newTile);

        otherTrees.Sort((a, b) =>
            Vector3Int.Distance(a.Coordinates, newTile.Coordinates)
            .CompareTo(Vector3Int.Distance(b.Coordinates, newTile.Coordinates)));

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

    /// <summary>
    /// Xử lý quy tắc khi tạo cỏ vàng.
    /// </summary>
    private void HandleYellowGrass(GridTileBase tile, TileType newType, GridTileBase newTile)
    {
        if (newType != TileType.T08_Yellow_Grass || newTile != tile) return;

        var connectedYellowGrass = GetConnectedTiles(newTile, t => t is YellowGrassTile);

        if (connectedYellowGrass.Count >= 3)
        {
            // Chỉ ô cỏ vàng mới thành Xavan
            StartCoroutine(ReplaceTile(newTile, TileType.T09_Xavan, GetTilePrefab(TileType.T09_Xavan)));
        }
    }

    /// <summary>
    /// Thay thế tile hiện tại bằng loại mới và prefab tương ứng - Có hiệu ứng
    /// </summary>
    /// <param name="tile">tile hiện tại</param>
    /// <param name="newType">loại tile mới</param>
    /// <param name="prefab">prefab của tile mới</param>
    public IEnumerator ReplaceTile(GridTileBase tile, TileType newType, GameObject prefab)
    {
        // // Kiểm tra xem tile này có thuộc vùng có động vật không
        // foreach (var region in currentRegions)
        // {
        //     if (region.ContainsTile(tile) &&
        //         levelManager.animalManager.currentAnimal.ContainsKey(region.Id))
        //     {
        //         Debug.Log($"Tile {tile.Coordinates} thuộc Region {region.Id} có động vật -> không thay đổi");
        //         yield break;
        //     }
        // }

        tile.LiftTile();
        yield return new WaitForSeconds(0.2f);
        tile.ResetTilePosition();
        tile.ReplaceTile(newType, prefab);
    }


    // ========== Xử lý vùng và động vật =========

    /// <summary>
    /// Trả về danh sách các tile liên kết với tile bắt đầu theo điều kiện cho trước. (BFS)
    /// </summary>
    /// <param name="startTile">Tile bắt đầu</param>
    /// <param name="match">Điều kiện kiểm tra tile (ví dụ: tile => tile is GreenTreeTile)</param>
    /// <returns>Danh sách các tile liên kết</returns>
    public List<GridTileBase> GetConnectedTiles(GridTileBase startTile, Func<GridTileBase, bool> match)
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
                if (!visited.Contains(neighbor) && match(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Cập nhật và lưu toàn bộ các vùng trên bản đồ.
    /// </summary>
    public void UpdateCurrentRegionTiles(List<GridTileBase> allTiles)
    {
        // Debug.LogWarning("Cập nhật vùng hiện tại...");
        var newRegions = new List<Region>();
        HashSet<GridTileBase> visited = new HashSet<GridTileBase>();

        foreach (var tile in allTiles)
        {
            if (visited.Contains(tile)) continue;

            var connectedTiles = GetConnectedTiles(tile, t => t.GetTileType() == tile.GetTileType());
            foreach (var t in connectedTiles)
                visited.Add(t);

            // Tìm xem có region cũ nào overlap không
            Region oldRegion = currentRegions.FirstOrDefault(r => r.Tiles.Intersect(connectedTiles).Any());

            if (oldRegion != null)
            {
                // Giữ lại Id cũ
                Region newRegion = new Region(oldRegion.Id, tile.GetTileType());
                foreach (var t in connectedTiles)
                    newRegion.AddTile(t);
                newRegions.Add(newRegion);
            }
            else
            {
                // Tạo region mới
                Region newRegion = new Region(nextRegionId++, tile.GetTileType());
                foreach (var t in connectedTiles)
                    newRegion.AddTile(t);
                newRegions.Add(newRegion);
            }
        }

        currentRegions = newRegions;
    }


    /// <summary>
    /// Lấy tất cả các vùng theo loại tile.
    /// </summary>
    public List<Region> GetRegionsByTileType(TileType type)
    {
        List<Region> result = new List<Region>();

        foreach (var region in currentRegions)
        {
            if (region.Type == type)
            {
                result.Add(region);
            }
        }

        return result;
    }

    public bool GetRegionById(int regionId, out Region region)
    {
        region = currentRegions.FirstOrDefault(r => r.Id == regionId);
        return region != null;
    }

    /// <summary>
    /// Làm nổi bật vùng (region) cho động vật dựa trên loại tile.
    /// </summary>
    /// <param name="animalType">loại động vật</param>
    public void HighlightRegion(AnimalType animalType, List<Vector3Int> inappropriateRegions)
    {
        ClearHighlight();

        // Lấy vùng hợp lệ dựa trên AnimalRegion
        TileType allowed = TileType.T00_Null;

        if (Enum.TryParse(animalType.ToString(), out AnimalRegion tile))
            allowed = (TileType)tile;

        // Debug.Log($"Làm nổi bật vùng cho động vật: {animalType} với loại tile: {allowed}");

        // Debug.Log($"Danh sách các tile không phù hợp");

        foreach (var tileCoordinate in tempTileStates.Keys)
        {
            GridTileBase t = GetTileAt(tileCoordinate);
            if (t != null && (t.GetTileType() == allowed) && !inappropriateRegions.Contains(tileCoordinate))
            {
                t.LiftTile();
                highlightedTiles.Add(t);
            }
            else
            {
                var sr = t.GetComponentInChildren<SpriteRenderer>();
                if (sr != null)
                {
                    sr.color = new Color(0.3f, 1, 1); // làm mờ
                    shadowedTiles.Add(t);
                }
            }
        }
    }

    /// <summary>
    /// Xóa bỏ hiệu ứng làm nổi bật vùng (region) cho động vật.
    /// </summary>
    public void ClearHighlight()
    {
        foreach (var t in highlightedTiles)
        {
            if (t != null)
            {
                t.ResetTilePosition();
            }
        }

        foreach (var t in shadowedTiles)
        {
            if (t != null)
            {
                var sr = t.GetComponentInChildren<SpriteRenderer>();
                if (sr != null)
                    sr.color = Color.white; // reset về màu cũ
            }
        }
        
        highlightedTiles.Clear();
        shadowedTiles.Clear();
    }
}
