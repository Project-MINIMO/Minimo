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

        var slots = GetComponentsInChildren<InventorySlot>(true);
        foreach (var slot in slots)
        {
            slot.OnItemSelected += OnItemSelected;
        }
    }

    public override void OpenPanel()
    {
        base.OpenPanel();

        ClearItems();
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
    
    private void OnItemSelected(Item selectedItem)
    {
        _selectedInfos[selectedItem.Data.Type - 1].UpdateItem(selectedItem, 1);
        _selectedItems[selectedItem.Data.Type - 1] = selectedItem;

        if (CheckCanPlant())
        {
            ((ProduceQuaternary)_produceObject).StartPlant(_selectedItems);

            ClearItems();
        }
    }

    private bool CheckCanPlant()
    {
        return _selectedItems.All(item => item != null);
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
        _selectedInfos[index].UpdateItem(-1, -1);
    }
}