using UnityEngine;

public class BoosterRetry : BoosterBase
{
    [Header("Gem Box")]
    [SerializeField] private GemBox gemBox;

    protected override void SetBoosterNumberKey()
    {
        boosterNumberKey = "BoosterRetry";
        PlayerPrefs.SetInt(boosterNumberKey, 1);
    }

    public override void Activate()
    {
        Debug.Log("Bắt đầu lại màn chơi!");
        // Consume();

        // LevelManager levelManager = FindObjectOfType<LevelManager>();
        // levelManager.NextLevel(isRetry: false);
        gemBox.ShowGem(true, 1);
    }

    public override void CancelActivate()
    {
        
    }
}
