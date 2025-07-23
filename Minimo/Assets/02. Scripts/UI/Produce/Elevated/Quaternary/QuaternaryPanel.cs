using System.Linq;

using UnityEngine;
using UnityEngine.UI;

public class QuaternaryPanel : ElevatedPanel
{
    [SerializeField] private Button[] _selectBtns;
    [SerializeField] private Toggle[] _inventoryTogs;
    [SerializeField] private QuaternaryClearDialog _clearDialog;
    [SerializeField] private QuaternaryTransitioner _transitioner;
    
    [SerializeField] private ItemInfoUpdater[] _selectedInfos;
    private readonly Item[] _selectedItems = new Item[2];
    
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        for (var i = 0; i < _selectBtns.Length; i++)
        {
            var index = i;
            _selectBtns[index].onClick.AddListener(() => OnClickSelectBtn(index));
        }

        var slots = GetComponentsInChildren<ItemSlot>(true);
        foreach (var slot in slots)
        {
            slot.OnItemSelected += OnItemSelected;
        }
    }

    public override void OpenPanel()
    {
        base.OpenPanel();

        ClearItems();
        _produceObject.OnMaxSlotCountChanged += CheckCanPlant;
        _produceObject.OnMinimoAssigned += OnMinimoAssigned;
    }

    public override void ClosePanel()
    {
        if (_produceObject != null)
        {
            _produceObject.OnMaxSlotCountChanged -= CheckCanPlant;
            _produceObject.OnMinimoAssigned -= OnMinimoAssigned;
            _produceObject = null;
        }
        
        base.ClosePanel();
    }

    private void OnClickSelectBtn(int index)
    {
        if (_selectedItems[index] != null)
        {
            _clearDialog.Show(() =>
            {
                ClearItem(index);
            });
        }
        else
        {
            _transitioner.Open();
            _inventoryTogs[index].isOn = true;
        }
    }
    
    private void OnItemSelected(InventorySlot<Item> slot)
    {
        _selectedInfos[(int)slot.Item.Type].UpdateItem(slot.Item);
        _selectedItems[(int)slot.Item.Type] = slot.Item;

        CheckCanPlant();
    }
    
    private void OnMinimoAssigned(Minimo _)
    {
        CheckCanPlant();
    }

    private void CheckCanPlant()
    {
        if (_selectedItems.All(item => item != null))
        {
            _produceManager.RequestPlant(_selectedItems,
                onSuccess: ClearItems,
                onFailed: App.Notification);
        }
    }

    private void ClearItems()
    {
        for (var i = 0; i < _selectedItems.Length; i++)
        {
            ClearItem(i);
        }
    }

    private void ClearItem(int index)
    {
        _selectedItems[index] = null;
        _selectedInfos[index].ClearItem();
    }
}