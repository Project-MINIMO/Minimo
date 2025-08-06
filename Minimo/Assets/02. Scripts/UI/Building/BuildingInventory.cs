using System.Linq;
using System.Collections.Generic;

using TMPro;

public class BuildingInventory : Inventory<Building>
{
    protected override void SetString()
    {
        var titleData = App.GetData<TitleData>();
        _menuTogs[0].GetComponentInChildren<TextMeshProUGUI>().text = "전체";
        _menuTogs[1].GetComponentInChildren<TextMeshProUGUI>().text = "작물";
        _menuTogs[2].GetComponentInChildren<TextMeshProUGUI>().text = "가공물";
        _menuTogs[3].GetComponentInChildren<TextMeshProUGUI>().text = "식품";
        _menuTogs[3].GetComponentInChildren<TextMeshProUGUI>().text = "기물";
    }

    protected override List<Building> GetFilteredItems() => App.GetData<TitleData>().Building.Values.ToList();

    protected override bool IsSlotFiltered(int index, Building item) => index == 0 || item.Type == (BuildingType)(index - 1);
}
