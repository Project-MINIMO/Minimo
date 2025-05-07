using System.Collections.Generic;

using UnityEngine;

public class Produce_Farm : ProducePrimary
{
    private enum CropType
    {
        Wheat,
        Corn,
        Pumpkin,
        Sugarcane,
        Pepper
    }

    public override void Initialize(BuildingData data)
    {
        base.Initialize(data);

        /*
        _cropSprites = new List<Sprite[]>(ProduceData.ProduceOptions.Length)
        {
            Resources.LoadAll<Sprite>("Produce/Farm/Wheat"),
            Resources.LoadAll<Sprite>("Produce/Farm/Corn"),
            Resources.LoadAll<Sprite>("Produce/Farm/Pumpkin"),
            Resources.LoadAll<Sprite>("Produce/Farm/Sugarcane"),
            Resources.LoadAll<Sprite>("Produce/Farm/Pepper")
        };
        */
    }
    
    protected override int GetCropType(int cropCode) => cropCode switch
    {
        0 => (int)CropType.Wheat,
        1 => (int)CropType.Corn,
        2 => (int)CropType.Pumpkin,
        3 => (int)CropType.Sugarcane,
        4 => (int)CropType.Pepper,
        _ => (int)CropType.Wheat
    };
}
