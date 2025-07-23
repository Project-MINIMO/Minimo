using UnityEngine;
using TMPro;

public class MinimoAssignWindow : PopUpWindow
{
    public override PopUpType Type() => PopUpType.MinimoAssign;
    
    [SerializeField] private MinimoInfoUpdater _infoUpdater;
    [SerializeField] private TextMeshProUGUI _descriptionTMP;

    private ProduceAdvanced _currentProduce;
    private Minimo _currentMinimo;
    
    protected override void Awake()
    {
        base.Awake();

        _descriptionTMP.text = App.GetData<TitleData>().GetString("STR_POPUP_PLACE_DESC1");
        AssignBtn.GetComponentInChildren<TextMeshProUGUI>().text = App.GetData<TitleData>().GetString("STR_BUTTON_CONFIRM");
    }

    public override void Show(ProduceAdvanced building, Minimo minimo)
    {
        base.Show();

        _currentProduce = building;
        _currentMinimo = minimo;
        
        _infoUpdater.UpdateInfo(minimo);
    }

    protected override void Assign()
    {
        _currentProduce.PlaceMinimo(_currentMinimo);
        
        base.Assign();
    }
}
