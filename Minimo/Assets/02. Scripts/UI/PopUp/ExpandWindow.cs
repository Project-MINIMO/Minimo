using UnityEngine;
using TMPro;

public abstract class ExpandWindow : PopUpWindow
{
    [SerializeField] private TextMeshProUGUI _titleTMP;
    [SerializeField] private TextMeshProUGUI _descriptionTMP;
    
    [SerializeField] private CapacityHandler _capacityHandler;

    private PopUpPanel _popUpPanel;
    
    protected override void Awake()
    {
        base.Awake();
        
        var titleData = App.GetData<TitleData>();
        _titleTMP.text = GetTitle(titleData);
        _descriptionTMP.text = GetDescription(titleData);

        _popUpPanel = App.GetManager<UIManager>().GetPanel<PopUpPanel>();
    }
    
    protected abstract string GetTitle(TitleData title);
    protected abstract string GetDescription(TitleData title);

    public override void Show()
    {
        base.Show();
        
        _capacityHandler.Initialize(Complete);
    }

    private void Complete()
    {
        _popUpPanel.OpenPanel(GetPopUpType());
    }
    
    protected abstract PopUpType GetPopUpType();
}
