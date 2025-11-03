using UnityEngine;

public class BoosterRetry : BoosterBase
{
    protected override void SetBoosterNumberKey()
    {
        boosterNumberKey = "BoosterRetry";
    }

    public override void Activate()
    {
        Debug.Log("Bắt đầu lại màn chơi!");
        Consume();

        LevelManager levelManager = FindObjectOfType<LevelManager>();
        levelManager.NextLevel(isRetry: false);
    }

    public override void CancelActivate()
    {
        
    }
}
