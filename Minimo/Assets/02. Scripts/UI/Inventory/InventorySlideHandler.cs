using System;
 
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class InventorySlideHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public event Action OnClose;
    
    [SerializeField] private RectTransform _targetRect;
    
    [SerializeField] private float _minY = -725f;
    [SerializeField] private float _maxY = -20f;
    [SerializeField] private float _outerMargin = 20f;

    [SerializeField] private bool _openWhenEnable = true;

    private const float _duration = 0.25f;
    
    private Vector2 _dragStartPos;
    private Vector2 _imageStartPos;
    
    private float _prevDragY;
    private float _lastDragDelta;

    private void OnEnable()
    {
        _targetRect.anchoredPosition = new Vector2(_targetRect.anchoredPosition.x, _openWhenEnable ? _maxY : _minY);
    }

    public void Open()
    {
        SnapTo(_maxY);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _dragStartPos = eventData.position;
        _imageStartPos = _targetRect.anchoredPosition;
        _prevDragY = _dragStartPos.y;
        _lastDragDelta = 0f;
        
        _targetRect.DOKill();
    }

    public void OnDrag(PointerEventData eventData)
    {
        var currentY = eventData.position.y;
        var deltaY = currentY - _dragStartPos.y;
        var unclampedY = _imageStartPos.y + deltaY;
        var targetY = Mathf.Clamp(unclampedY, _minY - _outerMargin, _maxY + _outerMargin);

        _targetRect.anchoredPosition = new Vector2(_imageStartPos.x, targetY);

        _lastDragDelta = currentY - _prevDragY;
        _prevDragY = currentY;
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
        else
        {
            SnapTo(_lastDragDelta > 0f ? _maxY : _minY);
        }
    }
    
    private void SnapTo(float targetY)
    {
        if (Mathf.Approximately(targetY, _minY)) OnClose?.Invoke();
        
        _targetRect.DOAnchorPosY(targetY, _duration)
            .SetEase(Ease.OutCubic);
    }
}