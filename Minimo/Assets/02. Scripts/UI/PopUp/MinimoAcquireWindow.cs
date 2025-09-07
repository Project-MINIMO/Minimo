using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MinimoAcquireWindow : PopUpWindow
{
    [SerializeField] private Image _minimoImg;
    [SerializeField] private TextMeshProUGUI _costTMP;
    
    public override PopUpType Type() => PopUpType.MinimoAcquire;
    
    private MinimoAcquireHandler _currentMinimo;
    private const string CostString = "{0} 사용";

    public override void Show(MinimoAcquireHandler handler)
    {
        base.Show();
        
        _currentMinimo = handler;
        _minimoImg.sprite = handler.GetComponentInChildren<SpriteRenderer>().sprite;
        _costTMP.text = string.Format(CostString, handler.RequiredCurreny);
    }

    protected override void Assign()
    {
        _currentMinimo.Acquire();
        
        base.Assign();
    }
}
