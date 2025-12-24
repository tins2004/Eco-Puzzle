using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Event7DaysManager : MonoBehaviour
{
    [SerializeField] private Event7DaysData event7DaysData;
    [SerializeField] private RewardData rewardData; 

    [SerializeField] private Button[] itemRewardButton = new Button[7];
    [SerializeField] private bool[] rewardCanPick = new bool[7];
    [SerializeField] private Sprite[] statusSprite = new Sprite[2];    

    private AudioManager audioManager;

    void Start()
    {
        if (event7DaysData == null || event7DaysData.rewardItems.Count < 7 || itemRewardButton.Length < 7)
        {
            Debug.LogError("Lỗi khi khởi tạo Event7DaysManager: Thiếu dữ liệu hoặc tham chiếu UI.");
            return;
        }

        audioManager = AudioManager.Instance;

        for (int i = 0; i < 7; i++)
        {
            int dayIndex = i; 
            DisplayReward(dayIndex);
            itemRewardButton[i].onClick.AddListener(() => OnRewardButtonClicked(dayIndex));
        }
    }

    private void DisplayReward(int dayIndex)
    {
        RewardOfDay reward = event7DaysData.rewardItems[dayIndex];
        rewardCanPick[dayIndex] = true;
        
        itemRewardButton[dayIndex].GetComponentInChildren<TMP_Text>(true).text = $"x{reward.quantity}";
        itemRewardButton[dayIndex].GetComponentsInChildren<Image>(true)[1].sprite = rewardData.GetIconByRewardType(reward.rewardType, reward.themeType, reward.chestType);

        Image statusImage = itemRewardButton[dayIndex].GetComponentsInChildren<Image>(true)[2];
        statusImage.sprite = statusSprite[0];

        if ((GameData.GetEvent7DaysDataArray() == null || GameData.GetEvent7DaysDataArray().Length == 0) && reward.dayNumber == 1)
        {
            statusImage.gameObject.SetActive(false);
            rewardCanPick[dayIndex] = false;
            return;
        }

        foreach (int dayHadGet in GameData.GetEvent7DaysDataArray())
        {
            if (reward.dayNumber == dayHadGet)
            {
                statusImage.sprite = statusSprite[1];
                break;
            }
            else
            {
                rewardCanPick[dayIndex] = false;
                statusImage.gameObject.SetActive(false);
            }
        }

        int maxDayIndex = GameData.GetEvent7DaysDataArray()[GameData.GetEvent7DaysDataArray().Length - 1];
        int dayCanGet = GameData.GetNumberOfWaitingDays(FindObjectOfType<HomeManager>().currentDate);
        if (maxDayIndex + dayCanGet > dayIndex)
        {
            statusImage.sprite = statusSprite[0];
            rewardCanPick[dayIndex] = false;
            return;
        }
    }

    private void OnRewardButtonClicked(int dayIndex)
    {
        audioManager.PlaySFXButton();

        if (rewardCanPick[dayIndex])
        {
            itemRewardButton[dayIndex].GetComponentsInChildren<Image>(true)[2].transform.DOPunchScale(
                Vector3.one * 0.6f,
                0.35f,
                12,
                0.8f
            ).OnComplete(() =>
            {
                itemRewardButton[dayIndex].GetComponentsInChildren<Image>(true)[2].transform.localScale = Vector3.one;
            });
        }
        else
        {

            RewardOfDay reward = event7DaysData.rewardItems[dayIndex];
            
            // Debug.Log($"Nhận phần thưởng cho ngày {reward.dayNumber}: Loại phần thưởng - {reward.rewardType}, Số lượng - {reward.quantity}");
            
            itemRewardButton[dayIndex].GetComponentsInChildren<Image>(true)[2].gameObject.SetActive(true);
            itemRewardButton[dayIndex].GetComponentsInChildren<Image>(true)[2].sprite = statusSprite[1];
            rewardCanPick[dayIndex] = true;

            GameData.AddEvent7DaysData(reward.dayNumber);
        }
    }
}
