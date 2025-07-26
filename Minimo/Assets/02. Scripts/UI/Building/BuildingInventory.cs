using System.Linq;
using System.Collections.Generic;

using TMPro;

public class BuildingInventory : Inventory<Building>
{
    protected override void SetString()
    {
        var titleData = App.GetData<TitleData>();
        _menuTogs[0].GetComponentInChildren<TextMeshProUGUI>().text = titleData.GetString("STR_BUILDING_UI_PRODUCTION");
        _menuTogs[1].GetComponentInChildren<TextMeshProUGUI>().text = titleData.GetString("STR_BUILDING_UI_DECORATION");
        _menuTogs[2].GetComponentInChildren<TextMeshProUGUI>().text = titleData.GetString("STR_BUILDING_UI_UTILITY");
        _menuTogs[3].GetComponentInChildren<TextMeshProUGUI>().text = titleData.GetString("STR_BUILDING_UI_MINIMO");
    }

    protected override List<Building> GetFilteredItems() => App.GetData<TitleData>().Building.Values.ToList();

    protected override bool IsSlotFiltered(int index, Building item) => index == 0
                                                                        ? item.Type is BuildingType.Tier1 or BuildingType.Tier2 or BuildingType.Tier3 or BuildingType.Tier4
                                                                        : item.Type == (BuildingType)index;
}
