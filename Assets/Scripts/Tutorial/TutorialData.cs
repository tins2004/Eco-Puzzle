using System;
using System.Collections.Generic;
using UnityEngine;

public enum TutorialTargetType
{
    None,
    Text,
    Tile,
    Animal,
    UIButton
}

[Serializable]
public class TutorialStep
{
    public TutorialTargetType targetType = TutorialTargetType.None;

    [Header("Target Position/Reference")]
    public Vector2Int tilePos; // cho tile
    public int animalIndex; // cho animal
    public string buttonName; // cho UI button

    [Header("Texts")]
    public string text_VN;
    public string text_EN;
}

[CreateAssetMenu(fileName = "TutorialData", menuName = "Data/Tutorial Data")]
public class TutorialData : ScriptableObject
{
    public List<TutorialStep> steps = new List<TutorialStep>();
}
