using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using DG.Tweening;

public class QuestCompactListPanel : QuestListPanel<QuestCompactSlot>
{
    public override bool IsDefaultPanel => true;
    
    private RectTransform _rect;
    private readonly Vector2 _showPosition = new(-2, 0);
    private readonly Vector2 _hidePosition = new(-370, 0);
    
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);
        
        _questManager.OnQuestsUpdated += UpdateQuest;
        
        var questListPanel = manager.GetPanel<QuestDetailListPanel>();
        var longPressDetector = GetComponentInChildren<UILongPressDetector>();
        longPressDetector.OnLongPress = questListPanel.OpenPanel;
        
        foreach (var slot in _slots)
        {
            slot.OnSlotOpened += OnSlotOpened;
        }
        
        _rect = GetComponent<RectTransform>();
    }

    private void OnSlotOpened(QuestCompactSlot openedSlot)
    {
        foreach (var slot in _slots.Where(slot => openedSlot != slot))
        {
            slot.Close();
        }
    }

    public override void Show(bool isNew)
    {
        base.Show(isNew);
        
        _rect.DOAnchorPos(_showPosition, 0.3f).SetEase(Ease.OutCubic);
    }

    public override void Hide(bool isNew)
    {
        base.Hide(isNew);
        
        _rect.DOAnchorPos(_hidePosition, 0.3f).SetEase(Ease.InCubic);
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
