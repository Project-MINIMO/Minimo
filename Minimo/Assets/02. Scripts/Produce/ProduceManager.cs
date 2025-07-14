using System;
using System.Collections.Generic;

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
    
    private Dictionary<BuildingTier, UIBase> _panelMap;

    private void Start()
    {
        var uiManager = App.GetManager<UIManager>();
        
        _panelMap = new Dictionary<BuildingTier, UIBase>
        {
            { BuildingTier.Tier1, uiManager.GetPanel<PrimaryPanel>() },
            { BuildingTier.Tier2, uiManager.GetPanel<PrimaryPanel>() },
            { BuildingTier.Tier3, uiManager.GetPanel<AdvancedPanel>() },
            { BuildingTier.Tier4, uiManager.GetPanel<WishPanel>() }
        };
    }

    public void Select(ProduceObject obj)
    {
        Deselect();
        
        CurrentObject = obj;
        _panelMap[(BuildingTier)obj.BuildingData.Type].OpenPanel();
    }

    public void Deselect()
    {
        if (CurrentObject == null) return;
        
        _panelMap[(BuildingTier)CurrentObject.BuildingData.Type].ClosePanel();
        CurrentObject = null;
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

    public void HarvestEarly()
    {
        if (CurrentObject == null) return;

        CurrentObject.HarvestEarly();
    }
}
