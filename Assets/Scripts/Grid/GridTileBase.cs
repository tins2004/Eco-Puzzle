// GridTileBase: Lớp cơ sở cho các tile trong lưới, chứa tọa độ và phương thức khởi tạo.

using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public abstract class GridTileBase : MonoBehaviour
{
    // ----- Grid -----
    public Vector3Int Coordinates { get; private set; } // Tọa độ của tile trong Grid
    protected GridMapManager gridManager;

    // ----- Mission -----
    protected MissionManager missionManager;

    // ----- Layer -----
    protected SpriteRenderer spriteRenderer;
    private int originalLayer;

    // ----- Position -----
    [HideInInspector] public Vector3 originalPosition;
    [SerializeField] private float liftAmount = 0.2f; // Khoảng nhích trục Y


    /// <summary>
    /// Khởi tạo tile với tọa độ xác định.
    /// </summary>
    /// <param name="coordinates">Tọa độ trong Grid.</param>
    public virtual void Init(Vector3Int coordinates)
    {
        Coordinates = coordinates;

        if (spriteRenderer == null)
            spriteRenderer = transform.Find("Sprite")?.GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = -(int)(Coordinates.y * 2);
            originalLayer = spriteRenderer.sortingOrder;
        }

        if (gridManager == null)
        {
            gridManager = FindObjectOfType<GridMapManager>();
        }

        if (missionManager == null)
        {
            missionManager = FindObjectOfType<MissionManager>();
        }
    }

    /// <summary>
    /// Trả về tên loại tile (cần được override ở lớp con).
    /// </summary>
    public abstract string GetTileType();

    /// <summary>
    /// Xử lý sự kiện nhấn giữ trên tile (cần được override ở lớp con).
    /// </summary>
    public abstract void HandleLongPress();

    /// <summary>
    /// Nhấc tile lên một chút.
    /// </summary>
    public void LiftTile()
    {
        transform.position = originalPosition + Vector3.up * liftAmount;
        spriteRenderer.sortingOrder = originalLayer + 1;
    }

    /// <summary>
    /// Đặt lại vị trí của tile về vị trí ban đầu.
    /// </summary>
    public void ResetTilePosition()
    {
        transform.position = originalPosition;
        spriteRenderer.sortingOrder = originalLayer;
    }

    /// <summary>
    /// Thay thế loại tile hiện tại bằng loại mới.
    /// </summary>
    /// <param name="newType">Loại tile mới.</param>
    /// <param name="prefab">Prefab của tile mới.</param>
    public void ReplaceTile(TileType newType, GameObject prefab, bool updateMap = true)
    {
        if (gridManager == null) return;

        // Cập nhật vào data ảo
        gridManager.SetTempTile(Coordinates, newType);

        // Tạo tile mới
        GridTileBase newTile = Instantiate(prefab, transform.position, transform.rotation, transform.parent)
                                    .GetComponent<GridTileBase>();
        if (newTile != null)
            newTile.Init(Coordinates);

        // Hủy tile cũ
        Destroy(gameObject);

        // Thêm điểm
        AddScoreByNewTile(newType);

        // Đánh nhiệm vụ
        if (missionManager != null)
            missionManager.CollectTile(newType);

        // Gọi UpdateMapState sau 1 frame từ gridManager
        if (updateMap)
            gridManager.StartCoroutine(DelayUpdateMap(gridManager, newType, newTile));
    }

    private IEnumerator DelayUpdateMap(GridMapManager gridManager, TileType newType, GridTileBase newTile)
    {
        yield return null; // chờ 1 frame
        gridManager.UpdateMapState(newType, newTile);
    }

    private void AddScoreByNewTile(TileType newType)
    {
        switch (newType)
        {
            case TileType.T01_Barren_Land:
                missionManager.uiManager.AddScore((int)TileScore.T01_Barren_Land);
                break;
            case TileType.T02_Water_Lake:
                missionManager.uiManager.AddScore((int)TileScore.T02_Water_Lake);
                break;
            case TileType.T03_Green_Grass:
                missionManager.uiManager.AddScore((int)TileScore.T03_Green_Grass);
                break;
            case TileType.T04_Green_Tree:
                missionManager.uiManager.AddScore((int)TileScore.T04_Green_Tree);
                break;
            case TileType.T05_Forest:
                missionManager.uiManager.AddScore((int)TileScore.T05_Forest);
                break;
            case TileType.T06_Flower_Field:
                missionManager.uiManager.AddScore((int)TileScore.T06_Flower_Field);
                break;
            case TileType.T07_Rock_Mountain:
                missionManager.uiManager.AddScore((int)TileScore.T07_Rock_Mountain);
                break;
            case TileType.T08_Yellow_Grass:
                missionManager.uiManager.AddScore((int)TileScore.T08_Yellow_Grass);
                break;
            case TileType.T09_Xavan:
                missionManager.uiManager.AddScore((int)TileScore.T09_Xavan);
                break;
            default:
                Debug.LogWarning($"Không có điểm cho loại tile: {newType}");
                break;
        }
    }

    /// <summary>
    /// Trả về danh sách các tile lân cận.
    /// </summary>
    /// <returns>Danh sách các tile lân cận.</returns>
    public GridTileBase[] GetNeighbors()
    {
        if (gridManager == null) return new GridTileBase[0];

        var neighbors = new List<GridTileBase>();

        int x = Coordinates.x;
        int y = Coordinates.y;

        // Offset cho hex Even-Q
        Vector2Int[] evenQOffsets = new Vector2Int[]
        {
        new Vector2Int(+1,  0),
        new Vector2Int( 0, +1),
        new Vector2Int(-1, +1),
        new Vector2Int(-1,  0),
        new Vector2Int(-1, -1),
        new Vector2Int( 0, -1),
        };

        Vector2Int[] oddQOffsets = new Vector2Int[]
        {
        new Vector2Int(+1,  0),
        new Vector2Int(+1, +1),
        new Vector2Int( 0, +1),
        new Vector2Int(-1,  0),
        new Vector2Int( 0, -1),
        new Vector2Int(+1, -1),
        };

        bool isEvenCol = y % 2 == 0;
        Vector2Int[] directions = isEvenCol ? evenQOffsets : oddQOffsets;

        foreach (var dir in directions)
        {
            Vector3Int neighborPos = new Vector3Int(x + dir.x, y + dir.y, 0);

            // Kiểm tra trong tempTileStates
            if (gridManager.tempTileStates.TryGetValue(neighborPos, out TileType type))
            {
                GridTileBase neighborTile = gridManager.GetTileAt(neighborPos);
                if (neighborTile != null)
                    neighbors.Add(neighborTile);
            }
        }

        return neighbors.ToArray();
    }
}
