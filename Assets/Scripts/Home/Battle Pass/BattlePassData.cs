using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BattlePassStep
{
    public int targetGem;

    public RewardType freeRewardType;
    // public Sprite iconFree;
    public int quantityFree; // cho gem, boosters
    public ChestType chestTypeFree;
    public ThemeType themeTypeFree;

    public RewardType vipRewardType;
    // public Sprite iconVip;
    public int quantityVip; // cho gem, boosters
    public ChestType chestTypeVip;
    public ThemeType themeTypeVip;
}


[CreateAssetMenu(fileName = "BattlePassData", menuName = "Data/Battle Pass Data")]
public class BattlePassData : ScriptableObject
{
    public List<BattlePassStep> steps = new List<BattlePassStep>();
}