using System;
using System.Collections.Generic;
using UnityEngine;

public enum RewardType
{
    R00_NONE,
    R01_GEM,
    R02_BOOSTER_UPGRADE,
    R03_BOOSTER_SWAP,
    R04_BOOSTER_KEY,
    R05_THEME,
    R06_CHEST
}

public enum ChestType
{
    C00_RANDOM_CHEST,
    C01_BOOSTER_CHEST,
    C02_GEM_LOW_CHEST,
    C03_GEM_MEDIUM_CHEST,
    C04_GEM_HIGH_CHEST,
    C05_GEM_RANDOM_CHEST
}

[Serializable]
public class RewardItem
{
    public RewardType itemType;
    public Sprite itemIcon;

    public ThemeType themeType; //Cho theme
    public ChestType chestType; //Cho chest
    public List<ChestData> chestData = new List<ChestData>();
}

[Serializable]
public class ChestData
{
    public RewardType rewardType;
    public Vector2Int numberBetween;   
}


[CreateAssetMenu(fileName = "RewardData", menuName = "Data/Reward Data")]
public class RewardData : ScriptableObject
{
    public List<RewardItem> rewardItems = new List<RewardItem>();

    public Sprite GetIconByRewardType(RewardType rewardType, ThemeType themeType = ThemeType.TH00_ECO, ChestType chestType = ChestType.C00_RANDOM_CHEST)
    {
        foreach (RewardItem item in rewardItems)
        {
            if (item.itemType == rewardType)
            {
                if (rewardType == RewardType.R05_THEME && item.themeType == themeType)
                {
                    return item.itemIcon;
                }
                else if (rewardType == RewardType.R06_CHEST && item.chestType == chestType)
                {
                    return item.itemIcon;
                }
                else if (rewardType != RewardType.R05_THEME && rewardType != RewardType.R06_CHEST)
                {
                    return item.itemIcon;
                }
            }
        }
        return null;
    }

    public List<ChestData> GetChestDataByType(ChestType chestType)
    {
        foreach (RewardItem item in rewardItems)
        {
            if (item.itemType == RewardType.R06_CHEST && item.chestType == chestType)
            {
                if (chestType == ChestType.C00_RANDOM_CHEST)
                {
                    int boosterChest = 0;
                    for (int i = 0; i < rewardItems.Count; i++)
                    {
                        if (rewardItems[i].itemType == RewardType.R06_CHEST && 
                        (rewardItems[i].chestType == ChestType.C01_BOOSTER_CHEST || 
                         rewardItems[i].chestType == ChestType.C05_GEM_RANDOM_CHEST))
                        {
                            boosterChest = i;

                            if (UnityEngine.Random.Range(0, 2) == 0)
                                return GetChestDataByType(rewardItems[i].chestType);
                        }
                    }

                    return rewardItems[boosterChest].chestData;
                }
                
                else if (chestType == ChestType.C05_GEM_RANDOM_CHEST)
                {
                    int gemInLowChest = 0;
                    for (int i = 0; i < rewardItems.Count; i++)
                    {
                        if (rewardItems[i].itemType == RewardType.R06_CHEST && 
                        (rewardItems[i].chestType == ChestType.C02_GEM_LOW_CHEST || 
                         rewardItems[i].chestType != ChestType.C03_GEM_MEDIUM_CHEST || 
                         rewardItems[i].chestType != ChestType.C04_GEM_HIGH_CHEST))
                        {
                            gemInLowChest = i;

                            if (UnityEngine.Random.Range(0, 2) == 0)
                                return rewardItems[i].chestData;
                        }
                    }

                    return rewardItems[gemInLowChest].chestData;
                }

                return item.chestData;
            }
        }
        return null;
    }

    public Sprite GetRewardIconByType(RewardType rewardType, ChestType chestType)
    {
        foreach (RewardItem item in rewardItems)
        {
            if (item.itemType == rewardType && rewardType != RewardType.R06_CHEST)
                return item.itemIcon; 

            else if (item.itemType == RewardType.R06_CHEST && item.chestType == chestType)
                return item.itemIcon;
        }
        return null;
    }
}