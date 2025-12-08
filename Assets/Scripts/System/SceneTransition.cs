using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    // private static SceneTransition instance;

    [SerializeField] private Image effectTransition;
    [SerializeField] private Color effectColor;
    [SerializeField] private Image tileIcon;
    [SerializeField] private ThemeData themeData;
    private Sprite[] tileSprites;

    private AudioManager audioManager;

    // private void Awake()
    // {
    //     // Singleton để dùng giữa các scene
    //     if (instance == null)
    //     {
    //         instance = this;
    //         DontDestroyOnLoad(gameObject); // giữ lại qua scene
    //     }
    //     else
    //     {
    //         Destroy(gameObject);
    //     }
    // }    

    public void OpenEffect(string nextScene, bool useEffect = true)
    {
        if (audioManager == null)
            audioManager = AudioManager.Instance;

        audioManager.StopMusicBackground();


        if (useEffect)
        {
            StartCoroutine(OpenAndLoad(nextScene));
        }
        else
        {
            StartCoroutine(OpenAndLoadShort(nextScene));
        }
    }

    private void LoadSprite()
    {
        if (themeData == null)
        {
            Debug.LogError("Chưa gán Theme Data cho Transition");
        }

        ThemeSprite themeSprite = themeData.listTheme[GameData.GetIdTheme()];
        tileSprites = new Sprite[] {themeSprite.T01, themeSprite.T03, themeSprite.T04, themeSprite.T05};
    }


    private IEnumerator OpenAndLoad(string nextScene)
    {
        if (tileSprites == null || tileSprites.Length == 0)
            LoadSprite();

        tileIcon.gameObject.SetActive(true);
        tileIcon.transform.localScale = Vector3.zero;
        tileIcon.sprite = tileSprites[0];

        effectTransition.gameObject.SetActive(true);
        effectTransition.color = Color.white;
        effectTransition.transform.position = new Vector3(0, -4000f, 0);

        // Chuỗi hiệu ứng mở
        Sequence seq = DOTween.Sequence();
        seq.Append(effectTransition.transform.DOMove(Vector3.zero, 0.5f).SetEase(Ease.OutCubic));
        seq.Append(effectTransition.DOColor(effectColor, 0.5f));
        seq.JoinCallback(() => audioManager.PlaySFXSlideChange());
        seq.Append(tileIcon.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack));
        seq.JoinCallback(() => audioManager.PlaySFXPop());

        for (int i = 1; i < tileSprites.Length; i++)
        {
            seq.AppendInterval(0.2f);
            seq.Append(tileIcon.transform.DOMove(Vector3.up * 0.5f, 0.2f).SetEase(Ease.OutCubic));
            int index = i;
            seq.AppendCallback(() => tileIcon.sprite = tileSprites[index]);
            seq.JoinCallback(() => audioManager.PlaySFXPop());
            seq.Append(tileIcon.transform.DOMove(Vector3.zero, 0.1f).SetEase(Ease.InCubic));
        }

        seq.AppendInterval(0.1f);
        // Bắt đầu load scene async (song song với hiệu ứng)
        AsyncOperation async = SceneManager.LoadSceneAsync(nextScene);
        async.allowSceneActivation = false; // chờ hiệu ứng xong mới chuyển

        // Chờ hiệu ứng hoàn thành
        yield return seq.WaitForCompletion();

        DOTween.KillAll();

        // Cho phép load xong
        async.allowSceneActivation = true;
    }

    private IEnumerator OpenAndLoadShort(string nextScene)
    {
        if (tileSprites == null || tileSprites.Length == 0)
            LoadSprite();

        tileIcon.gameObject.SetActive(true);
        tileIcon.transform.localScale = Vector3.zero;
        tileIcon.sprite = tileSprites[tileSprites.Length -1];

        effectTransition.gameObject.SetActive(true);
        effectTransition.color = Color.white;
        effectTransition.transform.position = new Vector3(0, -4000f, 0);

        // Chuỗi hiệu ứng mở
        Sequence seq = DOTween.Sequence();
        seq.Append(effectTransition.transform.DOMove(Vector3.zero, 0.5f).SetEase(Ease.OutCubic));
        seq.Append(effectTransition.DOColor(effectColor, 0.5f));
        seq.JoinCallback(() => audioManager.PlaySFXSlideChange());
        seq.Append(tileIcon.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack));
        seq.JoinCallback(() => audioManager.PlaySFXPop());

        seq.AppendInterval(0.1f);
        // Bắt đầu load scene async (song song với hiệu ứng)
        AsyncOperation async = SceneManager.LoadSceneAsync(nextScene);
        async.allowSceneActivation = false; // chờ hiệu ứng xong mới chuyển

        // Chờ hiệu ứng hoàn thành
        yield return seq.WaitForCompletion();

        DOTween.KillAll();

        // Cho phép load xong
        async.allowSceneActivation = true;
    }

    public void CloseEffect()
    {
        if (audioManager == null)
            audioManager = AudioManager.Instance;
        
        if (tileSprites == null || tileSprites.Length == 0)
            LoadSprite();

        tileIcon.gameObject.SetActive(true);
        tileIcon.transform.localScale = Vector3.one;
        tileIcon.sprite = tileSprites[tileSprites.Length - 1];

        effectTransition.gameObject.SetActive(true);
        effectTransition.color = effectColor;
        effectTransition.transform.position = Vector3.zero;

        Sequence seq = DOTween.Sequence();

        seq.AppendInterval(0.2f);
        seq.Append(tileIcon.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack));
        seq.JoinCallback(() => audioManager.PlaySFXSlideChange());
        seq.Append(effectTransition.transform.DOMove(new Vector3(0, 4000f, 0), 0.2f).SetEase(Ease.InCubic));
        seq.AppendCallback(() => effectTransition.gameObject.SetActive(false));
        seq.OnComplete(() => tileIcon.gameObject.SetActive(false));
    }
}
