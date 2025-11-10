using System.Collections.Generic;
using FancyScrollView.Example09;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMap : MonoBehaviour
{
    [SerializeField] private ScrollView scrollView = default;

    void Start()
    {
        int maxLevel = GameData.GetMaxLevel() + 1; // Cho phép mở khóa level tiếp theo

        maxLevel = HaveLevelData(maxLevel) ? maxLevel : maxLevel - 1; // Nếu level tiếp theo không có dữ liệu, giữ nguyên maxLevel hiện tại

        if (maxLevel < 1)
        {
            Debug.LogWarning("MaxLevel chưa được thiết lập đúng. Chuyển về Level 1.");
            PlayerPrefs.SetInt("CurrentLevel", 1);
            SceneManager.LoadScene("Game Scene");
        }
        else
        {
            var itemData = new List<ItemData>();

            for (int i = 1; i <= maxLevel; i++)
            {
                int stars = GameData.GetStars(i);

                itemData.Add(new ItemData(i, stars));
            }

            scrollView.UpdateData(itemData);
            scrollView.JumpTo(maxLevel);
        }
    }

    private bool HaveLevelData(int levelIndex)
    {
        string levelDataPath = $"Data/Level/Data Level {levelIndex}"; // đường dẫn trong Resources (không cần "Assets/")
        
        if (Resources.Load<LevelData>(levelDataPath) == null) return false;

        return true;
    }
}
