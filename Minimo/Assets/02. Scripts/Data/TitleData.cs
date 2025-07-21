using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;


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
}

[Serializable]
public class QuestData
{
    public int ID;
    public int Type;
    public int PreQuestID;
    public int OpenLevel;
    public int Condition;
    public string Name;
    public string Reward;
}

[Serializable]
public class QuestClearData
{
    public int ID;
    public int Type;
    public bool Result;
    public int Target;
    public int Amount;
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

    #region Data Path
    private const string STRING_PATH = "Data/StringData";
    private const string QUESTGROUP_PATH = "Data/QuestGroupData";
    private const string QUEST_PATH = "Data/QuestData";
    private const string QUESTCLEAR_PATH = "Data/QuestClearData";
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
        LoadDataAsync();
    }

    private void LoadData()
    {
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
    }
    
    private async Task LoadDataAsync()
    {
        var buildingRaw = DataLoader.LoadData<BuildingData>(BUILDING_PATH);
        var itemRaw = DataLoader.LoadData<ItemData>(ITEM_PATH);
        var questGroupRaw = DataLoader.LoadData<QuestGroupData>(QUESTGROUP_PATH);
        
        const string buildingAssetPath = "Assets/09. Scriptable Objects/Building/{0}.asset";
        const string itemIconPath = "Assets/03. Images/Item/{0}.png";
        const string questIconPath = "Assets/03. Images/Quest/{0}.png";
    
        var buildingTasks = buildingRaw
            .Select(d => LoadAddressableDataAsync<BuildingPositionData>(d.Name, buildingAssetPath))
            .ToList();
        var iconTasks = itemRaw
            .Select(d => LoadAddressableDataAsync<Sprite>(d.Name, itemIconPath))
            .ToList();
        var groupTasks = questGroupRaw
            .Select(d => LoadAddressableDataAsync<Sprite>(d.Name, questIconPath))
            .ToList();
        
        var buildingPositions = await Task.WhenAll(buildingTasks);
        var itemIcons = await Task.WhenAll(iconTasks);
        var questGroupIcons = await Task.WhenAll(groupTasks);

        for (var i = 0; i < buildingRaw.Length; i++)
        {
            Building.Add(buildingRaw[i].ID, new Building(buildingRaw[i], buildingPositions[i], this));
        }

        for (var i = 0; i < itemRaw.Length; i++)
        {
            Item.Add(itemRaw[i].ID, new Item(itemRaw[i], itemIcons[i], this));
        }
        
        var questGroups = new Dictionary<int, QuestGroup>();
        for (var i = 0; i < questGroupRaw.Length; i++)
        {
            questGroups.Add(questGroupRaw[i].ID, new QuestGroup(questGroupRaw[i], questGroupIcons[i], this));
        }
        
        LoadQuestData(questGroups);
        LoadProduceData();
        AccountInfo.Instance.AddItems(Item);
    }
   
    private Task<T> LoadAddressableDataAsync<T>(string assetName, string assetPath)
    {
        var path = string.Format(assetPath, assetName);
        var handle = Addressables.LoadAssetAsync<T>(path);
        return handle.Task.ContinueWith(task =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded) return handle.Result;
            Debug.LogError($"Failed to load {assetName}");
            return default;
        });
    }

    private void LoadQuestData(Dictionary<int, QuestGroup> questGroups)
    {
        var questRaw = DataLoader.LoadData<QuestData>(QUEST_PATH);
        var questClearRaw = DataLoader.LoadData<QuestClearData>(QUESTCLEAR_PATH);
        
        var groupedQuestClears = questClearRaw
            .GroupBy(data => data.ID)
            .ToDictionary(data => data.Key, data => data.ToList());
     
        var descriptions = new[]
        {
            GetString("STR_QUEST_CLEAR_LEVEL"),
            GetString("STR_QUEST_CLEAR_PREP"),
            GetString("STR_QUEST_CLEAR_HARVEST"),
            GetString("STR_QUEST_CLEAR_WISH"),
            GetString("STR_QUEST_CLEAR_BUILD"),
        };
        
        foreach (var data in questRaw)
        {
            var clear = groupedQuestClears.GetValueOrDefault(data.ID);
            
            var newQuest = questGroups.TryGetValue(data.ID / 100 * 100, out var questGroup) 
                ? new Quest(questGroup, data, clear, descriptions, this) 
                : new Quest(null, data, clear, descriptions, this);
            
            Quest.Add(data.ID, newQuest);
        }
    }

    private void LoadProduceData()
    {
        var produceDataRaw = DataLoader.LoadDataProduceData(PRODUCE_PATH);
        foreach (var data in produceDataRaw)
        {
            Produce.Add(data.ID, data);   
        }
        GroupedProduce = Produce
            .Values
            .GroupBy(data => data.Building)
            .ToDictionary(data => data.Key, data => data.ToList());
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
