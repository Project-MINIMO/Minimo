using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MinimoShared;

using UnityEngine;

public class Produce_Orchard :ProducePrimary
{
    private enum FruitType
    {
        Apple,
        Blueberry,
        Pineapple,
    }

    public override void Initialize(BuildingData data)
    {
        base.Initialize(data);

        _cropSprites = new List<Sprite[]>(ProduceData.ProduceOptions.Length)
        {
            Resources.LoadAll<Sprite>("Produce/Orchard/Apple"),
            Resources.LoadAll<Sprite>("Produce/Orchard/Blueberry"),
            Resources.LoadAll<Sprite>("Produce/Orchard/Pineapple"),
        };
    }

    protected override int GetCropType(string cropCode) => cropCode switch
    {
        "Item_Apple" => (int)FruitType.Apple,
        "Item_Blueberry" => (int)FruitType.Blueberry,
        "Item_Pineapple" => (int)FruitType.Pineapple,
        _ => (int)FruitType.Apple
    };
    
    protected override async UniTask OnPlant(ProduceTask task, int optionIndex)
    {
        if (AllTasks.Count > 0)
        {
            return;
        }
        
        await base.OnPlant(task, optionIndex);
        
        for (var i = 0; i < 4; i++)
        {
            var slotIndex = i + 1;
            var produceTask = new ProduceTask(task.Data, slotIndex);
            var newStartProduce = new BuildingStartProduceDTO
            {
                BuildingId = ID,
                SlotIndex = produceTask.SlotIndex,
                RecipeId = optionIndex + 1
            };
        
            await _buildingManager.StartProduce(newStartProduce);
            AllTasks.Add(produceTask);
        }
    }
    
    protected override void SetCropSprite()
    {
        float remainPercent;

        if (ActiveTask == null)
        {
            remainPercent = 0;
        }
        else if (AllTasks[0]?.RemainTime <= 0) 
        {
            remainPercent = 0;
        }
        else
        {
            remainPercent = (float)ActiveTask.RemainTime / ActiveTask.Data.Time;
        }

        var newSpriteIndex = remainPercent switch
        {
            >= 0.5f => 0,
            >= 0.01f => 1,
            _ => 2
        };

        if (newSpriteIndex != _currentSpriteIndex)
        {
            _currentSpriteIndex = newSpriteIndex;
            _cropSpriteRenderer.sprite = _currentCropSprites[_currentSpriteIndex];
        }
    }
}
