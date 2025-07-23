using UnityEngine;
using TMPro;

public class MinimoUnassignWindow : PopUpWindow
{
    public override PopUpType Type() => PopUpType.MinimoUnassign;
    
    [SerializeField] private TextMeshProUGUI _titleTMP;
    [SerializeField] private TextMeshProUGUI _descriptionTMP;

    private string _titleString;
    
    private ProduceAdvanced _currentProduce;
    
    protected override void Awake()
    {
        base.Awake();

        _titleString = App.GetData<TitleData>().GetString("STR_POPUP_DISPLACE_DESC1");
        _descriptionTMP.text = App.GetData<TitleData>().GetString("STR_POPUP_DISPLACE_DESC2");

        AssignBtn.GetComponentInChildren<TextMeshProUGUI>().text = App.GetData<TitleData>().GetString("STR_BUTTON_CONFIRM");
    }

    public override void Show(ProduceAdvanced building, Minimo minimo)
    {
        base.Show();

        _titleTMP.text = string.Format(_titleString, building.BuildingData.Name, minimo.Name);
        
        _currentProduce = building;
    }

    protected override void Assign()
    {
        _currentProduce.UnplaceMinimo();
        
        base.Assign();
    }
}
