using System;
using System.Collections.Generic;
using UnityEngine;

public enum ThemeType
{
    TH00_ECO,
    TH01_COUNTRYSIDE,
    TH02_CITY
}

[Serializable]
public class ThemeSprite
{
    public ThemeType themeType;
    public Sprite T01;
    public Sprite T02;
    public Sprite T03;
    public Sprite T04;
    public Sprite T05;
    public Sprite T06;
    public Sprite T07;
    public Sprite T08;
    public Sprite T09;
}

[CreateAssetMenu(fileName = "ThemeData", menuName = "Data/Theme Data")]
public class ThemeData : ScriptableObject
{
    public List<ThemeSprite> listTheme = new List<ThemeSprite>();
}
