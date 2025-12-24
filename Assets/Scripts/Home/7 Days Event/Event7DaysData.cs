using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RewardOfDay
{
    public int dayNumber;

    public RewardType rewardType;
    public int quantity;         
    public ChestType chestType;
    public ThemeType themeType;
}

[CreateAssetMenu(fileName = "Event7DaysData", menuName = "Data/Event7Days Data")]
public class Event7DaysData : ScriptableObject
{
    public List<RewardOfDay> rewardItems = new List<RewardOfDay>();
}
