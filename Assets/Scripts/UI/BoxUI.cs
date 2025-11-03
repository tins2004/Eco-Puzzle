using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class BoxUI : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

        // Ẩn ban đầu
        canvasGroup.alpha = 0;
        rectTransform.localScale = Vector3.zero;
    }

    public void ShowBox()
    {
        if (gameObject.active) return;

        gameObject.SetActive(true);

        rectTransform.localScale = Vector3.one * 2f; // bắt đầu hơi to
        canvasGroup.alpha = 0f;

        Sequence seq = DOTween.Sequence();
        seq.Append(rectTransform.DOScale(1f, 0.3f).SetEase(Ease.OutBack)); // thu nhỏ lại vừa
        seq.Join(canvasGroup.DOFade(1f, 0.3f)); // rõ dần
    }

    public void HideBox()
    {       
        Sequence seq = DOTween.Sequence();
        seq.Append(rectTransform.DOScale(1.2f, 0.3f).SetEase(Ease.InBack)); // phóng to
        seq.Join(canvasGroup.DOFade(0f, 0.3f)); // mờ dần
        seq.OnComplete(() => gameObject.SetActive(false)); // ẩn hẳn sau khi xong
    }

    public bool IsShowing()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        return (canvasGroup.alpha > 0 && gameObject.activeSelf);
    }
}
