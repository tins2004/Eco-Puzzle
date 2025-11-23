using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class VerticalProgressBarUI : MonoBehaviour
{
    [Header("References")]
    public RewardProgressData rewardData;    // Data tiers
    public RewardProgression progression;    // Current Gem

    [Header("UI")]
    public RectTransform barContainer;       // Container thanh dọc
    public Image fillImage;                  // Phần fill màu xanh
    public GameObject markerPrefab;          // Marker hiển thị từng Tier

    private List<Image> markers = new List<Image>();
    private int maxGem = 0;

    private void Start()
    {
        if (rewardData == null || barContainer == null || fillImage == null || markerPrefab == null)
            return;

        // Tính maxGem
        foreach (var tier in rewardData.tiers)
        {
            maxGem = Mathf.Max(maxGem, tier.requiredGem);
        }

        GenerateMarkers();
        RefreshUI();
    }

    private void GenerateMarkers()
    {
        foreach (Transform child in barContainer)
            Destroy(child.gameObject);

        markers.Clear();

        int tierCount = rewardData.tiers.Count;
        for (int i = 0; i < tierCount; i++)
        {
            RewardTier tier = rewardData.tiers[i];

            GameObject markerObj = Instantiate(markerPrefab, barContainer);
            Image markerImage = markerObj.GetComponent<Image>();
            markers.Add(markerImage);

            // Tính vị trí marker dọc (anchored y)
            float percent = (float)tier.requiredGem / maxGem;
            markerObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, percent * barContainer.rect.height);
        }
    }

    public void RefreshUI()
    {
        if (progression == null) return;

        // Lấy Gem hiện tại
        float currentGem = progression.currentGem;

        // Tính fill percent dựa trên maxGem
        float fillPercent = currentGem / maxGem;

        fillPercent = Mathf.Clamp01(fillPercent);

        // Áp dụng vào fill Image
        fillImage.fillAmount = fillPercent;

        // Update màu marker
        for (int i = 0; i < markers.Count; i++)
        {
            if (currentGem >= rewardData.tiers[i].requiredGem)
                markers[i].color = Color.green; // Tier đã đạt
            else
                markers[i].color = Color.yellow; // Tier chưa đạt
        }
    }

}
