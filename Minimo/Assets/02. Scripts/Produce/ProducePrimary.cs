using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ProducePrimary : ProduceObject
{
    private enum CropType
    {
        Grain,
        Bean,
        Fruit,
    }
    
    [SerializeField] protected SpriteRenderer _cropSpriteRenderer;
    
    private List<Sprite[]> _cropSprites;
   
    private Sprite[] _currentCropSprites;
    private int _currentSpriteIndex;
    
    private PrimaryPanel _primaryPanel;
  
    public override void Initialize(BuildingData data)
    {
        base.Initialize(data);

        _primaryPanel = App.GetManager<UIManager>().GetPanel<PrimaryPanel>();
        
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
        3 => (int)CropType.Grain,
        4 => (int)CropType.Bean,
        5 => (int)CropType.Fruit,
    };

    public override void OpenUI()
    {
        if (ActiveTask == null)
        {
            _primaryPanel.OpenPanel(ProduceState.Idle);
            return;
        }
        
        switch (ActiveTask.CurrentState)
        {
            case CompletedState:
                _primaryPanel.OpenPanel(ProduceState.Complete);
                break;
            
            case ActiveState:
                _primaryPanel.OpenPanel(ProduceState.Produce);
                break;
        }
    }
    
    public override void CloseUI()
    {
        _primaryPanel.ClosePanel();
    }
}
