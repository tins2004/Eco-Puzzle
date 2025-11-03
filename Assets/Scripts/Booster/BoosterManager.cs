using UnityEngine;

public class BoosterManager : MonoBehaviour
{
    [SerializeField] public BoosterSwapTile boosterSwapTile;
    [SerializeField] public BoosterUpgrade boosterUpgrade;
    [SerializeField] public BoosterRetry boosterRetry;

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
