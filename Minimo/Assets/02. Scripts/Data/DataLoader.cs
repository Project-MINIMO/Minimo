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
    
    public static T[] LoadDataWithConvert<T, U>(string dataPath) where U : struct, Enum
    {
        var json = Resources.Load<TextAsset>(dataPath);

        if (json)
        {
            var stringDataList = JsonUtilityHelper.FromJsonWithConvert<T, U>(json.ToString());

            return stringDataList;
        }

        return null;
    }
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

public class DataGrouper
{
    public static ProduceData[] GroupData(string path)
    {
        var json = Resources.Load<TextAsset>(path).text;
        
        //Preprocessing JSON string: Converting string value of "ID" field to numeric value of EBuilding enum
        json = JsonPreprocessor.PreprocessJson<EBuilding>(json);
        
        // Deserialize JSON array into a flat list of FlatData
        var rawData = JsonConvert.DeserializeObject<List<FlatProduceData>>(json);

        // Group by ID and map to ProduceData structure
        var groupedData = rawData
            .GroupBy(entry => entry.ID) // Group by Building ID
            .Select(group => new ProduceData
            {
                ID = group.Key,
                ProduceOptions = group.Select(option => new ProduceOption
                {
                    Materials = ParseMaterials(option.Materials),
                    Results = ParseResults(option.Results),
                    Time = option.Time,
                    EXP = option.EXP
                }).ToArray()
            }).ToArray();

        return groupedData;
    }
    
    private static ProduceMaterial[] ParseMaterials(string materialsRaw)
    {
        if (string.IsNullOrEmpty(materialsRaw)) return Array.Empty<ProduceMaterial>();

        return materialsRaw.Split(',').Select(mat =>
        {
            var parts = mat.Split(':').Select(p => p.Trim()).ToArray();
            return new ProduceMaterial
            {
                Code = parts[0],
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
            return new ProduceResult
            {
                Code = parts[0],
                Amount = int.Parse(parts[1])
            };
        }).ToArray();
    }
}

