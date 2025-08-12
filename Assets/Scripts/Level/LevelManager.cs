using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private int currentLevelIndex = 1; // bắt đầu từ Level 1
    private LevelData levelData;

    [Header("Manager Link")]
    [SerializeField] private GridMapManager gridMapManager;
    [SerializeField] private MissionManager missionManager;
    [SerializeField] private UIManager uiManager;

    private void Awake()
    {
        currentLevelIndex = PlayerPrefs.GetInt("CurrentLevel", 1);
        LoadLevelData(currentLevelIndex);
        InitManagers();
    }

    private void LoadLevelData(int levelIndex)
    {
        string path = $"Data/Level/Data Level {levelIndex}"; // đường dẫn trong Resources (không cần "Assets/")
        levelData = Resources.Load<LevelData>(path);

        if (levelData == null)
        {
            Debug.LogError($"Không tìm thấy LevelData ở path: {path}");
            
            Debug.LogWarning("Sử dụng dữ liệu mặc định cho Level 1.");
            PlayerPrefs.SetInt("CurrentLevel", 1);
            path = $"Data/Level/Data Level 1";
            levelData = Resources.Load<LevelData>(path);
        }
    }

    private void InitManagers()
    {
        if (levelData == null) return;

        // UI Manager
        if (uiManager != null)
        {
            uiManager.SetScoreData(levelData);
        }

        // Grid Map Manager
        if (gridMapManager != null)
        {
            gridMapManager.SetLevelData(levelData);
            gridMapManager.StartGrid();
        }

        // Mission Manager
        if (missionManager != null)
        {
            missionManager.SetLevelData(levelData, uiManager);
            missionManager.StartMission();
        }
    }

    public void NextLevel()
    {
        currentLevelIndex++;
        PlayerPrefs.SetInt("CurrentLevel", currentLevelIndex); // lưu level
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // restart scene
    }
    
    public void RestartLevelTest()
    {
        PlayerPrefs.SetInt("CurrentLevel", 1); // lưu level
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // restart scene
    }
}
