using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class OrderPanel : UIBase
{
    [SerializeField] private Button _openBtn;
    [SerializeField] private Button _closeBtn;
    [SerializeField] private GameObject _inventoryBack;
    [SerializeField] private OrderBack _orderBack;
    
    private OrderSlot _selectedSlot;
    
    private void Start()
    {
        var slots = GetComponentsInChildren<OrderItemSlot>(true);
        foreach (var slot in slots)
        {
            slot.OnItemSelected += OnItemSelected;
        }
        
        var slots2 = GetComponentsInChildren<OrderSlot>(true);
        foreach (var slot in slots2)
        {
            slot.OnSlotSelected += OnSlotSelected;
        }
    }

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        _openBtn.onClick.AddListener(OpenPanel);
        _closeBtn.onClick.AddListener(ClosePanel);
    }

    public override void OpenPanel()
    {
        base.OpenPanel();
        
        _inventoryBack.SetActive(false);
        _orderBack.gameObject.SetActive(false);
    }

    private void OnItemSelected(InventorySlot<Item> slot)
    {
        if (_selectedSlot == null) return;
        if (slot.Item == null) return;
        if (OrderManager.Instance.OrderItems.Any(x => x.item != slot.Item)) return;
        
        _orderBack.Initialize(slot.Item, _selectedSlot);
    }
    
    private void OnSlotSelected(OrderSlot slot)
    {
        _selectedSlot = slot;
        
        if (slot.Item != null)
        {
            _orderBack.Initialize(slot);
            _inventoryBack.SetActive(false);
        }
        else
        {
            _inventoryBack.SetActive(true);
        }
    }
}
