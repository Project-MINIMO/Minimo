using UnityEngine;
using UnityEngine.EventSystems;

using System;

public class UILongPressDetector : MonoBehaviour
{
    public Action OnLongPress { private get; set; }

    [SerializeField] private float _holdTime = 1f;

    private RectTransform _targetRect;
    
    private bool _isPressing = false;
    private float _pressTime = 0f;

    private void Start()
    {
        _targetRect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(_targetRect, Input.mousePosition))
            {
                _isPressing = true;
                _pressTime = 0f;
            }
        }

        if (_isPressing)
        {
            if (Input.GetMouseButton(0))
            {
                _pressTime += Time.deltaTime;
                if (_pressTime >= _holdTime)
                {
                    _isPressing = false;
                    OnLongPress?.Invoke();
                }
            }
            else
            {
                _isPressing = false;
            }
        }
    }
}