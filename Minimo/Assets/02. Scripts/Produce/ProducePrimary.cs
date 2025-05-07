using System.Collections.Generic;
using System.Linq;
using MinimoShared;
using Cysharp.Threading.Tasks;

using UnityEngine;

public abstract class ProducePrimary : ProduceObject
{
    public override bool IsPrimary => true;
    
    [SerializeField] protected SpriteRenderer _cropSpriteRenderer;
    
    protected List<Sprite[]> _cropSprites;
   
    protected Sprite[] _currentCropSprites;
    protected int _currentSpriteIndex;
  
    public override void Initialize(BuildingDTO buildingDto)
    {
        base.Initialize(buildingDto);

        if (AllTasks.Count > 0)
        {
            SetSpriteResources();
            SetCropSprite();
        }
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

    protected virtual void SetCropSprite()
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
    
    protected override async UniTask CompleteActiveTask()
    {
        await base.CompleteActiveTask();
        
        _currentSpriteIndex = 2;
        _cropSpriteRenderer.sprite = _currentCropSprites[_currentSpriteIndex];
    }
    
    protected override async UniTask OnPlant(ProduceTask task, int optionIndex)
    {
        if (AllTasks.Count > 0)
        {
            return;
        }
        
        await base.OnPlant(task, optionIndex);

        SetSpriteResources();
    }

    private void SetSpriteResources()
    {
        _currentSpriteIndex = 0;

        var cropCode = AllTasks[0].Data.ResultItems[0].ID;
        _currentCropSprites = _cropSprites[GetCropType(cropCode)];
        _cropSpriteRenderer.sprite = _currentCropSprites[_currentSpriteIndex];
    }
    
    public override async UniTask StartHarvest()
    {
        await base.StartHarvest();

        if (ActiveTask == null)
        {
            _cropSpriteRenderer.sprite = null;
        }
        else
        {
            SetCropSprite();
        }
    }
    
    public override async UniTask HarvestEarly()
    {
        await base.HarvestEarly();
        
        _currentSpriteIndex = 2;
        _cropSpriteRenderer.sprite = _currentCropSprites[_currentSpriteIndex];
    }

    protected abstract int GetCropType(int cropCode);
}
