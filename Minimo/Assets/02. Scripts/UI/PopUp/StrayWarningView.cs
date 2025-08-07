using DG.Tweening;
using UnityEngine;

public class StrayWarningView : PopUpWindow
{
    public override PopUpType Type() => PopUpType.StrayWarning;
    
    private RectTransform _rect;
    private float _completeTime;
    private bool _isCompleteExpand;

    protected override void Awake()
    {
        base.Awake();
        
        _rect = GetComponent<RectTransform>();
    }
    
    public override void Show()
    {
        base.Show();

        _completeTime = Time.time;
        _isCompleteExpand = false;
        _rect.localScale = Vector2.one;
    }

    private void Update()
    {
        if (!gameObject.activeInHierarchy) return;
        if (_isCompleteExpand) return;
        if (Time.time - _completeTime < 1f) return;

        _isCompleteExpand = true;
        _rect.DOScale(0, 0.1f).SetEase(Ease.OutCirc)
            .OnComplete(Assign);
    }
}
