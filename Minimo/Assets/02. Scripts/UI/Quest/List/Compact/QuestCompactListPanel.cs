using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using DG.Tweening;

public class QuestCompactListPanel : QuestListPanel<QuestCompactSlot>
{
    public override bool IsDefaultPanel => true;
    
    private RectTransform _rect;
    
    private readonly Vector2 _showPosition = new(-2, 0);
    private readonly Vector2 _hidePosition = new(-370, 0);
    
    private readonly Queue<QuestCompactSlot> _slotPool = new();
    private readonly Dictionary<Quest, QuestCompactSlot> _activeMap = new();
    
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);
        
        var questListPanel = manager.GetPanel<QuestDetailListPanel>();
        var longPressDetector = GetComponentInChildren<UILongPressDetector>();
        longPressDetector.OnLongPress = questListPanel.OpenPanel;
        
        foreach (var slot in Slots)
        {
            slot.OnSlotOpened += OnSlotOpened;
            slot.gameObject.SetActive(false);
            _slotPool.Enqueue(slot);
        }
        
        _rect = GetComponent<RectTransform>();
    }
    
    public override void Show(bool isNew)
    {
        base.Show(isNew);

        OnSlotOpened(null);
        _rect.DOAnchorPos(_showPosition, 0.3f).SetEase(Ease.OutCubic);
    }

    public override void Hide(bool isNew)
    {
        base.Hide(isNew);

        OnSlotOpened(null);
        _rect.DOAnchorPos(_hidePosition, 0.3f).SetEase(Ease.InCubic);
    }
   
    protected override void AssignSlot(Quest quest)
    {
        var slot = _slotPool.Dequeue();

        slot.gameObject.SetActive(true);
        slot.Initialize(quest);
        _activeMap[quest] = slot;
    }

    protected override void ReleaseSlot(Quest quest)
    {
        if (!_activeMap.TryGetValue(quest, out var slot)) return;
        
        slot.gameObject.SetActive(false);
        _activeMap.Remove(quest);
        _slotPool.Enqueue(slot);
    }
  
    private void OnSlotOpened(QuestCompactSlot openedSlot)
    {
        foreach (var slot in _activeMap.Values.Where(slot => openedSlot != slot))
        {
            slot.Close();
        }
    }
}
