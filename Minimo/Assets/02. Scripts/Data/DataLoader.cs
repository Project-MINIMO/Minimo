using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

using UnityEngine;

public class DataLoader
{
    public static T[] LoadData<T>(string dataPath)
    {
        var json = Resources.Load<TextAsset>(dataPath);

        if (json)
        {
            var stringDataList = JsonUtilityHelper.FromJson<T>(json.ToString());

            return stringDataList;
        }

        return null;
    }

    public static ProduceData[] LoadDataProduceData(string dataPath)
    {
        var rawList = LoadData<RawProduceData>(dataPath);
        if (rawList == null)
        {
            return Array.Empty<ProduceData>();
        }
        
        return rawList.Select(raw => new ProduceData
            {
                ID            = raw.ID,
                Building      = raw.Building,
                MaterialItems = ParseMaterials(raw.MaterialItems),
                ResultItems   = ParseResults(raw.ResultItems),
                Time          = raw.Time,
                EXP           = raw.EXP
            })
            .ToArray();
    }

    #region ProduceData Utils
    private static ProduceMaterial[] ParseMaterials(string materialsRaw)
    {
        if (string.IsNullOrEmpty(materialsRaw)) return Array.Empty<ProduceMaterial>();

        return materialsRaw.Split(',').Select(mat =>
        {
            var parts = mat.Split(':').Select(p => p.Trim()).ToArray();
            
            if (parts.Length < 2)
            {
                Debug.LogWarning($"[ParseMaterials] Invalid format: {mat}");
                return null;
            }
            
            return new ProduceMaterial
            {
                ID = GetItemIdFromCode(parts[0]),
                Amount = int.Parse(parts[1])
            };
        }).ToArray();
    }

    private static ProduceResult[] ParseResults(string resultsRaw)
    {
        if (string.IsNullOrEmpty(resultsRaw)) return Array.Empty<ProduceResult>();

        return resultsRaw.Split(',').Select(res =>
        {
            var parts = res.Split(':').Select(p => p.Trim()).ToArray();
            
            if (parts.Length < 2)
            {
                Debug.LogWarning($"[ParseMaterials] Invalid format: {res}");
                return null;
            }
            
            return new ProduceResult
            {
                ID = GetItemIdFromCode(parts[0]),
                Amount = int.Parse(parts[1])
            };
        }).ToArray();
    }

    private static int GetItemIdFromCode(string code)
    {
        var item = App.GetData<TitleData>().Item.FirstOrDefault(x => x.Value.Name == code);
        return item.Key;
    }
    #endregion
    
    #region DetailQuestData Utils
    public static DetailQuestData[] LoadDataDetailQuest(string dataPath)
    {
        var rawList = LoadData<RawDetailQuestData>(dataPath);
        if (rawList == null)
        {
            return Array.Empty<DetailQuestData>();
        }
        
        return rawList.Select(raw => new DetailQuestData
            {
                ID            = raw.ID,
                Type          = (QuestType)raw.Type,
                PreQuestID    = raw.PreQuestID,
                OpenLevel     = raw.OpenLevel,
                Name          = raw.Name,
                Condition     = (QuestCondition)raw.Condition,
                Clear         = ParseClear(raw.Clear),
                Reward        = ParseReward(raw.Reward),
            })
            .ToArray();
    }
    
    private static QuestClear[] ParseClear(string clearRaw)
    {
        if (string.IsNullOrEmpty(clearRaw)) return Array.Empty<QuestClear>();

        return clearRaw.Split(',').Select(res =>
        {
            var parts = res.Split(':').Select(p => p.Trim()).ToArray();

            if (parts.Length < 3)
            {
                Debug.LogWarning($"[ParseCondition] Invalid format: {res}");
                return null;
            }

            return new QuestClear
            {
                Type = (ClearType)int.Parse(parts[0]),
                Target = int.Parse(parts[1]),
                Amount = int.Parse(parts[2])
            };
        }).ToArray();
    }
    
    private static QuestReward[] ParseReward(string rewardRaw)
    {
        if (string.IsNullOrEmpty(rewardRaw)) return Array.Empty<QuestReward>();

        return rewardRaw.Split(',').Select(res =>
        {
            var parts = res.Split(':').Select(p => p.Trim()).ToArray();

            if (parts.Length < 3)
            {
                Debug.LogWarning($"[ParseCondition] Invalid format: {res}");
                return null;
            }

            return new QuestReward
            {
                Type = (RewardType)int.Parse(parts[0]),
                Target = int.Parse(parts[1]),
                Amount = int.Parse(parts[2])
            };
        }).ToArray();
    }
    #endregion
}

public class JsonPreprocessor
{
    // Method to find the "ID" field in a JSON string and convert the string value to a numeric value of the enum.
    public static string PreprocessJson<T>(string json) where T : struct, Enum
    {
        const string pattern = @"""ID"":\s*""(.*?)""";
        
        var processedJson = Regex.Replace(json, pattern, match =>
        {
            var enumString = match.Groups[1].Value;
            if (Enum.TryParse<T>(enumString, out var enumValue))
            {
                var numericValue = Convert.ToInt32(enumValue);
                return $@"""ID"": {numericValue}";
            }
            else
            {
                Debug.LogError("Enum value not found: " + enumString);
                return match.Value;
            }
        });
        
        return processedJson;
    }
}

public class JsonUtilityHelper
{
    public static T[] FromJson<T>(string json)
    {
        string newJson;

        if (json[0] == '{')
        {
            newJson = json;
        }
        else
        {
            newJson = "{ \"array\": " + json + "}";
        }

        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }
    
    public static T[] FromJsonWithConvert<T, U>(string json) where U : struct, Enum
    {
        json = JsonPreprocessor.PreprocessJson<U>(json);

        return FromJson<T>(json);
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new();
        wrapper.array = array;

        return JsonUtility.ToJson(wrapper);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }
}

