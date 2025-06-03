using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

public enum ItemType
{
    None,
    Food,
    Flower,
    Amulet,
}

[Serializable]
public class QuestData
{
    public int ID;
    public string Name;
    public int Type;
    public int Title;
    public int PreQuestID;
    public int OpenLevel;
}

[Serializable]
public class CommonData
{
    public string ID;
    public int Value;
}

[Serializable]
public class BuildingData
{
    public int ID;
    public int Type;
    public int UnlockLevel;
    public string Name;
    public int Cost;
    public int Duration;
}

[Serializable]
public class ItemData
{
    public int ID;
    public int Type;
    public int Level;
    public int SellCost;
    public int BuyCost;
    public string Name;
}

[Serializable]
public class RawProduceData
{
    public int    ID;        // 예: 빌딩 enum 값을 int로
    public string Building;  // 예: "StarCropFarm"
    public string MaterialItems; // "StarCrop:1,Water:5"
    public string ResultItems;   // "Sugar:2"
    public int    Time;      
    public int    EXP;
}

[Serializable]
public class ProduceData
{
    public int ID;
    public string Building;
    public ProduceMaterial[] MaterialItems;
    public ProduceResult[] ResultItems;
    public int Time;
    public int EXP;
}

[Serializable]
public class ProduceMaterial
{
    public int ID;
    public int Amount;
}

[Serializable]
public class ProduceResult
{
    public int ID;
    public int Amount;
}

[Serializable]
public class StringData
{
    public string ID;
    public string Korean;
    public string English;
    public string Chinese;
    public string Japanese;
}

public class TitleData : DataBase
{
    public Dictionary<int, QuestData> Quest { get; private set; } = new();
    public Dictionary<string, int> Common { get; private set; } = new();
    public Dictionary<int, BuildingData> Building { get; private set; } = new();
    public Dictionary<int, ItemData> Item { get; private set; } = new();
    public Dictionary<int, ProduceData> Produce { get; private set; } = new();
    public Dictionary<string, List<ProduceData>> GroupedProduce { get; private set; } = new();

    private Dictionary<string, StringData> _string = new();

    private bool _isGameDataLoaded = false;

    #region Data Path
    private const string STRING_PATH = "Data/StringData";
    private const string QUEST_PATH = "Data/QuestData";
    private const string COMMON_PATH = "Data/CommonData";
    private const string BUILDING_PATH = "Data/BuildingData";
    private const string ITEM_PATH = "Data/ItemData";
    private const string PRODUCE_PATH = "Data/ProduceData";
    #endregion

    protected override void Awake()
    {
        base.Awake();

        LoadData();
    }

    private void LoadData()
    {
        if (_isGameDataLoaded)
        {
            return;
        }

        _string.Clear();
        Quest.Clear();
        Common.Clear();
        Building.Clear();
        Item.Clear();
        Produce.Clear();

        var stringDataRaw = DataLoader.LoadData<StringData>(STRING_PATH);
        foreach (var data in stringDataRaw)
        {
            _string.Add(data.ID, data);
        }
        
        var questDataRaw = DataLoader.LoadData<QuestData>(QUEST_PATH);
        foreach (var data in questDataRaw)
        {
            Quest.Add(data.ID, data);
        }
        
        var commonDataRaw = DataLoader.LoadData<CommonData>(COMMON_PATH);
        foreach (var data in commonDataRaw)
        {
            Common.Add(data.ID, data.Value);
        }
        
        var buildingDataRaw = DataLoader.LoadData<BuildingData>(BUILDING_PATH);
        foreach (var data in buildingDataRaw)
        {
            Building.Add(data.ID, data);
        } 
        
        var itemDataRaw = DataLoader.LoadData<ItemData>(ITEM_PATH);
        foreach (var data in itemDataRaw)
        {
            Item.Add(data.ID, data);
        }
        
        var produceDataRaw = DataLoader.LoadDataProduceData(PRODUCE_PATH);
        foreach (var data in produceDataRaw)
        {
            Produce.Add(data.ID, data);   
        }
        GroupedProduce = Produce
            .Values
            .GroupBy(data => data.Building)
            .ToDictionary(data => data.Key, data => data.ToList());
       
        _isGameDataLoaded = true;
    }

    #region StringData
    public string GetString(string _code)
    {
        TryGetString(_code, out var str);
        return str;
    }

    public string GetFormatString(string _code, params string[] _args)
    {
        TryGetString(_code, out var str);

        try { return string.Format(str, _args); }
        catch (Exception error)
        { Debug.LogError($"Failed to format string {_code}. {error}"); }

        return _code;
    }

    private bool TryGetString(string _code, out string str)
    {
        return TryGetString(_code, App.GetData<SettingData>().Language, out str);
    }

    private bool TryGetString(string _code, SystemLanguage _language, out string str)
    {
        if (_code != null && _string.TryGetValue(_code, out var data))
        {
            str = _language switch
            {
                SystemLanguage.Korean => data.Korean,
                SystemLanguage.Chinese => data.Chinese,
                SystemLanguage.Japanese => data.Japanese,
                _ => data.English
            };

            return true;
        }

        str = _code;
        return false;
    }
    #endregion
}
