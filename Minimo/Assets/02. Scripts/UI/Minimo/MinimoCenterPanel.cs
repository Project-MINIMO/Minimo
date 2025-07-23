using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManageMinimoPanel : UIBase
{
    public override bool IsUseBlur => true;
            
    [SerializeField] private TextMeshProUGUI _titleTMP;
    [SerializeField] private TextMeshProUGUI _capacityTMP;
    
    [SerializeField] private Button _openBtn;
    [SerializeField] private Button _closeBtn;
    [SerializeField] private Button _assignBtn;
    [SerializeField] private Button _expandBtn;
    
    private MinimoProfilePanel _profilePanel;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        _profilePanel = manager.GetPanel<MinimoProfilePanel>();
        var assignedPanel = manager.GetPanel<MinimoAssignedPanel>();
        var popUpPanel = manager.GetPanel<PopUpPanel>();
        
        _openBtn.onClick.AddListener(OpenPanel);
        _closeBtn.onClick.AddListener(ClosePanel);
        _assignBtn.onClick.AddListener(assignedPanel.OpenPanel);
        _expandBtn.onClick.AddListener(() => popUpPanel.OpenPanel(PopUpType.MinimoExpand));
        
        var slots = GetComponentsInChildren<MinimoSlot>(true);
        foreach (var slot in slots)
        {
            slot.OnItemSelected += OnItemSelected;
        }

        _titleTMP.text = App.GetData<TitleData>().GetString("STR_MINIMOCENTER_NAME");

        AccountInfo.Instance.OnMinimoCapacityChanged += OnMinimoCapacityChanged;
        OnMinimoCapacityChanged(AccountInfo.Instance.MinimoCapacity);
    }

    private void OnItemSelected(InventorySlot<Minimo> slot)
    {
        _profilePanel.OpenPanel(slot.Item);
    }

    private void OnMinimoCapacityChanged(int amount)
    {
        _capacityTMP.SetText($"{20}/{amount}");
    }
}