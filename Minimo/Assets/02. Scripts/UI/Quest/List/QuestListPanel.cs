using System.Linq;
using System.Collections.Generic;

using UniRx;

public abstract class QuestListPanel<T> : UIBase where T : QuestSlot
{
    protected List<T> Slots;
    
    private QuestManager _questManager;
    private Dictionary<QuestType, UIBase> _panelMap;
    
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);
        
        _questManager = App.GetManager<QuestManager>();
        _questManager.ActiveQuests.ObserveAdd()
            .Subscribe(addEvent => AssignSlot(addEvent.Value))
            .AddTo(this);
        _questManager.ActiveQuests.ObserveRemove()
            .Subscribe(removeEvent => ReleaseSlot(removeEvent.Value))
            .AddTo(this);

        var consPanel = manager.GetPanel<QuestConsPanel>();
        var submissionPanel = manager.GetPanel<QuestSubmissionPanel>();
        _panelMap = new Dictionary<QuestType, UIBase>()
        {
            [QuestType.Guide] = submissionPanel,
            [QuestType.Story] = submissionPanel,
            [QuestType.Constellation] = consPanel,
            [QuestType.Side] = submissionPanel,
            [QuestType.Wish] = submissionPanel,
        };
        
        Slots = GetComponentsInChildren<T>(true).ToList();
        foreach (var slot in Slots)
        {
            slot.OnSlotSelected += OnSlotSelected;
        }
    }
    
    public override void Show(bool isNew)
    {
        base.Show(isNew);
        
        _questManager.DeselectQuest();
    }
    
    protected abstract void AssignSlot(Quest quest);
    protected abstract void ReleaseSlot(Quest quest);
    
    protected void OnSlotSelected(Quest quest)
    {
        _questManager.SelectQuest(quest);
        _panelMap[quest.Type].OpenPanel();
    }
}