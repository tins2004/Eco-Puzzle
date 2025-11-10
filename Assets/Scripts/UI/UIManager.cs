using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    // private LevelData levelData;
    [SerializeField] public Canvas mainCanvas;
    [HideInInspector] public AudioManager audioManager;

    [Header("Data Sprite")]
    [SerializeField] public List<TileDataSprite> tiles;
    [SerializeField] public List<AnimalData> animals;


    [Header("UI Object Mission, Limit, Score")]
    [SerializeField] private GameObject missionBox;
    [SerializeField] private GameObject missionItemPrefab;
    [SerializeField] private TMP_Text limitText;
    [SerializeField] private TMP_Text gemText;
    [SerializeField] private Slider scoreSlider;
    [SerializeField] private Image[] stars; 
    [SerializeField] private Sprite starEmpty;
    [SerializeField] private Sprite starFilled;

    // ----- Score and Star -----
    private int currentScore = 0;
    private int maxScore = 0;
    private int currentStart = 0;
    private int[] starRating = new int[3];

    // ----- Limit -----
    private int limitValue = 0;

    [Header("UI Tutorial")]
    [SerializeField] private TMP_Text tutorialText;
    [SerializeField] private GameObject tutorialTextBox;
    [SerializeField] private GameObject tutorialTextPanel;
    [SerializeField] private GameObject tutorialMask;

    // ----- Mission -----
    private Dictionary<TileType, MissionCache> tileMissionCache = new Dictionary<TileType, MissionCache>();
    private Dictionary<AnimalType, MissionCache> animalMissionCache = new Dictionary<AnimalType, MissionCache>();
    [SerializeField] private ObjectPool missionEffectPool;


    // ----- Tutorial -----
    private TutorialManager tutorialManager;
    private float sizeMask;
    private float bigSizeMask = 50f;
    private float timeZoomMask = 0.3f;
    private bool canHideMask = false;

    // ----- Box UI -----
    [Header("Box UI")]
    [SerializeField] private BoxUI pauseBox;
    [SerializeField] private BoxUI finishBox;
    [SerializeField] private BoxUI loseBox;
    [SerializeField] private Button pauseButton;

    public bool isReady = false;
    private bool isFinished = false;
    
    public void SetTutorialManager(TutorialManager manager)
    {
        tutorialManager = manager;
    }

    public void SetScoreData(LevelData levelData)
    {
        audioManager = AudioManager.Instance;

        // this.levelData = levelData;
        maxScore = levelData.maxScore;
        starRating[0] = (int)(maxScore * 0.25); // 1 sao = 25% điểm tối đa
        starRating[1] = (int)(maxScore * 0.5);  // 2 sao = 50% điểm tối đa
        starRating[2] = (int)(maxScore * 0.8);  // 3 sao = 80% điểm tối đa

        scoreSlider.value = 0;

        // Cập nhật sao
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].sprite = starEmpty;
        }

        pauseButton.onClick.AddListener(() =>
        {
            gameObject.GetComponent<CanvasGroup>().DOFade(0, 0.3f);
            pauseBox.ShowBox();
        });
    }

    public bool canMove()
    {
        if (!isReady) return false;
        if (isFinished) return false;
        if (pauseBox.IsShowing() || finishBox.IsShowing() || loseBox.IsShowing()) return false;
        if (limitValue <= 0) return false;

        return true;
    }

    public void UpdateUI(Dictionary<TileType, int> tileMissionCounts, Dictionary<AnimalType, int> animalMissionCounts, int limitValue)
    {
        // if (levelData == null)
        // {
        //     Debug.LogError("Chưa có dữ liệu của level hiện tại!");
        //     return;
        // }

        UpdateMissionUI(tileMissionCounts, animalMissionCounts);
        UpdateLimitUI(limitValue);
        UpdateGemUI();
        UpdateScoreUI();

        if (tutorialManager != null)
        {
            ShowCurrentTutorial();
        }
        else
        {
            // HideMaskTutorial();
            // HideTextTutorial();
            tutorialMask.SetActive(false);
            tutorialTextBox.SetActive(false);
        }
    }

    public void UpdateMissionUI(Dictionary<TileType, int> tileMissionCounts, Dictionary<AnimalType, int> animalMissionCounts)
    {
        if (missionItemPrefab == null || missionBox == null)
        {
            Debug.LogError("Mission chưa được gán!");
            return;
        }

        // Tạo item nhiệm vụ nếu chưa có
        GenerateMissionItem(tileMissionCounts, animalMissionCounts);


        // Cập nhật số lượng
        foreach (var kvp in tileMissionCounts)
        {
            if (tileMissionCache.TryGetValue(kvp.Key, out MissionCache cache))
            {
                cache.text.text = Mathf.Max(0, kvp.Value).ToString();
            }
        }

        foreach (var kvp in animalMissionCounts)
        {
            if (animalMissionCache.TryGetValue(kvp.Key, out MissionCache cache))
            {
                cache.text.text = Mathf.Max(0, kvp.Value).ToString();
            }
        }

        // Kiểm tra nếu hoàn thành hết 
        if (tileMissionCounts.Values.All(v => v <= 0) && animalMissionCounts.Values.All(v => v <= 0))
        {
            // GameObject doneItem = Instantiate(missionItemPrefab, missionBox.transform);
            // Text doneText = doneItem.transform.Find("Text").GetComponent<Text>();
            // doneText.text = "Tất cả nhiệm vụ đã hoàn thành!";
            FinishAllMissions();
        }
    }

    private void GenerateMissionItem(Dictionary<TileType, int> tileMissionCounts, Dictionary<AnimalType, int> animalMissionCounts)
    {
        // Kiểm tra số lượng nhiệm vụ
        if (tileMissionCounts.Count + animalMissionCounts.Count == 0)
        {
            Debug.LogError("Chưa có nhiệm vụ nào được gán!");
            return;
        }
        else if (tileMissionCounts.Count + animalMissionCounts.Count > 3)
        {
            Debug.LogError("Số lượng nhiệm vụ vượt quá 3!");
            return;
        }

        // Tile missions
        foreach (var kvp in tileMissionCounts)
        {
            if (!tileMissionCache.ContainsKey(kvp.Key))
            {
                GameObject item = Instantiate(missionItemPrefab, missionBox.transform);
                Image icon = item.transform.Find("Icon").GetComponent<Image>();
                TMP_Text text = item.transform.Find("Text").GetComponent<TMP_Text>();

                item.name = $"Mission_Tile_{(TileType)kvp.Key}";

                icon.sprite = tiles.Find(t => t.type == kvp.Key)?.icon;
                icon.transform.localScale = Vector3.one * 1.5f;
                
                // Lưu vào cache
                tileMissionCache[kvp.Key] = new MissionCache
                {
                    item = item,
                    text = text,
                    icon = icon
                };
            }
        }

        // Animal missions
        foreach (var kvp in animalMissionCounts)
        {
            if (!animalMissionCache.ContainsKey(kvp.Key))
            {
                GameObject item = Instantiate(missionItemPrefab, missionBox.transform);
                Image icon = item.transform.Find("Icon").GetComponent<Image>();
                TMP_Text text = item.transform.Find("Text").GetComponent<TMP_Text>();

                item.name = $"Mission_Animal_{(AnimalType)kvp.Key}";
                icon.sprite = animals.Find(a => a.type == kvp.Key)?.icon;
                icon.transform.localScale = Vector3.one * 1.2f;

                animalMissionCache[kvp.Key] = new MissionCache
                {
                    item = item,
                    text = text,
                    icon = icon
                };
            }
        }
    }
    public void ResetMissions()
    {
        foreach (var cache in tileMissionCache.Values)
        {
            Destroy(cache.item);
        }
        foreach (var cache in animalMissionCache.Values)
        {
            Destroy(cache.item);
        }
        tileMissionCache.Clear();
        animalMissionCache.Clear();
    }

    public void SpawnMissionTileEffect(TileType tileType, Vector3 position)
    {
        if (!tileMissionCache.TryGetValue(tileType, out MissionCache cache))
            return;
        
        if (int.TryParse(cache.text.text, out int value))
        {
            if (value <= 0)
                return;
        }

        // Tạo effect ở canvas
        GameObject effect = missionEffectPool.GetObject();
        effect.transform.position = position;
        effect.transform.localScale = Vector3.zero; // ban đầu = 0 để làm hiệu ứng mọc

        effect.GetComponent<Image>().sprite = tiles.Find(t => t.type == tileType)?.icon;

        Vector3 targetPos = cache.item.transform.position;

        // Sequence hiệu ứng
        StartMissionEffect(effect, targetPos, cache.item.transform);
    }

    public void SpawnMissionAnimalEffect(AnimalType animalType, Vector3 position)
    {
        if (!animalMissionCache.TryGetValue(animalType, out MissionCache cache))
            return;

        // Tạo effect ở canvas
        GameObject effect = missionEffectPool.GetObject();
        effect.transform.position = position;
        effect.transform.localScale = Vector3.zero; // ban đầu = 0 để làm hiệu ứng mọc

        effect.GetComponent<Image>().sprite = animals.Find(a => a.type == animalType)?.icon;

        Vector3 targetPos = cache.item.transform.position;

        // Sequence hiệu ứng
        StartMissionEffect(effect, targetPos, cache.item.transform);
    }

    private void StartMissionEffect(GameObject effect, Vector3 targetPos, Transform transformTaget)
    {
        effect.GetComponent<CanvasGroup>().alpha = 1f;
        Sequence seq = DOTween.Sequence();

        seq.Append(effect.transform.DOScale(3f, 0.2f).SetEase(Ease.OutBack)); // phóng to như nấm mọc
        seq.JoinCallback(() => audioManager.PlaySFXPop()); // phóng to như nấm mọc
        seq.Append(effect.transform.DOMove(targetPos, 0.4f).SetEase(Ease.InQuad)); // bay tới đích
        seq.Join(effect.transform.DOScale(0.7f, 0.4f)); // trong lúc bay thì hơi thu nhỏ lại
        seq.AppendCallback(() =>
        {
            // Hiệu ứng nổ tung/biến mất
            audioManager.PlaySFXCheckMission();
            effect.transform.DOScale(1.5f, 0.2f).SetEase(Ease.OutQuad);
            effect.GetComponent<CanvasGroup>()?.DOFade(0f, 0.2f);

            // Item mission rung nhẹ khi nhận
            transformTaget.DOPunchScale(Vector3.one * 0.2f, 0.3f, 5, 0.5f);
        });
        seq.AppendInterval(0.2f);
        seq.OnComplete(() =>
        {
            // Trả object về pool
            missionEffectPool.ReturnObject(effect);
        });
    }

    public void UpdateLimitUI(int value)
    {
        if (limitText == null)
        {
            Debug.LogError("Limit Text chưa được gán!");
            return;
        }
        limitValue = value;

        if (limitValue > 0)
            limitText.text = limitValue.ToString();
        else
        {
            limitValue = 0;
            limitText.text = "00";

            // Hết lượt -> thua
            if (!isFinished)
            {
                StartCoroutine(RecheckIsLose());
            }
        }
    }

    private IEnumerator RecheckIsLose()
    {
        yield return new WaitForSeconds(0.7f);

        if (limitValue <= 0 && !isFinished)
        {
            gameObject.GetComponent<CanvasGroup>().DOFade(0, 0.3f);
            loseBox.ShowBox();
        }
    }

    public void ContinueLevelWithPoint(int limitValue)
    {
        // Hiện lại UI
        gameObject.GetComponent<CanvasGroup>().DOFade(1, 0.3f);

        // Ẩn các box UI
        // pauseBox.HideBox();
        // finishBox.HideBox();
        loseBox.HideBox();

        // Reset trạng thái
        isFinished = false;

        // UpdateLimitUI(limitValue);
        FindObjectOfType<MissionManager>().SetLimitValue(limitValue);
    }

    public void AddScore(int score)
    {
        currentScore += score;

        if (currentScore > maxScore)
        {
            currentScore = maxScore;
        }

        UpdateScoreUI();
    }

    private void UpdateScoreUI(float targetProgress = 0)
    {
        if (scoreSlider == null)
        {
            Debug.LogError("Score Slider chưa được gán!");
            return;
        }

        if (targetProgress > 0)
            currentScore = (int)targetProgress;

        float progress = (float)currentScore / maxScore;

        DOTween.To(() => scoreSlider.value, x => scoreSlider.value = x, progress, 0.5f).SetEase(Ease.OutCubic);

        // Cập nhật sao
        for (int i = 0; i < stars.Length; i++)
        {
            if (currentStart > i) continue;

            if (currentScore >= starRating[i])
            {
                stars[i].sprite = starFilled;

                currentStart = i + 1;
                
                int index = i;

                // stars[index].transform.DOScale(2.5f, 0.15f)
                //     .SetEase(Ease.OutBack)
                //     .OnComplete(() =>
                //     {
                //         stars[index].transform.DOScale(1f, 0.15f).SetEase(Ease.InBack);
                //     });
                stars[index].transform.DOPunchScale(Vector3.one * 0.5f, 0.3f, 8, 1); // hiệu ứng nhún nhẹ
                audioManager.PlaySFXAddStar();
            }
            else
                stars[i].sprite = starEmpty;
        }
    }
    
    private void UpdateGemUI()
    {
        if (gemText == null)
        {
            Debug.LogError("Gem Text chưa được gán!");
            return;
        }

        gemText.text = GameData.GetCurrentGem().ToString();
    }

    private void FinishAllMissions()
    {
        isFinished = true;

        int endScore = limitValue > 0 ? currentScore + limitValue * 15 : currentScore;
        int endStars = endScore >= starRating[2] ? 3 : endScore >= starRating[1] ? 2 : endScore >= starRating[0] ? 1 : 0;

        // Debug.Log($"Max{GameData.GetMaxLevel()}, curent {GameData.GetCurrentLevel()}, {(GameData.GetMaxLevel() < GameData.GetCurrentLevel())}");
        FireBaseAnalytics.Instance.LogLevelComplete(GameData.GetCurrentLevel(), endStars);

        // Lưu điểm và sao
        GameData.SetMaxLevel(Mathf.Max(GameData.GetMaxLevel(), GameData.GetCurrentLevel()));
        GameData.SetStars(GameData.GetCurrentLevel(), endStars);


        if (limitValue <= 0)
        {
            // FindObjectOfType<LevelManager>().NextLevel();
            gameObject.GetComponent<CanvasGroup>().DOFade(0, 0.3f);
            finishBox.ShowBox();
            return;
        }

        int startValue = limitValue;

        // Hiệu ứng đếm nhanh về 0
        DOTween.To(() => startValue, x =>
        {
            startValue = x;
            limitText.text = startValue.ToString();
        }, 0, 0.8f)
        .SetEase(Ease.InQuad)
        .OnComplete(() =>
        {
            limitText.text = "0";
            limitValue = 0;

            // --- Tạo một Sequence để sắp xếp hiệu ứng ---
            Sequence seq = DOTween.Sequence();

            // Sinh 4 tile bay cách nhau 0.2s
            for (int i = 0; i < Random.Range(3, 6); i++)
            {
                seq.AppendCallback(() =>
                {
                    GameObject effect = missionEffectPool.GetObject();
                    effect.transform.position = limitText.transform.position + Random.insideUnitSphere * 0.2f;
                    effect.transform.localScale = Vector3.zero;
                    // effect.GetComponent<Image>().sprite = tiles.Find(t => t.type == TileType.T02_Water_Lake)?.icon;
                    effect.GetComponent<Image>().sprite = tiles[Random.Range(1, tiles.Count)].icon;

                    StartMissionEffect(effect, scoreSlider.transform.position, scoreSlider.transform);
                });

                // Cách nhau 0.2 giây giữa các hiệu ứng
                seq.AppendInterval(0.2f);
            }

            //Cho thêm bộ tăng điểm
            seq.AppendCallback(() =>
            {
                UpdateScoreUI(endScore);
            });

            // Sau khi tạo toàn bộ effect xong -> chờ -> NextLevel
            seq.AppendInterval(0.5f);
            seq.AppendCallback(() =>
            {
                gameObject.GetComponent<CanvasGroup>().DOFade(0, 0.3f);
                finishBox.ShowBox();
                // FindObjectOfType<LevelManager>().NextLevel();
            });

            seq.Play();
        });
    }

    // ----- UI Tutorial -----
    public void ShowCurrentTutorial()
    {
        if (tutorialMask == null || tutorialText == null || tutorialTextBox == null)
        {
            Debug.LogError("Chưa gán UI Tutorial!");
            return;
        }


        ShowMaskTutorial();
        ShowTextTutorial();
    }

    private void ShowMaskTutorial()
    {
        if (tutorialMask == null) return;

        var targetType = tutorialManager.GetTargetType();

        switch (targetType)
        {
            case TutorialTargetType.Tile:
                sizeMask = 3.7f;
                tutorialMask.transform.position = CellToWorldHex((Vector2Int)tutorialManager.GetTargetAttributes());
                // + new Vector3(0f, 0.05f, 0f);
                break;
            case TutorialTargetType.Animal:
                break;
            case TutorialTargetType.UIButton:
                sizeMask = 3.7f;
                tutorialMask.transform.position = GetVector3FromUIButton((string)tutorialManager.GetTargetAttributes());
                tutorialManager.nextTextButton.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
                break;
            case TutorialTargetType.Text:
            case TutorialTargetType.None:
                tutorialMask.SetActive(false);
                tutorialManager.nextTextButton.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
                return;
        }


        // bắt đầu từ lớn
        tutorialManager.nextTextButton.transform.localScale = Vector3.zero;

        tutorialMask.transform.localScale = Vector3.one * bigSizeMask;
        tutorialMask.SetActive(true);

        // Thu nhỏ lại tutorial
        tutorialMask.transform.DOScale(Vector3.one * sizeMask, timeZoomMask)
                                .SetEase(Ease.InBack)
                                .OnComplete(() =>
                                {
                                    tutorialMask.SetActive(true);
                                    // delay 0.3s trước khi cho phép hide
                                    DOVirtual.DelayedCall(0.3f, () =>
                                    {
                                        canHideMask = true;
                                    });
                                });
    }

    public void HideMaskTutorial()
    {
        if (!canHideMask) return;
        if (tutorialMask == null || !tutorialMask.activeSelf) return;

        canHideMask = false;
        
        tutorialMask.transform.DOScale(Vector3.one * bigSizeMask, timeZoomMask)
                    .SetEase(Ease.OutBack)
                    .OnComplete(() => tutorialMask.SetActive(false));
    }

    /// <summary>
    /// Chuyển từ tọa độ ô lục giác sang tọa độ thế giới
    /// </summary>
    /// <param name="cell">Vector3Int(x, y, 0) theo cellPos bạn đang dùng</param>
    /// <param name="cellSizeX">Kích thước ô theo chiều rộng (mặc định 1)</param>
    /// <param name="cellSizeY">Kích thước ô theo chiều cao (mặc định 1)</param>
    /// <returns></returns>
    private static Vector3 CellToWorldHex(Vector2Int cell, float cellSizeX = 1f, float cellSizeY = 1f)
    {
        int x = cell.x;
        int y = cell.y;

        // isOdd cho cả y âm: dùng Mathf.Abs
        bool isOdd = (Mathf.Abs(y) % 2) == 1;

        float xOffset = isOdd ? 0.5f : 0f;
        float wx = (x + xOffset) * cellSizeX;
        float wy = y * (cellSizeY * 0.75f);

        return new Vector3(wx, wy, 0f);
    }

    private Vector3 GetVector3FromUIButton(string buttonName)
    {
        Image[] uis = FindObjectsOfType<Image>();
        foreach (var ui in uis)
        {
            if (ui.name == buttonName)
            {
                return ui.transform.position;
            }
        }

        Debug.LogError($"Không tìm thấy button với tên {buttonName}");
        return Vector3.zero;
    }


    private void ShowTextTutorial()
    {
        tutorialText.transform.localScale = Vector3.zero;   
        tutorialTextBox.SetActive(true);
        tutorialTextPanel.SetActive(tutorialManager.GetTargetType() == TutorialTargetType.Text);

        tutorialManager.SetTutorialText(tutorialText);

        tutorialText.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
    }

    public void HideTextTutorial()
    {
        if (tutorialTextBox == null) return;
        tutorialText.transform.localScale = Vector3.one;   
        tutorialText.text = "";
        tutorialText.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack)
                                                        .OnComplete(() =>
                                                        {
                                                            tutorialTextBox.SetActive(false);
                                                            tutorialTextPanel.SetActive(false);
                                                        });
        
    }
}
