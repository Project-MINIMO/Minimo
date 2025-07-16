using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class QuaternaryPanel : ElevatedPanel
{
    [SerializeField] private Button[] _selectBtns;
    [SerializeField] private ItemInfoUpdater[] _selectedInfos;
    [SerializeField] private Toggle[] _inventoryTogs;
    [SerializeField] private QuaternaryTransitioner _transitioner;

    private readonly Item[] _selectedItems = new Item[2];
    
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        for (var i = 0; i < _selectBtns.Length; i++)
        {
            var index = i;
            _selectBtns[index].onClick.AddListener(() =>
            {
                _transitioner.Open();
                _inventoryTogs[index].isOn = true;
            });
        }

        var slots = GetComponentsInChildren<InventorySlot>(true);
        foreach (var slot in slots)
        {
            slot.OnItemSelected += SetItemOnSlot;
        }
    }
    
    private void SetItemOnSlot(Item selectedItem)
    {
        _selectedInfos[selectedItem.Data.Type - 1].UpdateItem(selectedItem, 1);
        _selectedItems[selectedItem.Data.Type - 1] = selectedItem;

        if (CheckCanPlant())
        {
            ((ProduceQuaternary)_produceObject).StartPlant(_selectedItems);

            foreach (var info in _selectedInfos)
            {
                info.UpdateItem(-1, -1);
            }

            for (var i = 0; i < _selectedItems.Length; i++)
            {
                _selectedItems[i] = null;
            }
        }
    }

    private bool CheckCanPlant()
    {
        return _selectedItems.All(item => item != null);
    }
}