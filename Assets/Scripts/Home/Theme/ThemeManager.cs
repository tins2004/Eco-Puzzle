using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ThemeManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ThemeData themeData;

    [Header("UI")]
    [SerializeField] private ObjectPool themeItemPool;
    [SerializeField] private Sprite choseIcon;
    [SerializeField] private Sprite lockIcon;

    private AudioManager audioManager;

    void Start()
    {
        if (themeData == null || themeItemPool == null)
        {
            Debug.LogError("Lỗi khi khởi tạo Theme Manager: Thiếu dữ liệu hoặc tham chiếu UI.");
            return;
        }

        audioManager = AudioManager.Instance;
    }

    public void DisplayTheme()
    {   
        foreach (Transform child in themeItemPool.transform)
        {
            themeItemPool.ReturnObject(child.gameObject);
        }

        for (int i = 0; i < themeData.listTheme.Count; i++)
        {
            GameObject themeItem = themeItemPool.GetObject();
            themeItem.transform.localScale = Vector3.one;

            Image[] itemImage = themeItem.GetComponentsInChildren<Image>(true);
            itemImage[0].sprite = themeData.listTheme[i].T05;
            itemImage[1].gameObject.SetActive(true);

            themeItem.GetComponent<Button>().onClick.AddListener(() =>
            {
                audioManager.PlaySFXFailClick();
                itemImage[1].transform.DOPunchScale(
                    Vector3.one * 0.6f,
                    0.35f,
                    12,
                    0.8f
                ).OnComplete(() =>
                {
                    itemImage[1].transform.localScale = Vector3.one;
                });
            });

            foreach (int ownedIdTheme in GameData.GetOwnedThemeArray())
            {
                if (i != ownedIdTheme && i != GameData.GetIdTheme())
                {
                    Debug.Log("Locked Theme: " + i);
                    itemImage[1].sprite = lockIcon;
                    continue;
                }
                
                if (i == GameData.GetIdTheme())
                {
                    Debug.Log("Chose Theme: " + i);
                    itemImage[1].sprite = choseIcon;
                    continue;
                }
                
                itemImage[1].gameObject.SetActive(false);
                themeItem.GetComponent<Button>().onClick.RemoveAllListeners();
                themeItem.GetComponent<Button>().onClick.AddListener(() =>
                {
                    audioManager.PlaySFXButton();
                    ChoseNewTheme(themeItem);
                });
            }
        }
    }

    private void ChoseNewTheme(GameObject newTheme)
    {
        GameObject oldTheme = themeItemPool.transform.GetChild(GameData.GetIdTheme()).gameObject;
        oldTheme.GetComponent<Button>().onClick.RemoveAllListeners();
        oldTheme.GetComponent<Button>().onClick.AddListener(() =>
        {
            audioManager.PlaySFXButton();
            ChoseNewTheme(oldTheme);
        });
        oldTheme.GetComponentsInChildren<Image>(true)[1].gameObject.SetActive(false);

        newTheme.GetComponent<Button>().onClick.RemoveAllListeners();
        newTheme.GetComponent<Button>().onClick.AddListener(() =>
        {
            audioManager.PlaySFXButton();
            newTheme.GetComponentsInChildren<Image>()[1].transform.DOPunchScale(
                Vector3.one * 0.6f,
                0.35f,
                12,
                0.8f
            ).OnComplete(() =>
            {
                newTheme.GetComponentsInChildren<Image>(true)[1].transform.localScale = Vector3.one;
            });
        });
        newTheme.GetComponentsInChildren<Image>(true)[1].gameObject.SetActive(true);
        newTheme.GetComponentsInChildren<Image>(true)[1].sprite = choseIcon;
        
        GameData.SetIdTheme(int.Parse(newTheme.name[^1].ToString()));
    }
}
