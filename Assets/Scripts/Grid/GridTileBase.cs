// GridTileBase: Lớp cơ sở cho các tile trong lưới, chứa tọa độ và phương thức khởi tạo.

using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.Tilemaps;
using System.Linq;
using DG.Tweening;
using NUnit.Framework;

public abstract class GridTileBase : MonoBehaviour
{
    // ----- Grid -----
    public Vector3Int Coordinates { get; private set; } // Tọa độ của tile trong Grid
    protected GridMapManager gridManager;

    // ----- Mission and Animal -----
    protected MissionManager missionManager;
    protected AnimalManager animalManager;

    // ----- Layer -----
    protected SpriteRenderer spriteRenderer;
    public int originalLayer;

    // ----- Position -----
    [HideInInspector] public Vector3 originalPosition;
    private float liftAmount = 0.1f; // Khoảng nhích trục Y

    public int LockCount { get; private set; } = 0;
    public bool IsLocked => LockCount > 0;
    private Color normalColor = Color.white;
    private Color lockedColor = new Color(0.4f, 0.4f, 0.4f, 1f);  // màu tối



    /// <summary>
    /// Khởi tạo tile với tọa độ xác định.
    /// </summary>
    /// <param name="coordinates">Tọa độ trong Grid.</param>
    public virtual void Init(Vector3Int coordinates)
    {
        Coordinates = coordinates;
        gameObject.name = $"{GetTileType()}_{Coordinates}";

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

        if (animalManager == null)
        {
            animalManager = FindObjectOfType<AnimalManager>();
        }
    }

    /// <summary>
    /// Trả về tên loại tile (cần được override ở lớp con).
    /// </summary>
    public abstract TileType GetTileType();

    /// <summary>
    /// Xử lý sự kiện nhấn giữ trên tile (cần được override ở lớp con).
    /// </summary>
    public abstract void UpgradeTile();

    /// <summary>
    /// Nhấc tile lên một chút.
    /// </summary>
    public void LiftTile()
    {
        animalManager.ShowMarkerIcon(false);

        // Dừng tween cũ (nếu có) để tránh xung đột
        transform.DOKill();

        // Tween nhấc lên trên
        transform.DOMove(originalPosition + Vector3.up * liftAmount, 0.2f)
                 .SetEase(Ease.OutQuad)
                 .SetLink(gameObject); // khi gameObject destroy => tween tự hủy


        // Đổi sorting order sau khi bắt đầu lift
        spriteRenderer.sortingOrder = originalLayer + 1;
    }


    /// <summary>
    /// Đặt lại vị trí của tile về vị trí ban đầu.
    /// </summary>
    public void ResetTilePosition()
    {
        // Dừng mọi tween trên transform để nó không override
        transform.DOKill();

        animalManager.ShowMarkerIcon(true);
        transform.position = originalPosition;
        spriteRenderer.sortingOrder = originalLayer;
    }

    /// <summary>
    /// Thay thế loại tile hiện tại bằng loại mới.
    /// </summary>
    /// <param name="newType">Loại tile mới.</param>
    /// <param name="prefab">Prefab của tile mới.</param>
    public void ReplaceTile(TileType newType, GameObject prefab, bool updateMap = true, bool isSwap = false, bool playSound = false)
    {
        if (gridManager == null) return;

        if (HasAnimalInRegion())
        {
            // Debug.Log($"Tile {Coordinates} thuộc vùng có động vật -> không thay đổi");
            return;
        }

        // Cập nhật vào data ảo
        gridManager.SetTempTile(Coordinates, newType);

        // Tạo tile mới
        GridTileBase newTile = Instantiate(prefab, originalPosition, transform.rotation, transform.parent)
                                    .GetComponent<GridTileBase>();
        if (newTile != null)
            newTile.Init(Coordinates);

        if (playSound)
            gridManager.audioManager.PlaySFXPop();

        // Hủy tile cũ
        Destroy(gameObject);

        // Thêm điểm
        if (Enum.TryParse(newType.ToString(), out TileScore score))
        {
            missionManager.uiManager.AddScore((int)score);
            // Debug.Log($"Thêm điểm: {(int)score} cho loại tile {newType}");
        }

        // Đánh nhiệm vụ
        if (missionManager != null && !isSwap)
            missionManager.CollectTile(newType, transform.position);

        // Gọi UpdateMapState sau 1 frame từ gridManager
        if (updateMap)
            gridManager.StartCoroutine(DelayUpdateMap(gridManager, newType, newTile));
    }

    private IEnumerator DelayUpdateMap(GridMapManager gridManager, TileType newType, GridTileBase newTile)
    {
        yield return null; // chờ 1 frame
        gridManager.UpdateMapState(newType, newTile);
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

    /// <summary>
    /// Kiểm tra nhanh xem tile này có nằm trong region nào đang chứa animal không.
    /// </summary>
    private bool IsInRegionWithAnimal(out Region foundRegion)
    {
        foundRegion = null;

        if (animalManager == null || gridManager == null) return false;

        var regions = gridManager.GetRegionsByTileType(GetTileType());

        foreach (var region in regions)
        {
            if (!region.Tiles.Contains(this)) continue;

            if (animalManager.currentAnimal.TryGetValue(region.Id, out var animals) && animals.Count > 0)
            {
                foundRegion = region;
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Kiểm tra tile này có thuộc một region chứa động vật, 
    /// và nếu region đó đã FULL thì không cho thay đổi.
    /// </summary>
   public bool HasAnimalInRegion()
    {
        if (!IsInRegionWithAnimal(out var region))
            return false; // tile này chưa có animal trong region → đổi thoải mái

        // Nếu tile không phải là tile ở rìa thì không cho thay đổi
        if (!IsEdgeTile(this, region))
        {
            Debug.LogWarning($"Tile {Coordinates} không phải tile rìa của region {region.Id}.");
            return true;
        }

        // Nếu region có animal thì tính dung lượng
        if (animalManager.currentAnimal.TryGetValue(region.Id, out var animals) && animals.Count > 0)
        {
            int totalAnimalSize = 0;
            foreach (var a in animals)
            {
                if (Enum.TryParse(a.ToString(), out AnimalSize sizeAnimal))
                {
                    totalAnimalSize += (int)sizeAnimal;
                }
            }

            // Nếu dung lượng animal >= số tile → FULL → KHÔNG cho thay đổi
            if (totalAnimalSize >= region.Tiles.Count)
            {
                Debug.LogWarning($"Region {region.Id} đã FULL với {totalAnimalSize} con trên {region.Tiles.Count} ô.");
                return true;
            }
        }

        // Di chuyển marker trong region nếu tile cũ bị thay.
        animalManager.RelocateMarker(region, this);

        return false;
    }

    /// <summary>
    /// Kiểm tra xem tile có phải là tile ở rìa của region không.
    /// Với hex: nếu neighbors cùng region không liền kề (rời rạc) thì tile coi như trong lòng.
    /// </summary>
    private bool IsEdgeTile(GridTileBase tile, Region region)
    {
        var neighbors = tile.GetNeighbors(); // 6 neighbors (theo Even-Q / Odd-Q)

        // Đánh dấu 6 hướng theo vòng tròn
        bool[] mask = new bool[6];
        for (int i = 0; i < neighbors.Length; i++)
        {
            var neighbor = neighbors[i];
            mask[i] = (neighbor != null && region.Tiles.Contains(neighbor));
        }

        int countInRegion = mask.Count(m => m);

        // Nếu ít hơn 2 neighbor cùng region → chắc chắn là rìa
        if (countInRegion < 2)
            return true;

        // Nếu đủ 4 neighbor cùng region → chắc chắn nằm trong lòng
        if (countInRegion > 4)
            return false;

        // Kiểm tra xem các neighbor true có liền kề thành một dải hay bị chia tách
        int segments = 0;
        for (int i = 0; i < 6; i++)
        {
            int next = (i + 1) % 6;
            if (mask[i] && !mask[next])
                segments++;
        }

        // Nếu nhiều hơn 1 đoạn neighbor rời rạc → coi như trong lòng (NOT edge)
        if (segments > 1)
            return false;

        // Còn lại là rìa
        return true;
    }

    public void SetLockCount(int value)
    {
        LockCount = Mathf.Max(0, value);

        if (spriteRenderer != null)
        {
            spriteRenderer.color = IsLocked ? new Color(0.55f, 0.55f, 0.55f) : Color.white;
        }
    }

    public void ReduceLockPoint(int amount = 1)
    {
        SetLockCount(LockCount - amount);
    }

}
