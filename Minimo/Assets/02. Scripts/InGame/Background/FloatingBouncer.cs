using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class FloatingBouncer : InteractObject
{
    [SerializeField] private float _floatHeight = 0.1f; 
    [SerializeField] private float _floatSpeed = 2f;   
    
    [SerializeField] private float _pressOffsetY = 0.15f;
    [SerializeField] private float _pressDuration = 0.5f; 
    [SerializeField] private float _bounceBackDuration = 0.15f;

    [SerializeField] private Transform _parentTransform;

    private float _baseY;
    private bool _isPressed;
    private bool _isLongPress;
    private Tweener _floatTween;

    private void Start()
    {
        _baseY = _parentTransform.localPosition.y;
        StartFloating();
    }

    private void StartFloating()
    {
        _parentTransform.DOLocalMoveY(_baseY + _floatHeight, _floatSpeed)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0) && _isLongPress)
        {
            _isLongPress = false;
            OnClickUp();
        }
    }

    public override void OnClickDown()
    {
        if (_isPressed) return;
        _isPressed = true;

        _parentTransform.DOKill();

        _parentTransform.DOLocalMoveY(_baseY - _pressOffsetY, _pressDuration).SetEase(Ease.OutQuad);
    }

    public override void OnDrag()
    {
        if (!_isPressed) return;
        _isPressed = false;
        _isLongPress = false;
        
        _parentTransform.DOKill();
        
        _parentTransform.DOLocalMoveY(_baseY, _bounceBackDuration).SetEase(Ease.OutBack)
            .OnComplete(StartFloating);
    }

    public override void OnLongPress()
    {
        _isLongPress = true;
    }

    public override void OnClickUp()
    {
        if (!_isPressed) return;
        _isPressed = false;
        
        _parentTransform.DOKill();
        
        _parentTransform.DOLocalMoveY(_baseY, _bounceBackDuration).SetEase(Ease.OutBack)
            .OnComplete(StartFloating);
    }
}
