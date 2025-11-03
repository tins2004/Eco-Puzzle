using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoosterUpgrade : BoosterBase
{
    [SerializeField] private Sprite[] rainEffectSprites;

    protected override void SetBoosterNumberKey()
    {
        boosterNumberKey = GameData.GetBoosterUpgradeKey();
    }

    public override void Activate()
    {
        Debug.Log("Bắt đầu chọn 1 tile để nâng cấp...");
        isSelecting = true;
    }

    public override void CancelActivate()
    {
        Debug.Log("Hủy nâng cấp.");

        isSelecting = false;
    }

    /// <summary>
    /// Gọi khi người chơi chọn một tile.
    /// </summary>
    public void OnTileSelected(GridTileBase tile)
    {
        if (!isSelecting) return;
        if (tile == null) return;

        // Nếu tile có động vật thì không cho chọn
        if (tile.HasAnimalInRegion())
        {
            // Debug.LogWarning($"Tile {tile.Coordinates} có động vật → không được chọn.");
            levelManager.DisplayLogWarning("Đã có động vật trong vùng này!\nKhông thể nâng cấp.");
            return;
        }

        Debug.Log($"Tile nâng cấp: {tile.name}");
        StartCoroutine(UpgradeTiles(tile));
    }

    private IEnumerator UpgradeTiles(GridTileBase tile)
    {
        canClick = false;
        // // Tween 
        // Sequence seq = DOTween.Sequence();
        // seq.AppendInterval(0.2f);
        // seq.Join(tile.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack));
        // seq.AppendInterval(0.2f);
        // seq.AppendCallback(() =>
        // {
        //     tile.ReplaceTile(UpgradeTileLogic(tile.GetTileType()), gridManager.GetTilePrefab(UpgradeTileLogic(tile.GetTileType())));
        // });

        // yield return seq.WaitForCompletion();

        // isSelecting = false;
        // Consume();

        // Tạo hiệu ứng sprite trên tile
        GameObject effectObj = new GameObject("UpgradeEffect");
        SpriteRenderer sr = effectObj.AddComponent<SpriteRenderer>();

        // Đặt vị trí ngay trên tile
        effectObj.transform.position = tile.transform.position + Vector3.up * 0.3f;
        effectObj.transform.localScale = Vector3.one * 0.3f;
        sr.sortingOrder = tile.originalLayer + 10; // cho nó nổi lên trên tile

        float totalDuration = 2.5f;
        float frameDelay = 0.05f;

        int frameCount = Mathf.FloorToInt(totalDuration / frameDelay);

        gridManager.audioManager.PlaySFXRain();

        for (int i = 0; i < frameCount; i++)
        {
            sr.sprite = rainEffectSprites[i % rainEffectSprites.Length];

            if (i == (frameCount / 3))
            {
                gridManager.audioManager.PlaySFXPop();
                tile.ReplaceTile(UpgradeTileLogic(tile.GetTileType()), gridManager.GetTilePrefab(UpgradeTileLogic(tile.GetTileType())));
            }

            yield return new WaitForSeconds(frameDelay);
        }

        gridManager.audioManager.StopSFX();
        gridManager.audioManager.PlaySFXFailClick();
        effectObj.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack)
        .OnComplete(() =>
        {
            Destroy(effectObj);

            isSelecting = false;
            canClick = true;
            Consume();
        });
        
    }

    private TileType UpgradeTileLogic(TileType type)
    {
        switch (type)
        {
            case TileType.T01_Barren_Land: // Đất ra cỏ
                return TileType.T03_Green_Grass;
            case TileType.T02_Water_Lake: // Nước ra cỏ
                return TileType.T01_Barren_Land;
            case TileType.T03_Green_Grass: // Cỏ ra hoa
                return TileType.T06_Flower_Field;
            case TileType.T04_Green_Tree: // Cây ra rừng
                return TileType.T05_Forest;
            case TileType.T05_Forest: // Rừng ra hoa
                return TileType.T06_Flower_Field;
            case TileType.T06_Flower_Field: // Hoa ra rừng
                return TileType.T05_Forest;
            case TileType.T07_Rock_Mountain: // Núi đá ra cỏ vàng
                return TileType.T08_Yellow_Grass;
            case TileType.T08_Yellow_Grass: // Cỏ vàng ra cây
                return TileType.T04_Green_Tree;
            case TileType.T09_Xavan: // Đồng cỏ vàng ra rừng
                return TileType.T05_Forest;
            default:
                return TileType.T01_Barren_Land;
        }
    }
}
