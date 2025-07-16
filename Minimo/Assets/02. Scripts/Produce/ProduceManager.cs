using System;
using System.Collections.Generic;

using UnityEngine;
using DG.Tweening;

public enum BuildingTier
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
    
    private Dictionary<BuildingTier, UIBase> _panelMap;
    private Camera _camera;

    private void Start()
    {
        var uiManager = App.GetManager<UIManager>();
        
        _panelMap = new Dictionary<BuildingTier, UIBase>
        {
            { BuildingTier.Tier1, uiManager.GetPanel<PrimaryPanel>() },
            { BuildingTier.Tier2, uiManager.GetPanel<SecondaryPanel>() },
            { BuildingTier.Tier3, uiManager.GetPanel<TertiaryPanel>() },
            { BuildingTier.Tier4, uiManager.GetPanel<QuaternaryPanel>() }
        };
        
        _camera = Camera.main;
    }
    
    public void RegisterTertiary(ProduceTertiary tertiary)   => _tertiaryBuildings.Add(tertiary);
    public void UnregisterTertiary(ProduceTertiary tertiary) => _tertiaryBuildings.Remove(tertiary);

    public void Select(ProduceObject obj)
    {
        Deselect();
        
        CurrentObject = obj;
        MoveCamera(_panelMap[(BuildingTier)obj.BuildingData.Type].OpenPanel);
    }

    public void Deselect()
    {
        if (CurrentObject == null) return;
        
        _panelMap[(BuildingTier)CurrentObject.BuildingData.Type].ClosePanel();
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
            .DOMove(targetPos, 0.5f)
            .SetEase(Ease.OutCubic)
            .OnComplete(() => onComplete?.Invoke());
    }
    
    public void Plant(ProduceData option)
    {
        if (CurrentObject == null) return;
        
        CurrentObject.StartPlant(option);
    }

    public void Plant(ProduceObject obj, ProduceData option)
    {
        obj.StartPlant(option);
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
