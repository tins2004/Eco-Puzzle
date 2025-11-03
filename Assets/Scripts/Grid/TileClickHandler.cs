// TileClickHandler: Xử lý thao tác của tile

using UnityEngine;

public class TileClickHandler : MonoBehaviour
{
    [HideInInspector] public GridTileBase tile;
    private LevelManager levelManager;

    [Header("Điều khiển - Nhấn giữ")]
    [SerializeField] private float holdTime = 0.3f; // Thời gian giữ (giây) để tính là "nhấn giữ"
    private float pressStartTime;
    private bool isPressing = false;

    private bool isTutorialActive = false;

    private void Start()
    {

        if (tile == null)
        {
            tile = GetComponent<GridTileBase>();
        }

        if (levelManager == null)
            levelManager = FindObjectOfType<LevelManager>();

        tile.originalPosition = transform.position; // Lưu vị trí ban đầu của tile
    }

    private void Update()
    {
        // Nếu đang ở trạng thái chọn động vật thì không thể thay đổi grid
        if (levelManager.animalManager.selectedAnimal != AnimalType.A00_Null) return;
        if (!levelManager.uiManager.canMove()) return;

        // --- Cảm ứng ---
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began && IsPointerOverTile(touch.position))
            {
                pressStartTime = Time.time;
                isPressing = true;
            }
            else if (touch.phase == TouchPhase.Stationary && isPressing)
            {
                if (Time.time - pressStartTime >= holdTime)
                {
                    tile.LiftTile();
                    if (isTutorialActive)
                        levelManager.uiManager.HideMaskTutorial();
                }
            }
            else if (touch.phase == TouchPhase.Ended && isPressing)
            {
                tile.ResetTilePosition();
                if (Time.time - pressStartTime >= holdTime)
                    HandleLongPress();
                else
                    OnClick();

                isPressing = false;
            }
        }
        // --- Chuột ---
        else
        {
            if (Input.GetMouseButtonDown(0) && IsPointerOverTile(Input.mousePosition))
            {
                pressStartTime = Time.time;
                isPressing = true;
            }
            else if (Input.GetMouseButton(0) && isPressing)
            {
                if (Time.time - pressStartTime >= holdTime)
                {
                    tile.LiftTile();
                    if (isTutorialActive)
                        levelManager.uiManager.HideMaskTutorial();

                }
            }
            else if (Input.GetMouseButtonUp(0) && isPressing)
            {
                tile.ResetTilePosition();
                if (Time.time - pressStartTime >= holdTime)
                    HandleLongPress();
                else
                    OnClick();

                isPressing = false;
            }
        }
    }

    private bool IsPointerOverTile(Vector2 screenPos)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        Collider2D hit = Physics2D.OverlapPoint(worldPos);

        if (levelManager.tutorialManager != null)
        {
            TutorialTargetType targetType = levelManager.tutorialManager.GetTargetType();

            if (targetType == TutorialTargetType.Text) return false;

            if (targetType == TutorialTargetType.Tile && targetType != TutorialTargetType.None)
            {
                Vector2Int tilePos = (Vector2Int)levelManager.tutorialManager.GetTargetAttributes();

                Vector3Int targetPos = new Vector3Int(tilePos.x, tilePos.y, 0);
                if (tile != null && tile.Coordinates != targetPos)
                {
                    return false; // Không phải tile mục tiêu
                }

                if (hit != null && hit.transform == transform)
                {
                    isTutorialActive = true;
                    return true; // Đúng tile mục tiêu
                }
            }
        } 

        return hit != null && hit.transform == transform;
    }

    private void OnClick()
    {
        if (levelManager.boosterManager.boosterSwapTile.HasBooster())
        {
            if (levelManager.boosterManager.boosterSwapTile.IsReady())
            {
                levelManager.boosterManager.boosterSwapTile.OnTileSelected(tile);
            }
        }

        if (levelManager.boosterManager.boosterUpgrade.HasBooster())
        {
            if (levelManager.boosterManager.boosterUpgrade.IsReady())
            {
                levelManager.boosterManager.boosterUpgrade.OnTileSelected(tile);
            }
        }
    }

    private void HandleLongPress()
    {
        if (levelManager.boosterManager.boosterSwapTile.IsReady() ||
        levelManager.boosterManager.boosterUpgrade.IsReady() ||
        levelManager.boosterManager.boosterSwapTile.IsReady())
            return;

        if (tile != null)
        {
            if (isTutorialActive)
            {
                levelManager.tutorialManager.NextTutorialStep();
                isTutorialActive = false;
            }
            // Debug.Log($"Nhấn giữu tại {tile.Coordinates}: {tile.GetTileType()}");
            tile.HandleLongPress();
            levelManager.audioManager.VibratePop();
        }
    }
}
