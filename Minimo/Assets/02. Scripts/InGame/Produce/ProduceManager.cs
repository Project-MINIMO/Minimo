using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public enum BuildingType
{
    Tier1,
    Tier2,
    Tier3,
    Tier4,
}

public class ProduceManager : ManagerBase
{
    public ProduceObject CurrentObject { get; private set; }
    
    private readonly List<ProduceTertiary> _tertiaryBuildings = new();
    public IReadOnlyList<ProduceTertiary> TertiaryBuildings => _tertiaryBuildings;
    
    private Dictionary<BuildingType, UIBase> _panelMap;
    private PlantService _plantService;
    private Camera _camera;

    private void Start()
    {
        var uiManager = App.GetManager<UIManager>();
        
        _panelMap = new Dictionary<BuildingType, UIBase>
        {
            { BuildingType.Tier1, uiManager.GetPanel<PrimaryPanel>() },
            { BuildingType.Tier2, uiManager.GetPanel<SecondaryPanel>() },
            { BuildingType.Tier3, uiManager.GetPanel<TertiaryPanel>() },
            { BuildingType.Tier4, uiManager.GetPanel<QuaternaryPanel>() }
        };
        
        _plantService = new PlantService(uiManager.GetPanel<UseCashPanel>());
        
        _camera = Camera.main;
    }
    
    public void RegisterTertiary(ProduceTertiary tertiary)
    {
        _tertiaryBuildings.Add(tertiary);
        _tertiaryBuildings.Sort((a, b) =>
            a.BuildingData.ID.CompareTo(b.BuildingData.ID));
    }

    public void UnregisterTertiary(ProduceTertiary tertiary) => _tertiaryBuildings.Remove(tertiary);

    public void Select(ProduceObject obj)
    {
        Deselect();
        
        CurrentObject = obj;
        MoveCamera(_panelMap[(BuildingType)obj.BuildingData.Type].OpenPanel);
    }

    public void Deselect()
    {
        if (CurrentObject == null) return;
        
        _panelMap[CurrentObject.BuildingData.Type].ClosePanel();
        CurrentObject = null;
    }

    private void MoveCamera(Action onComplete = null)
    {
        if (CurrentObject == null) return;
        
        var targetPos = new Vector3(
            CurrentObject.transform.position.x,
            CurrentObject.transform.position.y,
            _camera.transform.position.z
        );

        if (_camera.transform.position == targetPos)
        {
            onComplete?.Invoke();
            return;
        }

        _camera.transform
            .DOMove(targetPos, 0.3f)
            .SetEase(Ease.OutCubic)
            .OnComplete(() => onComplete?.Invoke());
    }
    
    public void RequestPlant(
        ProduceObject target, 
        ProduceData option, 
        Action onSuccess = null, 
        Action<NotifyType> onFailed = null)
    {
        _plantService.TryPlant(
            target,
            option,
            onSuccess: () =>
            {
                onSuccess?.Invoke();
                Debug.Log("success");
            },
            onFailed: reason =>
            {
                onFailed?.Invoke(reason);
                Debug.Log(reason);
            }
        );
    }
    
    public void RequestPlant(
        ProduceData option, 
        Action onSuccess = null, 
        Action<NotifyType> onFailed = null)
    {
        if (CurrentObject == null) return;

        RequestPlant(CurrentObject, option, onSuccess, onFailed);
    }

    public void RequestPlant(
        Item[] materials, 
        Action onSuccess = null, 
        Action<NotifyType> onFailed = null)
    {
        if (CurrentObject == null) return;
        
        var options = CurrentObject.ProduceData
            .Where(x => x.MaterialItems[0].ID == materials[1].ID)
            .Where(x => x.MaterialItems[1].ID == materials[0].ID).ToList();

        if (options.Count == 0)
        {
            onFailed?.Invoke(NotifyType.MissRecipe);
            return;
        }
        
        RequestPlant(CurrentObject, options[0], onSuccess, onFailed);
    }

    public void Harvest()
    {
        if (CurrentObject == null) return;

        CurrentObject.StartHarvest();
    }

    public void Harvest(ProduceObject obj)
    {
        obj.StartHarvest();
    }

    public void Skip()
    {
        if (CurrentObject == null) return;

        CurrentObject.Skip();
    }

    public void MoveToNextTertiary(int num)
    {
        var index = _tertiaryBuildings.IndexOf(CurrentObject as ProduceTertiary);
        var nextIndex = (index + num + _tertiaryBuildings.Count) % _tertiaryBuildings.Count;
        var nextObject = _tertiaryBuildings[nextIndex];
        Select(nextObject);
    }
}
