using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoragePanel : UIBase
{
    [SerializeField] private TextMeshProUGUI _titleTMP;
    [SerializeField] private StorageInfoCtrl _infoCtrl;
    
    [SerializeField] private CapacityHandler _capacityHandler;
    [SerializeField] private TextMeshProUGUI _capacityTMP;
    
    [SerializeField] private Button _openBtn;
    [SerializeField] private Button _closeBtn;
    [SerializeField] private Button _capacityBtn;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        _openBtn.onClick.AddListener(OpenPanel);
        _closeBtn.onClick.AddListener(ClosePanel);
        _capacityBtn.onClick.AddListener(_capacityHandler.Initialize);
        
        var slots = GetComponentsInChildren<InventorySlot>(true);
        foreach (var slot in slots)
        {
            slot.OnItemSelected += OnItemSelected;
        }

        _titleTMP.text = App.GetData<TitleData>().GetString("STR_STORAGE_UI_NAME");

        AccountInfo.Instance.OnCapacityChanged += SetCapacity;
        SetCapacity(AccountInfo.Instance.Capacity);
    }

    public override void OpenPanel()
    {
        base.OpenPanel();
        
        _infoCtrl.gameObject.SetActive(false);
        _capacityHandler.gameObject.SetActive(false);
    }

    private void OnItemSelected(InventorySlot slot)
    {
        _infoCtrl.Show(slot);
    }

    private void SetCapacity(int amount)
    {
        _capacityTMP.SetText($"{AccountInfo.Instance.CurrentItemCounts}/{amount}");
    }
}
