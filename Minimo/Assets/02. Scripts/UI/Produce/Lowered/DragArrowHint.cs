using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DragArrowHint : MonoBehaviour
{
    private readonly Vector2 _moveOffset = new(-25f, 25f);
    private const float MoveDuration = 0.5f;

    private Vector2 _originalPos;

    private RectTransform _rect;
    private Image _image;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
        
        _originalPos = _rect.anchoredPosition;
    }

    private void OnEnable()
    {
        _rect.anchoredPosition = _originalPos;
        _rect.localScale = Vector3.one;
        _image.color = Color.white;
        
        StartArrowAnimation();
    }

    public void OnDragChanged(bool isDragging)
    {
        if (isDragging)
        {
            gameObject.SetActive(false);
        }
    }

    private void StartArrowAnimation()
    {
        _rect.DOAnchorPos(_originalPos - _moveOffset, MoveDuration)
            .SetEase(Ease.InCubic).SetLoops(-1, LoopType.Yoyo);
        _rect.DOScale(0.7f, MoveDuration)
            .SetEase(Ease.InCubic).SetLoops(-1, LoopType.Yoyo);

        _image.DOFade(0.3f, MoveDuration)
            .SetEase(Ease.InCubic).SetLoops(-1, LoopType.Yoyo);
    }

    private void OnDisable()
    {
        _rect.DOKill();
        _image.DOKill();
    }
}
