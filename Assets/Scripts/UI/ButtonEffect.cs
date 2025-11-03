using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonEffect : MonoBehaviour
{
    private Button button;
    private Vector3 originalScale;

    [Header("Animation Settings")]
    private float pressedScale = 0.8f;    // Tỉ lệ thu nhỏ khi nhấn
    private float duration = 0.12f;        // Thời gian thu nhỏ và phóng lại

    private void Awake()
    {
        button = GetComponent<Button>();
        originalScale = transform.localScale;

        // Gán sự kiện nhấn nút
        button.onClick.AddListener(PlayPressEffect);
    }

    private void PlayPressEffect()
    {
        // Hủy tween cũ (nếu có)
        transform.DOKill();

        // Hiệu ứng: thu nhỏ rồi phóng lại
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(originalScale * pressedScale, duration).SetEase(Ease.OutQuad));
        seq.Append(transform.DOScale(originalScale, duration).SetEase(Ease.OutBack));
    }
}
