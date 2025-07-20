using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using DG.Tweening;

public class QuestSummaryPanel : UIBase
{
    public override bool IsDefaultPanel => true;

    [SerializeField] private Transform _questParent;
    [SerializeField] private GameObject _questPrefab;
    
    [SerializeField] private UILongPressDetector _longPressDetector;
    
    private QuestManager _questManager;
    private QuestListPanel _questListPanel;
    
    private RectTransform _panelRect;
    private readonly Vector2 _showPosition = new(-2, 0);
    private readonly Vector2 _hidePosition = new(-370, 0);

    private List<QuestSummarySlot> _slots;
    
    private Dictionary<QuestType, UIBase> _panelMap;
    
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        _panelRect = GetComponent<RectTransform>();
        
        _questManager = App.GetManager<QuestManager>();
        _questListPanel = manager.GetPanel<QuestListPanel>();
        _longPressDetector.OnLongPress = _questListPanel.OpenPanel;
        
        _slots = GetComponentsInChildren<QuestSummarySlot>(true).ToList();
        foreach (var slot in _slots)
        {
            slot.OnSlotSelected += OnSlotSelected;
        }
        
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
        
        _questManager.OnQuestsUpdated += UpdateQuest;

    }
    
    private void OnSlotSelected(Quest quest)
    {
        _questManager.CurrentQuest = quest;
        _panelMap[quest.Type].OpenPanel();
    }

    public override void Show(bool isNew)
    {
        _panelRect.DOAnchorPos(_showPosition, 0.3f).SetEase(Ease.OutCubic);
        _questManager.CurrentQuest = null;
    }

    public override void Hide(bool isNew)
    {
        _panelRect.DOAnchorPos(_hidePosition, 0.3f).SetEase(Ease.InCubic);
    }
  
    private void UpdateQuest(List<Quest> quests)
    {
        var orderedQuests = quests.OrderBy(x => x.ID).ToList();
  
        var i = 0;
        
        for (; i < orderedQuests.Count; i++)
        {
            var questInfo = _slots[i];

            questInfo.gameObject.SetActive(true);
            questInfo.Initialize(orderedQuests[i]);
        }

        for (; i < _slots.Count; i++)
        {
            _slots[i].gameObject.SetActive(false);
        }
    }
}
