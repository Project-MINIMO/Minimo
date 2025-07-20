using System.Linq;
using System.Collections.Generic;

public abstract class QuestPanel<T> : UIBase where T : QuestSlot
{
    protected QuestManager _questManager;
    protected Dictionary<QuestType, UIBase> _panelMap;
    protected List<T> _slots;
    
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);
        
        _questManager = App.GetManager<QuestManager>();

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
        
        _slots = GetComponentsInChildren<T>(true).ToList();
        foreach (var slot in _slots)
        {
            slot.OnSlotSelected += OnSlotSelected;
        }
    }
    
    protected virtual void OnSlotSelected(Quest quest) => _panelMap[quest.Type].OpenPanel();
}
