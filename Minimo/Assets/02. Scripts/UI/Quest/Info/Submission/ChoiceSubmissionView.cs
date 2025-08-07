using System.Linq;

using UnityEngine.UI;

public class ChoiceSubmissionView : QuestSubmissionView
{
    private Toggle[] _selectTogs;

    public override void Initialize(QuestManager questManager, QuestSubmissionPanel submissionPanel, TitleData titleData)
    {
        base.Initialize(questManager, submissionPanel, titleData);
        
        _selectTogs = SubmissionSlots.Select(x => x.GetComponent<Toggle>()).ToArray();
        foreach (var toggle in _selectTogs)
        {
            toggle.onValueChanged.AddListener(isOn => GetActiveToggle());
        }
    }
    
    public override void Setup(Quest quest)
    {
        base.Setup(quest);

        var i = 0;
        for (; i < quest.Clear.Length; i++)
        {
            SubmissionSlots[i].gameObject.SetActive(true);
            
            var clear = quest.Clear[i];
            SubmissionSlots[i].Initialize(clear.Type, clear.Target, clear.CurrentProgress, clear.Amount);
        }

        for (; i < SubmissionSlots.Length; i++)
        {
            SubmissionSlots[i].gameObject.SetActive(false);
        }

        foreach (var toggle in _selectTogs)
        {
            toggle.isOn = false;
        }
        
        SubmitBtn.interactable = false;
    }

    protected override void Submit()
    {
        var activeIndex = GetActiveToggle();
        if (activeIndex == -1) return;
        if (!Quest.Clear[activeIndex].IsCompleted) return;
        
        QuestManager.SubmitQuest(activeIndex);
        base.Submit();
    }

    private int GetActiveToggle()
    {
        var activeIndex = -1;
        
        for (var i = 0; i < _selectTogs.Length; i++)
        {
            if (!_selectTogs[i].isOn) continue;
            activeIndex = i;
        }

        SubmitBtn.interactable = activeIndex != -1;

        return activeIndex;
    }
}