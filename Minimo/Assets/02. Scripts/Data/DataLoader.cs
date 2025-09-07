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
                UnlockLevel   = raw.UnlockLevel,
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
        var item = App.GetData<TitleData>().Item.FirstOrDefault(x => x.Value.Code == code);
        return item.Key;
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

