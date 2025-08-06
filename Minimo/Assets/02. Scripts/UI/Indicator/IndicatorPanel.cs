using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IndicatorPanel : UIBase
{
    public override bool IsDefaultPanel => true;
    
    [SerializeField] private RectTransform _canvasRect;
    [SerializeField] private IndicatorHandler _indicatorPrefab;
    [SerializeField] private Transform _indicatorParent;

    private readonly Queue<IndicatorHandler> _indicatorPool = new();
    private readonly List<IndicatorHandler> _activeIndicators = new();

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        var indicators = GetComponentsInChildren<IndicatorHandler>(true);
        foreach (var indicator in indicators)
        {
            _indicatorPool.Enqueue(indicator);
        }
    }
    
    private void LateUpdate()
    {
        var toRemove = new List<IndicatorHandler>();
        
        foreach (var indicator in _activeIndicators)
        {
            if (!indicator.gameObject.activeSelf)
            {
                toRemove.Add(indicator);
                _indicatorPool.Enqueue(indicator);
                continue;
            }
            
            indicator.UpdateIndicator(Camera.main, _canvasRect);
        }
        
        foreach (var removeTarget in toRemove)
        {
            _activeIndicators.Remove(removeTarget);
        }
    }

    public void CreateIndicator(Transform target)
    {
        IndicatorHandler indicator;

        if (_indicatorPool.Count > 0)
        {
            indicator = _indicatorPool.Dequeue();
        }
        else
        {
            indicator = Instantiate(_indicatorPrefab, _indicatorParent);
        }
        
        indicator.Initialize(target);
        _activeIndicators.Add(indicator);
    }
}
