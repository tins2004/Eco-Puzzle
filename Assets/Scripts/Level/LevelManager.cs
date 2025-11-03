using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [HideInInspector] public AudioManager audioManager;

    [Header("Level Settings")]
    [SerializeField] private int currentLevelIndex = 1; // bắt đầu từ Level 1
    [SerializeField] private TMP_Text logWarning;
    private LevelData levelData;
    private TutorialData tutorialData;


    [Header("Manager Link")]
    [SerializeField] public GridMapManager gridMapManager;
    [SerializeField] public MissionManager missionManager;
    [SerializeField] public UIManager uiManager;
    [SerializeField] public TutorialManager tutorialManager;
    [SerializeField] public AnimalManager animalManager;
    [SerializeField] public BoosterManager boosterManager;

    [Header("Texture")]
    [SerializeField] public CanvasGroup UICanvas;
    [SerializeField] public SceneTransition sceneTransition;

    private void Awake()
    {
        currentLevelIndex = PlayerPrefs.GetInt("CurrentLevel", 1);
        LoadLevelData(currentLevelIndex);
    }

    private void Start()
    {
        audioManager = AudioManager.Instance;
        audioManager.PlayMusicBackground();

        FireBaseAnalytics.Instance.LogLevelStart(currentLevelIndex);

        sceneTransition.CloseEffect();
        logWarning.gameObject.SetActive(false);

        InitManagers();
    }

    private void LoadLevelData(int levelIndex)
    {
        string levelDataPath = $"Data/Level/Data Level {levelIndex}"; // đường dẫn trong Resources (không cần "Assets/")
        string tutorialPath = $"Data/Tutorial/Data Tutorial {levelIndex}";
        levelData = Resources.Load<LevelData>(levelDataPath);
        tutorialData = Resources.Load<TutorialData>(tutorialPath);

        if (levelData == null)
        {
            Debug.LogError($"Không tìm thấy LevelData ở path: {levelDataPath}");

            Debug.LogWarning("Sử dụng dữ liệu mặc định cho Level 1.");
            PlayerPrefs.SetInt("CurrentLevel", 1);
            levelDataPath = $"Data/Level/Data Level 1";
            levelData = Resources.Load<LevelData>(levelDataPath);
        }
    }

    private void InitManagers()
    {
        if (levelData == null) return;
        UICanvas.alpha = 0; // ẩn UI lúc đầu

        if (tutorialManager == null || tutorialData == null)
        {
            tutorialManager = null;
        }
        else
        {
            tutorialManager.SetTutorialData(tutorialData);
        }

        // booster Manager
        if (boosterManager != null)
        {
            uiManager.SetScoreData(levelData);
            if (tutorialManager != null) uiManager.SetTutorialManager(tutorialManager);
        }
        

        // Mission Manager
        if (missionManager != null)
        {
            missionManager.SetLevelData(levelData, uiManager);
            missionManager.StartMission();
        }

        // Grid Map Manager
        if (gridMapManager != null)
        {
            gridMapManager.SetLevelData(levelData, this);
            gridMapManager.StartGrid();
        }

        // Animal Manager
        if (animalManager != null)
        {
            animalManager.SetLevelData(levelData, this);
            animalManager.StartAnimal();
        }
    }

    public void NextLevel(bool isRetry = false)
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.5f);
        seq.Append(gridMapManager.transform.DOScale(0, 0.2f).SetEase(Ease.InBack));
        seq.Append(uiManager.GetComponent<CanvasGroup>().DOFade(0, 0.3f));
        seq.AppendCallback(() =>
        {
            if (isRetry) currentLevelIndex++;
            PlayerPrefs.SetInt("CurrentLevel", currentLevelIndex); // lưu level
            // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // restart scene
            sceneTransition.OpenEffect(SceneManager.GetActiveScene().name);
        });
    }

    public void RestartLevelTest()
    {
        PlayerPrefs.SetInt("CurrentLevel", 1); // lưu level
        sceneTransition.OpenEffect(SceneManager.GetActiveScene().name);
    }

    public void ChangeToHomeScene()
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.5f);
        seq.Append(gridMapManager.transform.DOScale(0, 0.2f)
                                .SetEase(Ease.InBack)
                                .OnComplete(() => sceneTransition.OpenEffect("Home Scene")));
    }

    public void ContinueLevelWithPoint()
    {
        uiManager.ContinueLevelWithPoint(10);
    }

    public void DisplayLogWarning(string mess)
    {
        logWarning.transform.localScale = Vector3.zero;
        logWarning.gameObject.SetActive(true);
        logWarning.text = mess;

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.2f);
        seq.Append(logWarning.transform.DOScale(1, 0.2f).SetEase(Ease.OutBack));
        seq.AppendInterval(1f);
        seq.Append(logWarning.transform.DOScale(0, 0.2f)
                                .SetEase(Ease.InBack)
                                .OnComplete(() => logWarning.gameObject.SetActive(false)));
    }
}
