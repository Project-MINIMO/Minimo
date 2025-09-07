using System.Linq;

using UnityEngine.UI;

public class MenuToggleGroup : ToggleGroup
{
    private Toggle _lastSelected;

    public void Show(bool isNew, int index = 0)
    {
        if (isNew)
        {
            var toggle = GetToggleFor(index);
            if (toggle != null)
            {
                toggle.isOn = true;
            }
        }
        else
        {
            _lastSelected.isOn = true;
        }
    }
    
    protected override void OnEnable()
    {
        
    }

    private Toggle GetToggleFor(int index)
    {
        if (m_Toggles.Count <= 0)
        {
            return null;
        }
        
        return m_Toggles
            .OrderBy(t => t.transform.GetSiblingIndex()).ToList()[index];
    }
    
    protected override void OnDisable()
    {
        base.OnDisable();
        
        _lastSelected = m_Toggles.FirstOrDefault(x => x.isOn);
    }
}