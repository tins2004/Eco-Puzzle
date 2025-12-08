using UnityEngine;

public class BoosterManager : MonoBehaviour
{
    [SerializeField] public BoosterSwapTile boosterSwapTile;
    [SerializeField] public BoosterUpgrade boosterUpgrade;
    [SerializeField] public BoosterRetry boosterRetry;

    [Header("Gem Box")]
    [SerializeField] public GemBox gemBox;

    public void SetBoosters()
    {
        boosterSwapTile.SetUpBuyBox(gemBox);
        boosterUpgrade.SetUpBuyBox(gemBox);
        // boosterRetry.SetUpBuyBox(gemBox);
    }

    void Update()
    {
        if (boosterSwapTile.IsReady())
        {
            boosterUpgrade.gameObject.SetActive(false);
            boosterRetry.gameObject.SetActive(false);
        }
        else if (boosterUpgrade.IsReady())
        {
            boosterSwapTile.gameObject.SetActive(false);
            boosterRetry.gameObject.SetActive(false);
        }
        else
        {
            boosterSwapTile.gameObject.SetActive(true);
            boosterUpgrade.gameObject.SetActive(true);
            boosterRetry.gameObject.SetActive(true);
        }
    }
}
