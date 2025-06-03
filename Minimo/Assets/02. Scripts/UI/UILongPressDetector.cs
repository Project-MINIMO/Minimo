using UnityEngine;
using UnityEngine.EventSystems;

using System;

public class UILongPressDetector : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public Action OnLongPress { private get; set; }

    [SerializeField] private float _holdTime = 1f;
    
    private bool _isPointerDown;
    private float _pressTime;

    public void OnPointerDown(PointerEventData eventData)
    {
        _isPointerDown = true;
        _pressTime = 0f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isPointerDown = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isPointerDown = false;
    }

    private void Update()
    {
        if (_isPointerDown)
        {
            _pressTime += Time.unscaledDeltaTime;
            if (_pressTime >= _holdTime)
            {
                _isPointerDown = false;
                OnLongPress?.Invoke();
            }
        }
    }
}