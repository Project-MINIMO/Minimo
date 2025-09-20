using System.Collections.Generic;
using System.Linq;

public class TradeInventory : Inventory<Item>
{
    private void OnEnable()
    {
        foreach (var slot in Slots)
        {
            slot.gameObject.SetActive(true);
        }
    }

    protected override void SetString() { }
    protected override List<Item> GetFilteredItems() => AccountInfo.Instance.Items.Values.Where(item => item.Level != 0).ToList();
    protected override bool IsSlotFiltered(int index, Item item) => true;
}
