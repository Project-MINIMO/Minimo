using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StrayIndicatorHandler : IndicatorHandler
{
    [SerializeField] private Image _strayIcon;
    
    private readonly Vector3 _shrinkScale = new(0.8f, 0.8f);
    
    public override void Initialize(Transform target, RectTransform canvasRect)
    {
        base.Initialize(target, canvasRect);

        _strayIcon.rectTransform.DOScale(_shrinkScale, 0.5f).SetEase(Ease.InCubic).SetLoops(-1 ,LoopType.Yoyo);
    }
    
    protected override void CheckTarget()
    {
        if (!Target.gameObject.activeSelf)
        {
            Target = null;
            _strayIcon.DOKill();
            gameObject.SetActive(false);
        }
    }
    
    protected override void CalculatePosition()
    {
        base.CalculatePosition();
        
        _strayIcon.rectTransform.rotation = Quaternion.identity;
    }
}
