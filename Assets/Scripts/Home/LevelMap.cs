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
}
