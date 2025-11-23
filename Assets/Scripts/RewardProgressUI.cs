using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RewardProgressUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RewardProgression progression;
    [SerializeField] private RewardProgressData rewardData;

    [SerializeField] private Transform rewardParent;        // Free container
    [SerializeField] private Transform rewardParentVIP;     // VIP container

    private List<TierUIItem> tierUIList = new List<TierUIItem>();

    private void Start()
    {
        GenerateUI();
        RefreshUI();
    }

    // ==========================================================
    //  TẠO CÁC TIER UI TỪ DATA
    // ==========================================================
    public void GenerateUI()
    {
        // Xóa UI cũ
        foreach (Transform child in rewardParent)
            Destroy(child.gameObject);

        foreach (Transform child in rewardParentVIP)
            Destroy(child.gameObject);

        tierUIList.Clear();

        for (int i = 0; i < rewardData.tiers.Count; i++)
        {
            RewardTier data = rewardData.tiers[i];

            // ==========================
            //  FREE UI
            // ==========================
            GameObject freeUIObj = Instantiate(data.freeRewardPrefab, rewardParent);
            TierUIItem freeUI = freeUIObj.GetComponent<TierUIItem>();
            freeUI.Setup(i, data.requiredGem);

            // ==========================
            //  VIP UI
            // ==========================
            GameObject vipUIObj = Instantiate(data.vipRewardPrefab, rewardParentVIP);
            TierUIItem vipUI = vipUIObj.GetComponent<TierUIItem>();
            vipUI.Setup(i, data.requiredGem);

            int tierIndex = i;

            // Free button event
            freeUI.freeButton.onClick.AddListener(() =>
            {
                progression.ClaimFree(tierIndex);
                RefreshUI();
            });

            // VIP button event
            vipUI.vipButton.onClick.AddListener(() =>
            {
                progression.ClaimVIP(tierIndex);
                RefreshUI();
            });

            tierUIList.Add(freeUI);
            tierUIList.Add(vipUI);
        }
    }

    // ==========================================================
    //  CẬP NHẬT UI
    // ==========================================================
    public void RefreshUI()
    {
        int tierCount = rewardData.tiers.Count;

        for (int i = 0; i < tierCount; i++)
        {
            TierUIItem freeUI = tierUIList[i * 2];
            TierUIItem vipUI = tierUIList[i * 2 + 1];

            bool canClaimFree = progression.CanClaimFree(i);
            bool canClaimVip = progression.CanClaimVip(i);

            // Free
            freeUI.freeButton.interactable = canClaimFree;

            // VIP (luôn hiện nhưng có thể bị khóa)
            vipUI.vipButton.interactable = canClaimVip;

            // Màu nền
            if (i <= progression.LastClaimedTier)
            {
                freeUI.SetStatusClaimed();
                vipUI.SetStatusClaimed();
            }
            else if (!canClaimFree)
            {
                freeUI.SetStatusLocked();
                vipUI.SetStatusLocked();
            }
            else
            {
                freeUI.SetStatusReady();
                vipUI.SetStatusReady();
            }
        }
    }

    public void AddGem(int amount)
    {
        progression.AddGem(amount);
        RefreshUI();
    }
}
