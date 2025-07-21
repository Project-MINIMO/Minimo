using System.Linq;
using System.Collections.Generic;

using TMPro;

public class ItemInventory : Inventory<Item>
{
    private void OnEnable()
    {
        _menuTogs[0].isOn = true;
    }

    protected override void SetString()
    {
        var titleData = App.GetData<TitleData>();
        _menuTogs[0].GetComponentInChildren<TextMeshProUGUI>().text = titleData.GetString("STR_STORAGE_UI_TAB1_NAME");
        _menuTogs[1].GetComponentInChildren<TextMeshProUGUI>().text = titleData.GetString("STR_STORAGE_UI_TAB2_NAME");
        _menuTogs[2].GetComponentInChildren<TextMeshProUGUI>().text = titleData.GetString("STR_STORAGE_UI_TAB3_NAME");
        _menuTogs[3].GetComponentInChildren<TextMeshProUGUI>().text = titleData.GetString("STR_STORAGE_UI_TAB4_NAME");
        _menuTogs[4].GetComponentInChildren<TextMeshProUGUI>().text = titleData.GetString("STR_STORAGE_UI_TAB5_NAME");
    }

    protected override List<Item> GetFilteredItems() => AccountInfo.Instance.Items.Values.Where(item => item.Level != 0).ToList();

    protected override bool IsSlotFiltered(int index, Item item) => index == 0 || index == (int)item.Type + 1;
}
