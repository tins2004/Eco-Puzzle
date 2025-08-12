// TileClickHandler: Xử lý thao tác của tile

using UnityEngine;

public class TileClickHandler : MonoBehaviour
{
    [HideInInspector] public GridTileBase tile;

    [Header("Điều khiển - Nhấn giữ")]
    [SerializeField] private float holdTime = 0.3f; // Thời gian giữ (giây) để tính là "nhấn giữ"
    private float pressStartTime;
    private bool isPressing = false;


    private void Start()
    {

        if (tile == null)
        {
            tile = GetComponent<GridTileBase>();
        }

        tile.originalPosition = transform.position; // Lưu vị trí ban đầu của tile
    }

    private void Update()
    {
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
        return hit != null && hit.transform == transform;
    }

    private void OnClick()
    {
        if (tile != null)
        {
            Debug.Log($"Nhấn thả tại {tile.Coordinates}: {tile.GetTileType()}");
        }
    }

    private void HandleLongPress()
    {
        if (tile != null)
        {
            // Debug.Log($"Nhấn giữu tại {tile.Coordinates}: {tile.GetTileType()}");
            tile.HandleLongPress();
        }
    }
}
