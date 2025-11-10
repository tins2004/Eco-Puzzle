using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GemBox : MonoBehaviour
{
    private BoxUI boxUI;
    private AudioManager audioManager;

    [Header("Gem")]
    [SerializeField] private Image gemIcon;
    [SerializeField] private TMP_Text gemText;

    void Awake()
    {
        gameObject.SetActive(true);
        gameObject.GetComponent<CanvasGroup>().alpha = 0;
    }

    void Start()
    {
        SetUp();
    }
    
    void SetUp()
    {
        boxUI = GetComponent<BoxUI>();

        gemIcon.transform.localScale = Vector3.zero;
        audioManager = AudioManager.Instance;
    }

    /// <summary>
    /// "isRetry": 0 Home, 1 Retry, 2 Next Level
    /// </summary>
    public void ShowGem(bool isMinus, int sceneChange)
    {
        if (boxUI == null) SetUp();
        boxUI.ShowBox();

        if (audioManager == null)
            audioManager = AudioManager.Instance;

        audioManager.PlaySFXOpenGemBox();

        gemIcon.gameObject.SetActive(true);

        int valueChange = 10;
        // Debug.Log("Giá trị thay đổi: " + valueChange);
        gemText.text = "0";

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.5f);
        seq.Append(gemIcon.transform.DOScale(1, 0.2f).SetEase(Ease.OutBack));

        seq.AppendInterval(0.5f);
        if (isMinus)
        {
            valueChange = Random.Range(10, 30);
            seq.AppendCallback(() =>
            DOTween.To(() => 0, x =>
            {
                valueChange = x;
                gemText.text = "-" + valueChange.ToString();
            }, valueChange, 0.8f)
            .SetEase(Ease.InQuad));
        }
        else
        {
            valueChange = Random.Range(4, 18);
            seq.AppendCallback(() =>
            DOTween.To(() => 0, x =>
            {
                valueChange = x;
                gemText.text = "+" + valueChange.ToString();
            }, valueChange, 0.8f)
            .SetEase(Ease.InQuad));
        }

        seq.AppendInterval(1f);
        seq.Append(gemText.transform.DOPunchScale(Vector3.one * 0.5f, 0.3f, 8, 1));
        seq.JoinCallback(() => audioManager.PlaySFXGemReceive());
        seq.JoinCallback(() => audioManager.VibrateClassic());

        seq.AppendInterval(0.3f);
        seq.Append(gemText.transform.DOScale(0, 0.2f).SetEase(Ease.InBack));
        seq.JoinCallback(() => audioManager.PlaySFXFailClick());
        seq.Append(gemIcon.transform.DOScale(0, 0.2f).SetEase(Ease.InBack));
        seq.JoinCallback(() => audioManager.PlaySFXFailClick());
        seq.AppendCallback(() =>
        {
            if (isMinus == true)
            {
                FireBaseAnalytics.Instance.LogLevelFail(GameData.GetCurrentLevel());
                GameData.MinusGem(valueChange);
            }
            else GameData.AddGem(valueChange);

            boxUI.HideBox();
            LevelManager levelManager = FindObjectOfType<LevelManager>();

            switch (sceneChange)
            {
                case 0: //Home
                    levelManager.ChangeToHomeScene();
                    break;
                case 1: //Retry
                    levelManager.NextLevel(false);
                    break;
                case 2: //Next Level
                    levelManager.NextLevel(true);
                    break;
                default:
                    Debug.LogError("Sai giá trị chuyển Scene");
                    break;
            }
        });


        seq.Play();
    }
}
