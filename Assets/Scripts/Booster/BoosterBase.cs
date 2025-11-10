using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class BoosterBase : MonoBehaviour
{
    [SerializeField] private TMP_Text numberOfBooster;
    private Button button;

    protected bool canClick = true;
    protected string boosterNumberKey;
    protected bool isSelecting = false;

    protected GridMapManager gridManager;
    protected LevelManager levelManager;

    protected virtual void Start()
    {
        canClick = true;

        SetBoosterNumberKey();

        levelManager = FindObjectOfType<LevelManager>();
        gridManager = levelManager.gridMapManager;

        button = GetComponent<Button>();
        button.onClick.AddListener(() => {
            if (!canClick) return;

            if (IsReady())
            {
                gridManager.audioManager.PlaySFXButton();
                CancelActivate();
                return;
            }

            if (HasBooster())
            {
                gridManager.audioManager.PlaySFXButton();
                Activate();
            }
            else
            {
                gridManager.audioManager.PlaySFXFailClick();
                gridManager.audioManager.VibrateClassic();
                Debug.LogWarning("Hết bổ trợ rồi!");
            }
        });
        UpdateUI();
        // numberOfBooster.text = boosterNumberKey + PlayerPrefs.GetInt(boosterNumberKey, 0).ToString();
    }

    /// <summary>
    /// Kích hoạt booster — được override ở các lớp con.
    /// </summary>
    public abstract void Activate();

    public abstract void CancelActivate();

    /// <summary>
    /// Đặt key PlayerPrefs
    /// </summary>
    protected abstract void SetBoosterNumberKey();

    /// <summary>
    /// Trừ 1 booster khi sử dụng.
    /// </summary>
    protected void Consume()
    {
        int boosterCount = PlayerPrefs.GetInt(boosterNumberKey, 0);
        if (boosterCount <= 0)
        {
            Debug.LogWarning($"Booster {boosterNumberKey} đã hết!");
            // return false;
            return;
        }

        boosterCount--;
        PlayerPrefs.SetInt(boosterNumberKey, boosterCount);
        UpdateUI();
        // return true;
    }

    /// <summary>
    /// Kiểm tra còn booster không.
    /// </summary>
    public bool HasBooster()
    {
        return PlayerPrefs.GetInt(boosterNumberKey, 0) > 0;
    }

    /// <summary>
    /// Sẵn sàng hoạt động chưa.
    /// </summary>
    public bool IsReady()
    {
        return isSelecting;
    }

    private void UpdateUI()
    {
        int number = PlayerPrefs.GetInt(boosterNumberKey, 0);

        gameObject.GetComponent<Image>().color = number <= 0 ? new Color(0.3f, 0.3f, 0.3f) : Color.white;

        numberOfBooster.text = number <= 0 ? "+" : number.ToString();
        numberOfBooster.color = number <= 0 ? Color.gray : Color.black;
    }
}
