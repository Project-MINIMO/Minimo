using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class QuestCompactListPanel : QuestListPanel<QuestCompactSlot>
{
    public override bool IsDefaultPanel => true;
    
    [SerializeField] private QuestCompactSlot _slotPrefab;
    [SerializeField] private Transform _contentParent;
    
    [SerializeField] private RectTransform _contentRect;
    [SerializeField] private Button _openBtn;
    [SerializeField] private Button _closeBtn;
    
    private RectTransform _rect;

    private const int ShowPosition = 130;
    private const int HidePosition = -100;

    private const int OpenPosition = 0;
    private const int ClosePosition = -555;

    private readonly Queue<QuestCompactSlot> _slotPool = new();
    private readonly Dictionary<Quest, QuestCompactSlot> _activeMap = new();
    
    private bool _isOpened = true;
    
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);
        
        var questListPanel = manager.GetPanel<QuestDetailListPanel>();
        var longPressDetector = GetComponentInChildren<UILongPressDetector>(true);
        longPressDetector.OnLongPress += questListPanel.OpenPanel;
        
        foreach (var slot in Slots)
        {
            slot.gameObject.SetActive(false);
            _slotPool.Enqueue(slot);
        }
        
        _openBtn.onClick.AddListener(Toggle);
        _closeBtn.onClick.AddListener(Toggle);
        Toggle();
        
        _rect = GetComponent<RectTransform>();
    }
    
    public override void Show(bool isNew)
    {
        base.Show(isNew);
        
        _rect.DOAnchorPosX(ShowPosition, 0.3f).SetEase(Ease.OutCubic);
    }

    public override void Hide(bool isNew)
    {
        base.Hide(isNew);
        
        _rect.DOAnchorPosX(HidePosition, 0.3f).SetEase(Ease.InCubic);
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
            slot.OnSlotSelected += OnSlotSelected;
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
    
    protected override void OnSlotSelected(Quest quest)
    {
        if (quest.Condition != QuestCondition.Normal)
        {
            base.OnSlotSelected(quest);
            return;
        }
        
        foreach (var clear in quest.Clear)
        {
            if (!clear.IsCompleted)
            {
                base.OnSlotSelected(quest);
                return;
            }
        }
        
        QuestManager.SubmitQuest(quest);
    }
    
    private void Toggle()
    {
        if (_isOpened)
        {
            Close();
        }
        else
        {
            Open();
        }
        
        _openBtn.gameObject.SetActive(!_isOpened);
        _closeBtn.gameObject.SetActive(_isOpened);
    }

    private void Open()
    {
        if (_isOpened) return;
        
        _isOpened = true;
        _contentRect.DOKill();
        _contentRect.DOAnchorPosX(OpenPosition, 0.1f).SetEase(Ease.Linear);
    }

    private void Close()
    {
        if (!_isOpened) return;
        
        _isOpened = false;
        _contentRect.DOKill();
        _contentRect.DOAnchorPosX(ClosePosition, 0.1f).SetEase(Ease.Linear);
    }
}
