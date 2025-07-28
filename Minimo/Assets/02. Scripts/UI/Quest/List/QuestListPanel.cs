using System.Linq;
using System.Collections.Generic;

using UniRx;

public abstract class QuestListPanel<T> : UIBase where T : QuestSlot
{
    protected List<T> Slots;
    protected QuestManager QuestManager;
    
    private Dictionary<QuestType, UIBase> _panelMap;
    
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);
        
        QuestManager = App.GetManager<QuestManager>();
        QuestManager.ActiveQuests.ObserveAdd()
            .Subscribe(addEvent => AssignSlot(addEvent.Value))
            .AddTo(this);
        QuestManager.ActiveQuests.ObserveRemove()
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
        
        QuestManager.DeselectQuest();
    }
    
    protected abstract void AssignSlot(Quest quest);
    protected abstract void ReleaseSlot(Quest quest);
    
    protected virtual void OnSlotSelected(Quest quest)
    {
        QuestManager.SelectQuest(quest);
        _panelMap[quest.Type].OpenPanel();
    }
}