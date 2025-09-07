using System.Linq;
using System.Collections.Generic;

using UnityEngine;

public class IndicatorPanel : UIBase
{
    public override bool IsDefaultPanel => true;
    
    [SerializeField] private RectTransform _canvasRect;
    [SerializeField] private IndicatorHandler _indicatorPrefab;
    [SerializeField] private StrayIndicatorHandler _strayIndicatorPrefab;
    [SerializeField] private Transform _indicatorParent;
    
    private readonly List<IndicatorHandler> _indicators = new();

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        var indicators = GetComponentsInChildren<IndicatorHandler>(true);
        foreach (var indicator in indicators)
        {
            _indicators.Add(indicator);
            indicator.gameObject.SetActive(false);
        }
    }

    public void CreateIndicator(Transform target)
    {
        var indicator = _indicators.FirstOrDefault(x => x.CanUse);

        if (indicator == null)
        {
            indicator = Instantiate(_indicatorPrefab, _indicatorParent);
            _indicators.Add(indicator);
        }
        
        indicator.Initialize(target, _canvasRect);
    }

    public void CreateStrayIndicator(Transform target)
    {
        var indicator = _indicators
            .Where(x => x is StrayIndicatorHandler)
            .FirstOrDefault(x => x.CanUse);
        
        if (indicator == null)
        {
            indicator = Instantiate(_strayIndicatorPrefab, _indicatorParent);
            _indicators.Add(indicator);
        }
        
        indicator.Initialize(target, _canvasRect);
    }
}
