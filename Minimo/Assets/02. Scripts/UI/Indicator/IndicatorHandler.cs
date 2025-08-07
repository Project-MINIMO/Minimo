using UnityEngine;
using UnityEngine.UI;

public class IndicatorHandler : MonoBehaviour
{
    public bool CanUse => Target == null;
    
    [SerializeField] private Image _icon;
    [SerializeField] private CanvasGroup _canvasGroup;

    protected Transform Target;
    
    private Camera _camera;
    private RectTransform _canvasRect;
    private RectTransform _rect;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _camera = Camera.main;
    }

    public void Initialize(Transform target, RectTransform canvasRect)
    {
        Target = target;
        _canvasRect = canvasRect;
        _icon.sprite = target.GetComponentInChildren<SpriteRenderer>().sprite;
        CalculatePosition();
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (Target == null) return;

        CalculatePosition();
        CheckTarget();
    }

    protected virtual void CalculatePosition()
    {
        var screenPos = _camera.WorldToScreenPoint(Target.position);

        var isVisible = screenPos.z > 0 &&
                        screenPos.x >= 0 && screenPos.x <= Screen.width &&
                        screenPos.y >= 0 && screenPos.y <= Screen.height;

        _canvasGroup.alpha = isVisible ? 0 : 1;

        if (!isVisible)
        {
            var clampedPos = screenPos;
            var margin = 100f;

            clampedPos.x = Mathf.Clamp(clampedPos.x, margin, Screen.width - margin);
            clampedPos.y = Mathf.Clamp(clampedPos.y, margin, Screen.height - margin);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvasRect,
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
            
            _icon.rectTransform.rotation = Quaternion.identity;
        }
    }

    protected virtual void CheckTarget()
    {
        if (_canvasGroup.alpha == 0)
        {
            Target = null;
            gameObject.SetActive(false);
        }
    }
}
