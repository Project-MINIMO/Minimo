using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MinimoPlacePanel : UIBase
{
    public override bool IsUseBlur => true;
            
    [SerializeField] private TextMeshProUGUI _titleTMP;
    [SerializeField] private TextMeshProUGUI _descriptionTMP;

    [SerializeField] private Image _buildingImg;
    [SerializeField] private Button _closeBtn;
    
    private PopUpPanel _popUpPanel;
    private ProduceAdvanced _currentProduce;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        _popUpPanel = manager.GetPanel<PopUpPanel>();
        
        _closeBtn.onClick.AddListener(ClosePanel);

        var slots = GetComponentsInChildren<MinimoSlot>(true);
        foreach (var slot in slots)
        {
            slot.OnItemSelected += OnItemSelected;
        }

        _titleTMP.text = App.GetData<TitleData>().GetString("STR_POPUP_PLACEBUILDING_NAME");
        _descriptionTMP.text = App.GetData<TitleData>().GetString("STR_POPUP_PLACEBUILDING_DESC");
    }
    
    public void OpenPanel(ProduceAdvanced building)
    {
        OpenPanel();
        
        _currentProduce = building;
        _buildingImg.sprite = building.BuildingData.Icon;
    }
    
    private void OnItemSelected(InventorySlot<Minimo> slot)
    {
        var type = _currentProduce.AssignedMinimo switch
        {
            null => PopUpType.MinimoAssign,
            var assigned when assigned == slot.Item => PopUpType.MinimoUnassign,
            _ => PopUpType.MinimoShift
        };

        _popUpPanel.OpenPanel(type, _currentProduce, slot.Item);
    }
}
