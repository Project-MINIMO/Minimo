using System.Linq;

using UnityEngine.UI;

public class MenuToggleGroup : ToggleGroup
{
    protected override void OnEnable()
    {
        var toggle = GetFirstToggle();
        if (toggle != null)
        {
            toggle.isOn = true;
        }
    }

    private Toggle GetFirstToggle()
    {
        if (m_Toggles.Count <= 0)
        {
            return null;
        }
        
        return m_Toggles
            .OrderBy(t => t.transform.GetSiblingIndex())
            .First();
    }
    
    protected override void OnDisable()
    {
        base.OnDisable();
        
        foreach (var toggle in m_Toggles)
        {
            toggle.isOn = false;
        }
    }
}