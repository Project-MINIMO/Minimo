using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoragePanel : UIBase
{
    public override bool IsUseBlur => true;
    protected override bool IsUseGuide => true;
            
    [SerializeField] private TextMeshProUGUI _titleTMP;
    [SerializeField] private StorageInfoCtrl _infoCtrl;
    [SerializeField] private TextMeshProUGUI _capacityTMP;
    
    [SerializeField] private Button _openBtn;
    [SerializeField] private Button _closeBtn;
    [SerializeField] private Button _expandBtn;
    [SerializeField] private Button _infoBtn;
    [SerializeField] private MenuToggleGroup _toggleGroup;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        _openBtn.onClick.AddListener(OpenPanel);
        _closeBtn.onClick.AddListener(ClosePanel);

        var popUpPanel = manager.GetPanel<PopUpPanel>();
        _expandBtn.onClick.AddListener(() => popUpPanel.OpenPanel(PopUpType.StorageExpand));
        
        _infoBtn.onClick.AddListener(ShowGuide);

        _titleTMP.text = App.GetData<TitleData>().GetString("STR_STORAGE_UI_NAME");

        AccountInfo.Instance.OnStorageCapacityChanged += OnStorageCapacityChanged;
        OnStorageCapacityChanged(AccountInfo.Instance.StorageCapacity);
    }
    
    private void Start()
    {
        var slots = GetComponentsInChildren<ItemSlot>(true);
        foreach (var slot in slots)
        {
            slot.OnItemSelected += OnItemSelected;
        }
    }
    
    public override void Show(bool isNew)
    {
        base.Show(isNew);
        
        _toggleGroup.Show(isNew);
    }

    public override void OpenPanel()
    {
        base.OpenPanel();
        
        _infoCtrl.gameObject.SetActive(false);
    }

    private void OnItemSelected(InventorySlot<Item> slot)
    {
        _infoCtrl.Show(slot);
    }

    private void OnStorageCapacityChanged(int amount)
    {
        _capacityTMP.SetText($"{AccountInfo.Instance.CurrentItemCounts}/{amount}");
    }
}
