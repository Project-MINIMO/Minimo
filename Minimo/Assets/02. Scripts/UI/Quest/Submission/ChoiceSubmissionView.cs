using System.Linq;

using UnityEngine.UI;

public class ChoiceSubmissionView : QuestSubmissionView
{
    private Toggle[] _selectTogs;

    public override void Initialize(QuestManager questManager, TitleData titleData)
    {
        base.Initialize(questManager, titleData);
        
        _selectTogs = _infoUpdaters.Select(x => x.GetComponent<Toggle>()).ToArray();
    }
    
    public override void Setup(Quest quest)
    {
        base.Setup(quest);

        var i = 0;
        for (; i < quest.Clear.Length; i++)
        {
            _infoUpdaters[i].gameObject.SetActive(true);
            
            var clear = quest.Clear[i];
            var icon = clear.Target.Icon;
            var amount = clear.Amount;
            _infoUpdaters[i].UpdateItem(icon, amount);
        }

        for (; i < _infoUpdaters.Length; i++)
        {
            _infoUpdaters[i].gameObject.SetActive(false);
        }
    }

    protected override void Submit()
    {
        var activeIndex = 0;
        
        for (var i = 0; i < _selectTogs.Length; i++)
        {
            if (!_selectTogs[i].isOn) continue;
            if (!Quest.Clear[i].IsCompleted) return;
            activeIndex = i;
        }
        
        QuestManager.SubmitQuest(activeIndex);
    }
}