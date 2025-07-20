using System.Linq;

using UnityEngine;

public class StarQuest : QuestGiver
{
    protected override void SetQuest()
    {
        //var titleData = App.GetData<TitleData>();
        //var filtered = titleData.Quest.Values.Where(q => q.Type == QuestType.Side).ToList();

        //if (filtered.Count == 0)
        //{
        //    Debug.LogError("No quest data found");
        //    return;
        //}
        
        //var selected = filtered[Random.Range(0, filtered.Count)];

        //_questData = titleData.Quest.Values.FirstOrDefault(x => x.ID / 10 == selected.ID);
    }
    
    public override void OnLongPress() { }

    public override void OnClickUp() { }
}
