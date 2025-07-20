using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class QuestItemSelectedSlot : MonoBehaviour
{
    public bool CanSelect => _count < _maxCount;
    public Action<QuestItemSelectedSlot> OnSlotSelected;
    public Item Item { get; private set; }
    
    [SerializeField] private Toggle _selectedTog;
    
    [SerializeField] private Image _iconImg;
    [SerializeField] private TextMeshProUGUI _countTMP;
    
    private int _count;
    private int _maxCount;
    private QuestCondition _mode;
    
    private void Awake()
    {
        _selectedTog.group = GetComponentInParent<ToggleGroup>();
        _selectedTog.onValueChanged.AddListener(isOn =>
        {
            if (isOn) OnSlotSelected?.Invoke(this);
        });
    }
    
    public void Initialize(Item item, int maxCount, QuestCondition mode)
    {
        _mode = mode;
        _maxCount = maxCount;
        _count = 0;
        
        Item = item;
        
        _iconImg.gameObject.SetActive(mode == QuestCondition.Normal);
        _countTMP.gameObject.SetActive(mode == QuestCondition.Normal);
        _selectedTog.isOn = false;
        _selectedTog.enabled = mode == QuestCondition.Choice;

        if (mode == QuestCondition.Normal)
        {
            _iconImg.sprite = item.Icon;
            _countTMP.text = $"0 / {_maxCount}";
        }
    }
    
    public void AddItem(Item newItem)
    {
        if (!CanSelect) return;

        _count++;
        
        switch (_mode)
        {
            case QuestCondition.Normal:
                _countTMP.text = $"{_count} / {_maxCount}";
                break;
            
            case QuestCondition.Quiz when Item == null:
                Item = newItem;
                _iconImg.sprite = newItem.Icon;
                _iconImg.gameObject.SetActive(true);
                break;
        }

        _count++;
    }

    public void RemoveItem()
    {
        _count = 0;
        
        switch (_mode)
        {
            case QuestCondition.Normal:
                _countTMP.text = $"{_count} / {_maxCount}";
                break;
            
            case QuestCondition.Choice:
                _selectedTog.isOn = false;
                break;
            
            case QuestCondition.Quiz when Item == null:
                Item = null;
                _iconImg.sprite = null;
                _iconImg.gameObject.SetActive(false);
                break;
        }
    }
}
