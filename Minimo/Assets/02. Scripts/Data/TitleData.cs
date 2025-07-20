using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;


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
    public int Property;
    public int SellCost;
    public int BuyCost;
    public string Name;
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

#region Quest
[Serializable]
public class QuestGroupData
{
    public int ID;
    public string Name;
    public int Title;
    public int PreQuestID;
    public int OpenLevel;
}

[Serializable]
public class QuestData
{
    public int ID;
    public int Type;
    public int PreQuestID;
    public int OpenLevel;
    public string Name;
    public int Condition;
    public string Clear;
    public string Reward;
}
#endregion

#region Produce
[Serializable]
public class RawProduceData
{
    public int    ID;       
    public string Building; 
    public string MaterialItems;
    public string ResultItems; 
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
#endregion

#region UserMinimo
[Serializable]
public class UMData
{
    public int ID;
    public int Potential;
    public int StatType1;
    public int StatType2;
    public int StatType3;
    public string Name;
}

[Serializable]
public class UMStatData
{
    public int ID;
    public int StatType;
    public int Tier;
    public int Application;
    public string Name;
}

[Serializable]
public class UMStatGrowthData
{
    public int ID;
    public float BaseValue;
    public float Step;
}
#endregion

public class TitleData : DataBase
{
    public Dictionary<int, Quest> Quest { get; private set; } = new();
    public Dictionary<string, int> Common { get; private set; } = new();
    public Dictionary<int, Building> Building { get; private set; } = new();
    public Dictionary<int, Item> Item { get; private set; } = new();
    public Dictionary<int, ProduceData> Produce { get; private set; } = new();
    public Dictionary<string, List<ProduceData>> GroupedProduce { get; private set; } = new();
    public Dictionary<int, UMData> UserMinimo { get; private set; } = new();
    public Dictionary<int, UMStatData> UMStat { get; private set; } = new();
    public Dictionary<int, UMStatGrowthData> UMStatGrowth { get; private set; } = new();

    private Dictionary<string, StringData> _string = new();

    private bool _isGameDataLoaded = false;

    #region Data Path
    private const string STRING_PATH = "Data/StringData";
    private const string QUEST_PATH = "Data/QuestData";
    private const string DETAILQUEST_PATH = "Data/DetailQuestData";
    private const string COMMON_PATH = "Data/CommonData";
    private const string BUILDING_PATH = "Data/BuildingData";
    private const string ITEM_PATH = "Data/ItemData";
    private const string PRODUCE_PATH = "Data/ProduceData";
    private const string UM_PATH = "Data/UMData";
    private const string UMSTAT_PATH = "Data/UMStatData";
    private const string UMSTATGROWTH_PATH = "Data/UMStatGrowthData";
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
        UserMinimo.Clear();
        UMStat.Clear();
        UMStatGrowth.Clear();

        var stringDataRaw = DataLoader.LoadData<StringData>(STRING_PATH);
        foreach (var data in stringDataRaw)
        {
            _string.Add(data.ID, data);
        }
        
        var commonDataRaw = DataLoader.LoadData<CommonData>(COMMON_PATH);
        foreach (var data in commonDataRaw)
        {
            Common.Add(data.ID, data.Value);
        }
        
        var buildingDataRaw = DataLoader.LoadData<BuildingData>(BUILDING_PATH);
        foreach (var data in buildingDataRaw)
        {
            var newBuilding = new Building(data, this);
            Building.Add(data.ID, newBuilding);
        } 
        
        var itemDataRaw = DataLoader.LoadData<ItemData>(ITEM_PATH);
        foreach (var data in itemDataRaw)
        {
            var newItem = new Item(data, this);
            Item.Add(data.ID, newItem);
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
       
        var userMinimoDataRaw = DataLoader.LoadData<UMData>(UM_PATH);
        foreach (var data in userMinimoDataRaw)
        {
            UserMinimo.Add(data.ID, data);
        }
        
        var umStatDataRaw = DataLoader.LoadData<UMStatData>(UMSTAT_PATH);
        foreach (var data in umStatDataRaw)
        {
            UMStat.Add(data.ID, data);
        }
        
        var umStatGrowthDataRaw = DataLoader.LoadData<UMStatGrowthData>(UMSTATGROWTH_PATH);
        foreach (var data in umStatGrowthDataRaw)
        {
            UMStatGrowth.Add(data.ID, data);
        }
        
        var questDataRaw = DataLoader.LoadData<QuestGroupData>(QUEST_PATH);
        var quests = questDataRaw.ToDictionary(data => data.ID);

        var detailQuestDataRaw = DataLoader.LoadData<QuestData>(DETAILQUEST_PATH);
        foreach (var data in detailQuestDataRaw)
        {
            var newQuest = new Quest(quests[data.ID / 100 * 100], data, this);
            Quest.Add(data.ID, newQuest);
        }
        
        
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
