using System.Collections.Generic;

using UnityEngine;

public class ProducePrimary : ProduceObject
{
    private enum CropType
    {
        Grain,
        Bean,
        Fruit,
    }
    
    private SpriteRenderer _cropSpriteRenderer;
    
    private List<Sprite[]> _cropSprites;
   
    private Sprite[] _currentCropSprites;
    private int _currentSpriteIndex;

    protected override void Awake()
    {
        base.Awake();
        
        _cropSpriteRenderer = transform.GetChild(1).GetComponent<SpriteRenderer>();
    }
    
    public override void Initialize(BuildingData data)
    {
        base.Initialize(data);

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

        if (_currentSpriteIndex == 2)
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

    public override void StartPlant(ProduceData option)
    {
        if (AllTasks.Count > 0)
        {
            return;
        }
        
        base.StartPlant(option);
    }

    protected override void OnPlant(ProduceTask task, int optionIndex)
    {
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
        3 or 47 or 48 => (int)CropType.Grain,
        4 or 49 or 50 => (int)CropType.Bean,
        5 or 51 or 52 => (int)CropType.Fruit,
    };
}
