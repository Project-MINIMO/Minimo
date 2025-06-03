using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class Star : InteractObject
{
    private QuestConfirmPanel _questPanel;
    private QuestData _questData;

    private void Awake()
    {
        var filtered = App.GetData<TitleData>().Quest.Values.Where(q => q.Type == 2).ToList();

        if (filtered.Count == 0)
        {
            Debug.LogError("No quest data found");
            return;
        }
        
        _questData = filtered[Random.Range(0, filtered.Count)];

        _questPanel = App.GetManager<UIManager>().GetPanel<QuestConfirmPanel>();
    }
    
    public override void OnLongPress() { }

    public override void OnClickUp()
    {
        _questPanel.OpenPanel(_questData);
    }
}
