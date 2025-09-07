using System.Linq;
using System.Collections.Generic;

public class QuestConsPanel : QuestInfoPanel
{
    private List<QuestConsSlot> _slots;
    private QuestSubmissionPanel _submissionPanel;
    
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);
        
        _slots = GetComponentsInChildren<QuestConsSlot>(true).ToList();
        foreach (var slot in _slots)
        {
            slot.OnSlotSelected += OnSlotSelected;
        }
        
        _submissionPanel = manager.GetPanel<QuestSubmissionPanel>();
    }

    public override void Show(bool isNew)
    {
        base.Show(isNew);
        
        _infoUpdater.UpdateQuestGroup(SelectedQuest);
        UpdateQuest(SelectedQuest);
    }

    private void UpdateQuest(Quest questData)
    {
        var quests = QuestManager.GroupedQuest[questData.Group.ID];

        var i = 0;
        var activeIndex = int.MaxValue;
        
        for (; i < quests.Count; i++)
        {
            _slots[i].gameObject.SetActive(true);
            
            if (quests[i] == questData)
            {
                activeIndex = i;
            }
            
            _slots[i].Initialize(GetQuestState(i, activeIndex), quests[i]);
        }
        
        for (; i < _slots.Count; i++)
        {
            _slots[i].gameObject.SetActive(false);
        }
    }

    private QuestState GetQuestState(int index, int activeIndex) => (index, activeIndex) switch
    {
        var (i, a) when i < a => QuestState.Completed,
        var (i, a) when i == a => QuestState.InProgress,
        var (i, a) when i > a => QuestState.Locked,
        _ => QuestState.Completed
    };

    private void OnSlotSelected(Quest quest) => _submissionPanel.OpenPanel();
}
