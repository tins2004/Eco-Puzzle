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
    public string text_Chinese;
    public string text_Japanese;
    public string text_Korean;
    public string text_Spanish;
    public string text_Portuguese;
    public string text_French;
    public string text_German;
    public string text_Russian;
    public string text_Thai;
}

[CreateAssetMenu(fileName = "TutorialData", menuName = "Data/Tutorial Data")]
public class TutorialData : ScriptableObject
{
    public List<TutorialStep> steps = new List<TutorialStep>();
}
