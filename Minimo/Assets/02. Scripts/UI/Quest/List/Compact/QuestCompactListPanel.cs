using System.Collections.Generic;

using UnityEngine;
using DG.Tweening;

public class QuestCompactListPanel : QuestListPanel<QuestCompactSlot>
{
    public override bool IsDefaultPanel => true;
    
    [SerializeField] private QuestCompactSlot _slotPrefab;
    [SerializeField] private Transform _contentParent;   
    
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
        longPressDetector.OnLongPress += questListPanel.OpenPanel;
        
        foreach (var slot in Slots)
        {
            slot.gameObject.SetActive(false);
            _slotPool.Enqueue(slot);
        }
        
        _rect = GetComponent<RectTransform>();
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
   
    protected override void AssignSlot(Quest quest)
    {
        QuestCompactSlot slot;

        if (_slotPool.Count > 0)
        {
            slot = _slotPool.Dequeue();
        }
        else
        {
            slot = Instantiate(_slotPrefab, _contentParent);
        }

        slot.gameObject.SetActive(true);
        slot.Initialize(quest);
        
        slot.transform.SetAsLastSibling();
        
        _activeMap[quest] = slot;
    }

    protected override void ReleaseSlot(Quest quest)
    {
        if (!_activeMap.TryGetValue(quest, out var slot)) return;
        
        slot.gameObject.SetActive(false);
        _activeMap.Remove(quest);
        _slotPool.Enqueue(slot);
    }
}
