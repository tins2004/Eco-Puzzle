using UnityEngine;

public class BoosterRetry : BoosterBase
{
    protected override void SetBoosterNumberKey()
    {
        boosterNumberKey = "BoosterRetry";
        PlayerPrefs.SetInt(boosterNumberKey, 1);
    }

    public override void Activate()
    {
        Debug.Log("Bắt đầu lại màn chơi!");
        // Consume();

        LevelManager levelManager = FindObjectOfType<LevelManager>();
        levelManager.RetryLevel();
    }

    public override void CancelActivate()
    {
        
    }
}
