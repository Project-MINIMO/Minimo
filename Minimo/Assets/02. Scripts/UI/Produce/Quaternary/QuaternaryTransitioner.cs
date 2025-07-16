using UnityEngine;
using DG.Tweening;

public class QuaternaryTransitioner : MonoBehaviour
{
    [SerializeField] private RectTransform _quaternaryRect;
    [SerializeField] private InventorySlideHandler _slider;

    private void Awake()
    {
        _slider.OnClose += Close;
    }

    private void OnEnable()
    {
        _quaternaryRect.anchoredPosition = Vector2.zero;
    }

    public void Open()
    {
        _quaternaryRect.DOAnchorPosX(-640f, 0.25f).SetEase(Ease.OutCubic);
        _slider.Open();
    }

    private void Close()
    {
        _quaternaryRect.DOAnchorPosX(0f, 0.25f).SetEase(Ease.OutCubic);
    }
}
