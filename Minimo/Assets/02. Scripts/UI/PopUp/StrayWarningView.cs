using DG.Tweening;
using UnityEngine;

public class StrayWarningView : PopUpWindow
{
    public override PopUpType Type() => PopUpType.StrayWarning;

    [SerializeField] private RectTransform _warningRect;
    
    private RectTransform _rect;
    private float _completeTime;
    private bool _isCompleteExpand;

    private readonly Vector3 _shrinkScale = new(0.6f, 0.6f);

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
        
        _warningRect.DOScale(_shrinkScale, 0.2f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }
    
    public override void Hide()
    {
        _warningRect.DOKill();
        _warningRect.localScale = Vector2.one;
        
        base.Hide();
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
