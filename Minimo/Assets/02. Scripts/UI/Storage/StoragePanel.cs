using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoragePanel : UIBase
{
    public override bool IsUseBlur => true;
            
    [SerializeField] private TextMeshProUGUI _titleTMP;
    [SerializeField] private StorageInfoCtrl _infoCtrl;
    [SerializeField] private StorageExpandCtrl _expandCtrl;
    [SerializeField] private TextMeshProUGUI _capacityTMP;
    
    [SerializeField] private Button _openBtn;
    [SerializeField] private Button _closeBtn;
    [SerializeField] private Button _capacityBtn;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        _openBtn.onClick.AddListener(OpenPanel);
        _closeBtn.onClick.AddListener(ClosePanel);
        _capacityBtn.onClick.AddListener(_expandCtrl.Show);
        
        var slots = GetComponentsInChildren<ItemSlot>(true);
        foreach (var slot in slots)
        {
            slot.OnItemSelected += OnItemSelected;
        }

        _titleTMP.text = App.GetData<TitleData>().GetString("STR_STORAGE_UI_NAME");

        AccountInfo.Instance.OnStorageCapacityChanged += SetStorageCapacity;
        SetStorageCapacity(AccountInfo.Instance.StorageCapacity);
    }

    public override void OpenPanel()
    {
        base.OpenPanel();
        
        _infoCtrl.gameObject.SetActive(false);
        _expandCtrl.gameObject.SetActive(false);
    }

    private void OnItemSelected(InventorySlot<Item> slot)
    {
        _infoCtrl.Show(slot);
    }

    private void SetStorageCapacity(int amount)
    {
        _capacityTMP.SetText($"{AccountInfo.Instance.CurrentItemCounts}/{amount}");
    }
}
