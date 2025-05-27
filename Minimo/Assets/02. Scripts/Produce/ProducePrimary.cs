using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class ProducePrimary : ProduceObject
{
    private enum CropType
    {
        Wheat,
        Corn,
        Pumpkin,
        Sugarcane,
        Pepper
    }
    
    public override bool IsPrimary => true;
    
    [SerializeField] protected SpriteRenderer _cropSpriteRenderer;
    
    private List<Sprite[]> _cropSprites;
   
    private Sprite[] _currentCropSprites;
    private int _currentSpriteIndex;
  
    public override void Initialize(int id)
    {
        base.Initialize(id);

        if (AllTasks.Count > 0)
        {
            SetSpriteResources();
            SetCropSprite();
        }
        
        _cropSprites = new List<Sprite[]>(ProduceData.Count)
        {
            Resources.LoadAll<Sprite>("Produce/Farm/Wheat"),
            Resources.LoadAll<Sprite>("Produce/Farm/Corn"),
            Resources.LoadAll<Sprite>("Produce/Farm/Pumpkin"),
            Resources.LoadAll<Sprite>("Produce/Farm/Sugarcane"),
            Resources.LoadAll<Sprite>("Produce/Farm/Pepper")
        };
    }
    
    protected override void Update()
    {
        base.Update();

        if (AllTasks.Count == 0) 
        {
            return;
        }
        
        if (_currentCropSprites == null) 
        {
            return;
        }

        if (AllTasks.Any(task => task.RemainTime <= 0))
        {
            return;
        }

        SetCropSprite();
    }

    private void SetCropSprite()
    {
        float remainPercent;

        if (ActiveTask == null)
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
    
    protected override void CompleteActiveTask()
    {
        base.CompleteActiveTask();
        
        _currentSpriteIndex = 2;
        _cropSpriteRenderer.sprite = _currentCropSprites[_currentSpriteIndex];
    }
    
    protected override void OnPlant(ProduceTask task, int optionIndex)
    {
        if (AllTasks.Count > 0)
        {
            return;
        }
        
        base.OnPlant(task, optionIndex);

        SetSpriteResources();
    }

    private void SetSpriteResources()
    {
        _currentSpriteIndex = 0;

        var cropCode = AllTasks[0].Data.ResultItems[0].ID;
        _currentCropSprites = _cropSprites[GetCropType(cropCode)];
        _cropSpriteRenderer.sprite = _currentCropSprites[_currentSpriteIndex];
    }
    
    public override void StartHarvest()
    {
        base.StartHarvest();

        if (ActiveTask == null)
        {
            _cropSpriteRenderer.sprite = null;
        }
        else
        {
            SetCropSprite();
        }
    }
    
    public override void HarvestEarly()
    {
        base.HarvestEarly();
        
        _currentSpriteIndex = 2;
        _cropSpriteRenderer.sprite = _currentCropSprites[_currentSpriteIndex];
    }

    private int GetCropType(int cropCode) => cropCode switch
    {
        0 => (int)CropType.Wheat,
        1 => (int)CropType.Corn,
        2 => (int)CropType.Pumpkin,
        3 => (int)CropType.Sugarcane,
        4 => (int)CropType.Pepper,
        _ => (int)CropType.Wheat
    };
}
