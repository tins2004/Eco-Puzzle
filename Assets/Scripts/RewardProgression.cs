using UnityEngine;

public class RewardProgression : MonoBehaviour
{
    [SerializeField] private RewardProgressData rewardData;
    [SerializeField] private Transform rewardParent;
    public int LastClaimedTier => lastClaimedTier;

    public int currentGem = 30;
    private int lastClaimedTier = -1;

    // ---------------------------
    // DEBUG HELPER
    // ---------------------------
    private void DebugRewardState(string header)
    {
        Debug.Log("<color=yellow>==== DEBUG: " + header + " ====</color>");

        Debug.Log("rewardData = " + (rewardData == null ? "NULL" : "OK"));
        Debug.Log("rewardParent = " + (rewardParent == null ? "NULL" : rewardParent.name));
        Debug.Log("currentGem = " + currentGem);
        Debug.Log("lastClaimedTier = " + lastClaimedTier);

        if (rewardData != null)
        {
            Debug.Log("tiers count = " + rewardData.tiers.Count);
        }

        Debug.Log("<color=yellow>===========================</color>");
    }

    public void AddGem(int amount)
    {
        currentGem += amount;
        Debug.Log($"Add Gem: {amount}. Total = {currentGem}");
    }

    public bool CanClaimFree(int tierIndex)
    {
        if (rewardData == null)
        {
            Debug.LogError("rewardData == NULL");
            return false;
        }

        if (tierIndex < 0 || tierIndex >= rewardData.tiers.Count)
        {
            Debug.LogError("tierIndex OUT OF RANGE: " + tierIndex);
            return false;
        }

        bool enoughGem = currentGem >= rewardData.tiers[tierIndex].requiredGem;
        bool orderOK = tierIndex == lastClaimedTier + 1;

        Debug.Log($"Check CanClaimFree: enoughGem={enoughGem}, orderOK={orderOK}");

        return enoughGem && orderOK;
    }

    public bool CanClaimVip(int tierIndex)
    {
        return CanClaimFree(tierIndex);
    }

    public void ClaimFree(int tierIndex)
    {
        DebugRewardState($"ClaimFree tier {tierIndex}");

        if (!CanClaimFree(tierIndex))
        {
            Debug.LogError("CanClaimFree == FALSE → STOP CLAIM");
            return;
        }

        RewardTier tier = rewardData.tiers[tierIndex];

        Debug.Log("tier = " + (tier == null ? "NULL" : "OK"));

        if (tier.freeRewardPrefab == null)
        {
            Debug.LogError("FREE REWARD PREFAB IS NULL !!!");
            return;
        }

        if (rewardParent == null)
        {
            Debug.LogError("rewardParent == NULL !!!");
            return;
        }

        Debug.Log($"Instantiate FREE => {tier.freeRewardPrefab.name}");
        // nhận phần thưởng ở đây nò 
        //GameObject freeObj = Instantiate(tier.freeRewardPrefab, rewardParent);

        //Debug.Log("Instantiate FREE SUCCESS → " + freeObj.name);

        lastClaimedTier = tierIndex;
    }

    public void ClaimVIP(int tierIndex)
    {
        DebugRewardState($"ClaimVIP tier {tierIndex}");

        if (!CanClaimVip(tierIndex))
        {
            Debug.LogError("CanClaimVip == FALSE → STOP CLAIM");
            return;
        }

        RewardTier tier = rewardData.tiers[tierIndex];

        if (tier.freeRewardPrefab == null)
        {
            Debug.LogError("FREE REWARD PREFAB IS NULL !!!");
            return;
        }

        if (tier.vipRewardPrefab == null)
        {
            Debug.LogError("VIP REWARD PREFAB IS NULL !!!");
            return;
        }

        if (rewardParent == null)
        {
            Debug.LogError("rewardParent == NULL !!!");
            return;
        }

        Debug.Log($"Instantiate FREE => {tier.freeRewardPrefab.name}");
        Debug.Log($"Instantiate VIP  => {tier.vipRewardPrefab.name}");
        // nhận phần thưởng nhưng vip hơn
        //GameObject freeObj = Instantiate(tier.freeRewardPrefab, rewardParent);
        //Debug.Log("FREE Instantiate Success → " + freeObj.name);

        //GameObject vipObj = Instantiate(tier.vipRewardPrefab, rewardParent);
        //Debug.Log("VIP Instantiate Success → " + vipObj.name);

        lastClaimedTier = tierIndex;
    }
}
