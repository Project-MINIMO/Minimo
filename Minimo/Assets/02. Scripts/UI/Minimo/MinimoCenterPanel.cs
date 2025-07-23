using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManageMinimoPanel : UIBase
{
    public override bool IsUseBlur => true;
            
    [SerializeField] private TextMeshProUGUI _titleTMP;
    [SerializeField] private ExpandCtrl _expandCtrl;
    [SerializeField] private TextMeshProUGUI _capacityTMP;
    
    [SerializeField] private Button _openBtn;
    [SerializeField] private Button _closeBtn;
    [SerializeField] private Button _assignBtn;
    
    private MinimoProfilePanel _profilePanel;
    private MinimoAssignedPanel _assignedPanel;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        _profilePanel = manager.GetPanel<MinimoProfilePanel>();
        _assignedPanel = manager.GetPanel<MinimoAssignedPanel>();
        
        _openBtn.onClick.AddListener(OpenPanel);
        _closeBtn.onClick.AddListener(ClosePanel);
        _assignBtn.onClick.AddListener(_assignedPanel.OpenPanel);
        
        _expandCtrl.Initialize("STR_MINIMOCENTER_NAME",
            "STR_MC_RESIDENCEEXPAND_DESC",
            "STR_MC_EXPANDSUCCEED_DESC");
        
        var slots = GetComponentsInChildren<MinimoSlot>(true);
        foreach (var slot in slots)
        {
            slot.OnItemSelected += OnItemSelected;
        }

        _titleTMP.text = App.GetData<TitleData>().GetString("STR_MINIMOCENTER_NAME");

        AccountInfo.Instance.OnMinimoCapacityChanged += OnMinimoCapacityChanged;
        OnMinimoCapacityChanged(AccountInfo.Instance.MinimoCapacity);
    }

    public override void OpenPanel()
    {
        base.OpenPanel();
        
        _expandCtrl.gameObject.SetActive(false);
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