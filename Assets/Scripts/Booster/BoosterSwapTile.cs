using System.Collections;
using DG.Tweening;
using UnityEngine;

public class BoosterSwapTile : BoosterBase
{
    private GridTileBase firstTile;

    protected override void SetBoosterNumberKey()
    {
        boosterNumberKey = GameData.GetBoosterSwapKey();
    }

    public override void Activate()
    {
        Debug.Log("Bắt đầu chọn 2 tile để hoán đổi...");
        isSelecting = true;
    }

    public override void CancelActivate()
    {
        Debug.Log("Hủy hoán đổi.");

        isSelecting = false;
        if (firstTile != null)
            firstTile.ResetTilePosition();
        firstTile = null;
    }

    /// <summary>
    /// Gọi khi người chơi chọn một tile.
    /// </summary>
    public void OnTileSelected(GridTileBase tile)
    {
        if (!isSelecting) return;
        if (tile == null) return;

        gridManager.audioManager.PlaySFXPop();

        // Nếu tile có động vật thì không cho chọn
        if (tile.HasAnimalInRegion())
        {
            // Debug.LogWarning($"Tile {tile.Coordinates} có động vật → không được chọn.");
            levelManager.DisplayLogWarning("Đã có động vật trong vùng này!\nKhông thể hoán đổi.");

            return;
        }

        // Chọn tile đầu tiên
        if (firstTile == null)
        {
            firstTile = tile;
            tile.LiftTile(); // nhấc tile để báo hiệu đang chọn
            Debug.Log($"Tile đầu tiên: {tile.name}");
        }
        else
        {
            // Chọn tile thứ 2
            if (tile == firstTile)
            {
                // Bấm lại cùng tile → hủy chọn
                firstTile.ResetTilePosition();
                firstTile = null;
                Debug.Log("Hủy chọn tile đầu tiên.");
                return;
            }

            if (tile.HasAnimalInRegion())
            {
                // Debug.LogWarning($"Tile thứ 2 {tile.Coordinates} có động vật → không thể hoán đổi.");
                levelManager.DisplayLogWarning("Đã có động vật trong vùng này!\nKhông thể hoán đổi.");
                firstTile.ResetTilePosition();
                firstTile = null;
                return;
            }

            Debug.Log($"Tile thứ 2: {tile.name}");
            StartCoroutine(SwapTiles(firstTile, tile));

            firstTile = null;
        }
    }
    
    private IEnumerator SwapTiles(GridTileBase tileA, GridTileBase tileB)
    {
        canClick = false;

        tileA.ResetTilePosition();

        // Tween 
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.2f);
        seq.Join(tileA.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack));
        seq.Join(tileB.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack));
        seq.AppendInterval(0.2f);
        seq.AppendCallback(() =>
        {
            gridManager.audioManager.PlaySFXSwapTile();
            tileA.ReplaceTile(tileB.GetTileType(), gridManager.GetTilePrefab(tileB.GetTileType()), true, true);
            tileB.ReplaceTile(tileA.GetTileType(), gridManager.GetTilePrefab(tileA.GetTileType()), true, true);
        });

        yield return seq.WaitForCompletion();

        isSelecting = false;
        canClick = true;
        Consume();
    }
}
