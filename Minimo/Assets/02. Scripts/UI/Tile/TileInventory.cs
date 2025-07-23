using System.Collections.Generic;
using System.Linq;
using TMPro;

public class TileInventory : Inventory<CustomTile>
{
    protected override void SetString()
    {
        var titleData = App.GetData<TitleData>();
        _menuTogs[0].GetComponentInChildren<TextMeshProUGUI>().text = titleData.GetString("STR_MC_FILTER_COMPONENT1_NAME");
        _menuTogs[1].GetComponentInChildren<TextMeshProUGUI>().text = titleData.GetString("STR_TILEMANAGE_TAB2_NAME");
        _menuTogs[2].GetComponentInChildren<TextMeshProUGUI>().text = titleData.GetString("STR_TILEMANAGE_TAB2_NAME");
        _menuTogs[3].GetComponentInChildren<TextMeshProUGUI>().text = titleData.GetString("STR_TILEMANAGE_TAB3_NAME");
    }

    protected override List<CustomTile> GetFilteredItems() => App.GetData<TitleData>().CustomTile.Values.ToList();

    protected override bool IsSlotFiltered(int index, CustomTile item) => index == 0 || index == (int)item.Type + 1;
}
