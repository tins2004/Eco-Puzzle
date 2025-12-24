using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattlePassManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BattlePassData battlePassData; 
    [SerializeField] private RewardData rewardData; 
    // public RewardProgression progression;  

    [Header("UI")]
    [SerializeField] private GameObject[] items; // 0: Free Bot, 1: VIP Bot, 2: Free Top, 3: VIP Top, 4: Target Gem Bot, 5: Target Gem Top
    [SerializeField] private Button nextButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button vipBattlePassButton;
    [SerializeField] private Transform homeTarget; // Nơi hiển thị hiệu ứng nhận phần thưởng

    [SerializeField] private Sprite[] statusSprite = new Sprite[2];    

    private int currentIndex = 0;

    private AudioManager audioManager;

    void Start()
    {
        if (battlePassData == null || battlePassData.steps.Count < 2 || items.Length < 2)
        {
            Debug.LogError("Lỗi khi khởi tạo BattlePassManager: Thiếu dữ liệu hoặc tham chiếu UI.");
            return;
        }

        audioManager = AudioManager.Instance;

        UpdateUI(GetCurrentIndexCanGive());

        nextButton.onClick.AddListener(() =>
        {
            if (currentIndex + 1 >= battlePassData.steps.Count - 1)
                return;

            audioManager.PlaySFXButton();

            int nextIndex = currentIndex + 1;
            if (nextIndex < battlePassData.steps.Count)
            {
                UpdateUI(nextIndex);
            }
        });

        backButton.onClick.AddListener(() =>
        {
            if (currentIndex <= 0)
                return;

            audioManager.PlaySFXButton();

            int prevIndex = currentIndex - 1;
            if (prevIndex >= 0)
            {
                UpdateUI(prevIndex);
            }
        });

        if (GameData.GetVIPBattlePass() == 0)
        {
            vipBattlePassButton.onClick.AddListener(() =>
            {
                audioManager.PlaySFXGemReceive();

                items[1].transform.DOPunchScale(
                    Vector3.one * 0.6f,
                    0.35f,
                    12,
                    0.8f
                );

                items[3].transform.DOPunchScale(
                    Vector3.one * 0.6f,
                    0.35f,
                    12,
                    0.8f
                );
    
                GameData.SetVIPBattlePass(1);
                UpdateUI(currentIndex);
                vipBattlePassButton.gameObject.SetActive(false);  
            });
        }
        else
        {
            vipBattlePassButton.gameObject.SetActive(false);
        }
    }

    private void SetPickUpItemButton(Button button, bool isVip, int index, Image imgItem, RewardType rewardType, int quantity, ThemeType themeType, ChestType chestType)
    {
        button.onClick.RemoveAllListeners();

        if (rewardType == RewardType.R00_NONE || quantity <= 0)
            return;

        button.onClick.AddListener(() =>
        {
            if (quantity <= 0)
                return;
            
            audioManager.PlaySFXGemReceive();
            


            // Debug.Log("Nhận phần thưởng: " + rewardType.ToString());
            switch (rewardType)
            {
                case RewardType.R01_GEM:
                    GameData.AddGem(quantity);
                    StartPickUpRewardEffect(imgItem, homeTarget.position, homeTarget);
                    break;
                case RewardType.R02_BOOSTER_UPGRADE:
                    GameData.AddNumberOfBoosterUpgrade(quantity);
                    StartPickUpRewardEffect(imgItem, homeTarget.position, homeTarget);
                    break;
                case RewardType.R03_BOOSTER_SWAP:
                    GameData.AddNumberOfBoosterSwap(quantity);
                    StartPickUpRewardEffect(imgItem, homeTarget.position, homeTarget);
                    break;
                case RewardType.R04_BOOSTER_KEY:
                    GameData.AddNumberOfBoosterLocker(quantity);
                    StartPickUpRewardEffect(imgItem, homeTarget.position, homeTarget);
                    break;
                case RewardType.R05_THEME:
                    GameData.AddOwnedTheme((int)themeType);
                    StartPickUpRewardEffect(imgItem, homeTarget.position, homeTarget);
                    break;
                case RewardType.R06_CHEST:
                    StartOpenChestEffect(chestType);
                    // Debug.Log("Nhận chest: " + (int)chestType);
                    break;
                default:
                    Debug.LogWarning("Loại phần thưởng không hợp lệ.");
                    break;
            }

            GameData.AddBattlePassData(!isVip ? "F" + index : "V" + index);
            UpdateUI(currentIndex);
        });
    }

    private void StartPickUpRewardEffect(Image effect, Vector3 targetPos, Transform transformTaget)
    {
        effect.color = new Color(effect.color.r, effect.color.g, effect.color.b, 1);
        Vector3 originalPos = effect.transform.position;

        Canvas canvas = effect.GetComponent<Canvas>();

        canvas.overrideSorting = true;
        canvas.sortingOrder = 9999; 

        Sequence seq = DOTween.Sequence();

        seq.Append(effect.transform.DOScale(3f, 0.2f).SetEase(Ease.OutBack)); // phóng to như nấm mọc
        seq.JoinCallback(() => audioManager.PlaySFXPop()); // phóng to như nấm mọc
        seq.Append(effect.transform.DOMove(targetPos, 0.4f).SetEase(Ease.InQuad)); // bay tới đích
        seq.Join(effect.transform.DOScale(0.7f, 0.4f)); // trong lúc bay thì hơi thu nhỏ lại
        seq.AppendCallback(() =>
        {
            // Hiệu ứng nổ tung/biến mất
            audioManager.PlaySFXCheckMission();
            effect.transform.DOScale(1f, 0.2f).SetEase(Ease.OutQuad);
            effect.color = new Color(effect.color.r, effect.color.g, effect.color.b, 0);

            // Item mission rung nhẹ khi nhận
            transformTaget.DOPunchScale(Vector3.one * 0.2f, 0.3f, 5, 0.5f);
        });
        seq.AppendInterval(0.05f);
        seq.OnComplete(() =>
        {
            effect.color = new Color(effect.color.r, effect.color.g, effect.color.b, 1);
            effect.transform.position = originalPos;
            effect.transform.localScale = Vector3.one;
            canvas.sortingOrder = 99;
        });
    }

    // private Dictionary<RewardType, int> GetRewardFromChest(ChestType chestType)
    // {
    //     List<ChestData> chest = rewardData.rewardItems.Find(item => item.itemType == RewardType.R06_CHEST && item.chestType == chestType).chestData;

    //     foreach (ChestData chestData in chest)
    //     {
    //         Debug.Log("Chest Data: " + chestData.rewardType + " - " + chestData.numberBetween.x + " to " + chestData.numberBetween.y);
    //     }
    //     // int randomAmount = Random.Range(chest.numberBetween.x, chest.numberBetween.y + 1);

    //     // // dictionary lưu tạm
    //     // var rewardDict = new Dictionary<RewardType, int>();
    //     // rewardDict[chest.rewardType] = randomAmount;

    //     // return rewardDict;
    // }

    private void StartOpenChestEffect(ChestType chestType)
    {
        List<ChestData> chests = rewardData.GetChestDataByType(chestType);
    
        FindObjectOfType<HomeManager>().ShowItemReward(chests, chestType, rewardData);
        // foreach (ChestData chestData in chests)
        // {
        //     Debug.Log(chestType + " Chest Data: " + chestData.rewardType + " - " + chestData.numberBetween.x + " to " + chestData.numberBetween.y);
        // }
        // rewardBackground.localScale = Vector3.zero;


        // GameObject item = itemRewardPool.GetObject();
        // item.GetComponentInChildren<Image>(true).sprite = 

    }

    private void UpdateUI(int index)
    {
        if (index < 0)
            return;
        
        if (index >= battlePassData.steps.Count - 1)
            index = battlePassData.steps.Count - 2;

        currentIndex = index;
        // Debug.Log("Cập nhật UI Battle Pass, index: " + index + " và count - 1: " + (battlePassData.steps.Count - 1));
        bool[] received = new bool[4] { false, false, false, false }; // 0: Free index, 1: VIP index, 2: Free index+1, 3: VIP index+1
        
        foreach (string data in GameData.GetBattlePassDataArray())
        {
            // data dạng "F1", "V2", ...
            if (data.Length < 2) continue;

            // lấy chữ
            char type = data[0];     // 'F' hoặc 'V'
            // lấy số
            int num = int.Parse(data.Substring(1));

            if (num == index)
            {
                if (type == 'F')
                    received[0] = true;
                else if (type == 'V')
                    received[1] = true;
            }
            if (num == index + 1)
            {
                if (type == 'F')
                    received[2] = true;
                else if (type == 'V')
                    received[3] = true;
            }
        }
        

        DisplayReward(isBotBox: false, index + 1, received[2], received[3]);
        DisplayReward(isBotBox: true, index, received[0], received[1]);

        backButton.GetComponent<Image>().color = index > 0 ? Color.white : Color.gray;
        nextButton.GetComponent<Image>().color = (index + 1 < battlePassData.steps.Count - 1) ? Color.white : Color.gray;
    }

    private void DisplayReward(bool isBotBox, int index, bool receivedFree, bool receivedVIP)
    {
        bool canGive = index <= GetCurrentIndexCanGive();

        TMP_Text targetGemText = items[isBotBox ? 4 : 5].GetComponentInChildren<TMP_Text>();
        targetGemText.text = battlePassData.steps[index].targetGem.ToString() + "\nGem";
        targetGemText.color = canGive ? Color.green : Color.red;

        // Free Item
        Image[] iconImage = items[isBotBox ? 0 : 2].transform.Find("Image").GetComponentsInChildren<Image>(true);
        // iconImage.sprite = battlePassData.steps[index].iconFree;
        iconImage[0].sprite = rewardData.GetIconByRewardType(battlePassData.steps[index].freeRewardType, battlePassData.steps[index].themeTypeFree, battlePassData.steps[index].chestTypeFree);
        TMP_Text quantityText = items[isBotBox ? 0 : 2].transform.Find("Text").GetComponent<TMP_Text>();
        quantityText.text = battlePassData.steps[index].quantityFree.ToString();

        Button freeButton = items[isBotBox ? 0 : 2].GetComponent<Button>();
        freeButton.onClick.RemoveAllListeners();
        // freeButton.GetComponent<Image>().color = canGive ? receivedFree ? Color.yellow : Color.white : Color.red;
        if (canGive)
        {
            iconImage[1].gameObject.SetActive(false);   
        }
        else
        {
            iconImage[1].gameObject.SetActive(true);
            iconImage[1].sprite = receivedFree ? statusSprite[1] : statusSprite[0];
        }

        int quantity = canGive && !receivedFree ? battlePassData.steps[index].quantityFree : 0;
        SetPickUpItemButton(freeButton, false, index, iconImage[0], battlePassData.steps[index].freeRewardType, quantity, battlePassData.steps[index].themeTypeFree, battlePassData.steps[index].chestTypeFree);


        // VIP Item
        iconImage = items[isBotBox ? 1 : 3].transform.Find("Image").GetComponentsInChildren<Image>(true);
        // iconImage.sprite = battlePassData.steps[index].iconVip;
        iconImage[0].sprite = rewardData.GetIconByRewardType(battlePassData.steps[index].vipRewardType, battlePassData.steps[index].themeTypeVip, battlePassData.steps[index].chestTypeVip);
        quantityText = items[isBotBox ? 1 : 3].transform.Find("Text").GetComponent<TMP_Text>();
        quantityText.text = battlePassData.steps[index].quantityVip.ToString();

        Button vipButton = items[isBotBox ? 1 : 3].GetComponent<Button>();
        vipButton.onClick.RemoveAllListeners();
        // vipButton.GetComponent<Image>().color = (canGive && GameData.GetVIPBattlePass() == 1) ? receivedVIP ? Color.yellow : Color.white : Color.red;
        if (canGive && GameData.GetVIPBattlePass() == 1)
        {
            iconImage[1].gameObject.SetActive(false);   
        }
        else
        {
            iconImage[1].gameObject.SetActive(true);
            iconImage[1].sprite = receivedVIP ? statusSprite[1] : statusSprite[0];
        }

        quantity = canGive && GameData.GetVIPBattlePass() == 1 && !receivedVIP ? battlePassData.steps[index].quantityVip : 0;
        SetPickUpItemButton(vipButton, true, index, iconImage[0], battlePassData.steps[index].vipRewardType, quantity, battlePassData.steps[index].themeTypeVip, battlePassData.steps[index].chestTypeVip);


    }

    private int GetCurrentIndexCanGive()
    {
        int currentGem = GameData.GetCurrentGem();
        for (int i = battlePassData.steps.Count - 1; i >= 0; i--)
        {
            if (currentGem >= battlePassData.steps[i].targetGem)
            {
                return i;
            }
        }
        return 0;
    }

    private void DisplayTile(){
        
    }
}
