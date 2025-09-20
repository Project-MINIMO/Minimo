using UnityEngine;
using UnityEngine.UI;

public class TradeMenu : MonoBehaviour
{
    [SerializeField] private Button _openBtn;
    [SerializeField] private Button _closeBtn;
    [SerializeField] private GameObject _inventoryBack;
    
    private TradeMenuSlot _selectedSlot;
    
    private void Awake()
    {
        var slots = GetComponentsInChildren<TradeItemSlot>(true);
        foreach (var slot in slots)
        {
            slot.OnItemSelected += OnItemSelected;
        }
        
        var slots2 = GetComponentsInChildren<TradeMenuSlot>(true);
        foreach (var slot in slots2)
        {
            slot.OnSlotSelected += OnSlotSelected;
        }
        
        _openBtn.onClick.AddListener(() =>
        {
            gameObject.SetActive(true);
        });
        _closeBtn.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
        
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _inventoryBack.SetActive(false);
    }
    
    private void OnItemSelected(InventorySlot<Item> slot)
    {
        if (_selectedSlot == null) return;
        if (slot.Item == null) return;
        
        _selectedSlot.SetItem(slot.Item);
        TradeManager.Instance.TradeItems.Add(slot.Item);
    }
    
    private void OnSlotSelected(TradeMenuSlot slot)
    {
        if (slot.Item != null)
        {
            TradeManager.Instance.TradeItems.Remove(slot.Item);
            slot.ClearItem();
            _inventoryBack.SetActive(false);
        }
        else
        {
            _selectedSlot = slot;
            _inventoryBack.SetActive(true);
        }
    }
}
