using UnityEngine;
using UnityEngine.UI;

public class IndicatorHandler : MonoBehaviour
{
    [SerializeField] private Image _icon;

    private Transform _target;
    private RectTransform _rect;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    public void Initialize(Transform target)
    {
        gameObject.SetActive(true);
        _target = target;
        _icon.sprite = target.GetComponentInChildren<SpriteRenderer>().sprite;
    }

    public void UpdateIndicator(Camera cam, RectTransform canvasRect)
    {
        if (_target == null) return;

        var screenPos = cam.WorldToScreenPoint(_target.position);

        var isVisible = screenPos.z > 0 &&
                         screenPos.x >= 0 && screenPos.x <= Screen.width &&
                         screenPos.y >= 0 && screenPos.y <= Screen.height;

        gameObject.SetActive(!isVisible);

        if (!isVisible)
        {
            var clampedPos = screenPos;
            var margin = 100f;

            clampedPos.x = Mathf.Clamp(clampedPos.x, margin, Screen.width - margin);
            clampedPos.y = Mathf.Clamp(clampedPos.y, margin, Screen.height - margin);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                clampedPos,
                null,
                out var anchoredPos
            );

            _rect.anchoredPosition = anchoredPos;
            
            var isLeft = screenPos.x <= margin;
            var isRight = screenPos.x >= Screen.width - margin;
            var isTop = screenPos.y >= Screen.height - margin;
            var isBottom = screenPos.y <= margin;

            if (isTop)
                _rect.localRotation = Quaternion.Euler(0, 0, 180);  
            else if (isBottom)
                _rect.localRotation = Quaternion.Euler(0, 0, 0);  
            else if (isLeft)
                _rect.localRotation = Quaternion.Euler(0, 0, -90); 
            else if (isRight)
                _rect.localRotation = Quaternion.Euler(0, 0, 90); 
            else
                _rect.localRotation = Quaternion.identity;    
        }
    }
}
