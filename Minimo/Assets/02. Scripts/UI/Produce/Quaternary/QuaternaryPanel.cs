using System.Linq;

using UnityEngine;
using UnityEngine.UI;

public class QuaternaryPanel : ElevatedPanel
{
    [SerializeField] private Button[] _selectBtns;
    [SerializeField] private ItemInfoUpdater[] _selectedInfos;

    [SerializeField] private QuaternaryInventory _inventory;

    private readonly Item[] _selectedItems = new Item[2];
    
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        for (var i = 0; i < _selectBtns.Length; i++)
        {
            var index = i;
            _selectBtns[index].onClick.AddListener(() => _inventory.Show(index));
        }

        var slots = _inventory.transform.GetComponentsInChildren<WishOptionBtn>(true);
        for (var i = 0; i < slots.Length; i++)
        {
            var index = i;
            slots[index].GetComponent<Button>()
                .onClick
                .AddListener(() => SetItemOnSlot(slots[index].Item));
        }
    }
    
    public void SetItemOnSlot(Item selectedItem)
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