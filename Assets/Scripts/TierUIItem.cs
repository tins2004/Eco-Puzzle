using UnityEngine;
using UnityEngine.UI;

public class TierUIItem : MonoBehaviour
{
    public Text requiredGemText;
    public Button freeButton;
    public Button vipButton;
    public Image background;

    public void Setup(int tierIndex, int requiredGem)
    {
        if (requiredGemText)
            requiredGemText.text = $"Tier {tierIndex + 1}\nGem: {requiredGem}";
    }

    public void SetStatusLocked()
    {
        background.color = new Color(0.5f, 0.5f, 0.5f); // xám
    }

    public void SetStatusReady()
    {
        background.color = new Color(1f, 1f, 0.6f); // vàng
    }

    public void SetStatusClaimed()
    {
        background.color = new Color(0.6f, 1f, 0.6f); // xanh
        freeButton.interactable = false;
        vipButton.interactable = false;
    }
}
