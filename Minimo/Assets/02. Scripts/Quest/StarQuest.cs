using System.Linq;

using UnityEngine;

public class StarQuest : QuestGiver
{
    protected override void SetQuest()
    {
        var titleData = App.GetData<TitleData>();
        var filtered = titleData.Quest.Values.Where(q => q.Type == 2).ToList();

        if (filtered.Count == 0)
        {
            Debug.LogError("No quest data found");
            return;
        }
        
        var selected = filtered[Random.Range(0, filtered.Count)];

        _questData = titleData.DetailQuest.Values.FirstOrDefault(x => x.QuestID == selected.ID);
    }
    
    public override void OnLongPress() { }

    public override void OnClickUp() { }
}
