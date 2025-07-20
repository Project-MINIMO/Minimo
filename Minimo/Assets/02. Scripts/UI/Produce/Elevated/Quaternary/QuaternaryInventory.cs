using System.Linq;
using System.Collections.Generic;

using TMPro;

public class QuaternaryInventory : Inventory<Item>
{
    protected override void SetString()
    {
        var titleData = App.GetData<TitleData>();
        _menuTogs[0].GetComponentInChildren<TextMeshProUGUI>().text = titleData.GetString("STR_STORAGE_UI_TAB3_NAME");
        _menuTogs[1].GetComponentInChildren<TextMeshProUGUI>().text = titleData.GetString("STR_STORAGE_UI_TAB2_NAME");
    }

    protected override List<Item> GetFilteredItems() => AccountInfo.Instance.Items.Values
                                                            .Where(x => (x.Type == ItemType.Food && x.Level == 3)
                                                            || (x.Type == ItemType.Flower && x.Level == 2))
                                                            .ToList();

    protected override bool IsSlotFiltered(int index, Item item) => (index + 1) % 2 == (int)item.Type;
}
