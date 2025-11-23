using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RewardTier
{
    public int requiredGem;

    // Quà FREE là prefab dạng Button
    public GameObject freeRewardPrefab;

    // Quà VIP cũng là prefab
    public GameObject vipRewardPrefab;
}

[CreateAssetMenu(fileName = "RewardProgressData", menuName = "Reward/Progress Data")]
public class RewardProgressData : ScriptableObject
{
    public List<RewardTier> tiers = new List<RewardTier>();
}
