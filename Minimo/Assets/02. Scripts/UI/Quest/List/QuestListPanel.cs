using System.Collections.Generic;

public abstract class QuestListPanel<T> : QuestPanel<T> where T : QuestSlot
{
    protected override void OnSlotSelected(Quest quest)
    {
        _questManager.SelectQuest(quest);
        
        base.OnSlotSelected(quest);
    }

    public override void Show(bool isNew)
    {
        base.Show(isNew);
        
        _questManager.DeselectQuest();
    }
}
