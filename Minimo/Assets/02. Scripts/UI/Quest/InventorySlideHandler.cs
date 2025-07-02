using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class InventorySlideHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private RectTransform _targetRect;
    
    [SerializeField] private float _minY = -725f;
    [SerializeField] private float _maxY = -20f;
    [SerializeField] private float _outerMargin = 20f;

    private Vector2 _dragStartPos;
    private Vector2 _imageStartPos;

    private void OnEnable()
    {
        _targetRect.anchoredPosition = new Vector2(-180f, _maxY);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _dragStartPos = eventData.position;
        _imageStartPos = _targetRect.anchoredPosition;
        
        _targetRect.DOKill();
    }

    public void OnDrag(PointerEventData eventData)
    {
        var deltaY = eventData.position.y - _dragStartPos.y;
        var unclampedY = _imageStartPos.y + deltaY;
        var targetY = Mathf.Clamp(unclampedY, _minY - _outerMargin, _maxY + _outerMargin);

        _targetRect.anchoredPosition = new Vector2(_imageStartPos.x, targetY);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var currentY = _targetRect.anchoredPosition.y;

        if (currentY > _maxY && currentY <= _maxY + _outerMargin)
        {
            SnapTo(_maxY);
        }
        else if (currentY < _minY && currentY >= _minY - _outerMargin)
        {
            SnapTo(_minY);
        }
    }
    
    private void SnapTo(float targetY)
    {
        _targetRect.DOAnchorPosY(targetY, 0.25f)
            .SetEase(Ease.OutCubic);
    }
}