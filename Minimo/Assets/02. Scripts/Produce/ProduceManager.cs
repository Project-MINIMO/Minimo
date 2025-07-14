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
    public event Action<ProduceObject> OnSelected;
    public event Action OnDeselected;
    
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
        if (CurrentObject == obj) return;
        
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
    
    public void Plant()
    {
        if (CurrentObject == null) return;
        
        // Current.StartTask();  // ProduceTask 생성 호출
    }

    public void Harvest()
    {
        if (CurrentObject == null) return;
        // Current.HarvestTask();
    }
}
