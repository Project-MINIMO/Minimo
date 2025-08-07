using UnityEngine;
using UnityEngine.UI;

public class StrayIndicatorHandler : IndicatorHandler
{
    [SerializeField] private Image _strayIcon;
    
    protected override void CheckTarget()
    {
        if (!Target.gameObject.activeSelf)
        {
            Target = null;
            gameObject.SetActive(false);
        }
    }
    
    protected override void CalculatePosition()
    {
        base.CalculatePosition();
        
        _strayIcon.rectTransform.rotation = Quaternion.identity;
    }
}
