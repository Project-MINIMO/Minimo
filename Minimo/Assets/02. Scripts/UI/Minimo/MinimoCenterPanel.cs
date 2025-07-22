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
    [SerializeField] private Button _capacityBtn;
    
    private MinimoProfilePanel _profilePanel;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        _profilePanel = manager.GetPanel<MinimoProfilePanel>();
        
        _openBtn.onClick.AddListener(OpenPanel);
        _closeBtn.onClick.AddListener(ClosePanel);
        _capacityBtn.onClick.AddListener(_expandCtrl.Show);
        
        _expandCtrl.Initialize("STR_MINIMOCENTER_NAME",
            "STR_MC_RESIDENCEEXPAND_DESC",
            "STR_MC_EXPANDSUCCEED_DESC");
        
        var slots = GetComponentsInChildren<MinimoSlot>(true);
        foreach (var slot in slots)
        {
            slot.OnItemSelected += OnItemSelected;
        }

        _titleTMP.text = App.GetData<TitleData>().GetString("STR_MINIMOCENTER_NAME");

        AccountInfo.Instance.OnMinimoCapacityChanged += SetMinimoCapacity;
        SetMinimoCapacity(AccountInfo.Instance.MinimoCapacity);
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

    private void SetMinimoCapacity(int amount)
    {
        _capacityTMP.SetText($"{20}/{amount}");
    }
}