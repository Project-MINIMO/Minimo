using System.Linq;

using UnityEngine.UI;
using TMPro;
using UnityEngine;

public interface ISubmissionStrategy
{
    void Initialize(TextMeshProUGUI progressTMP, QuestItemSelectedSlot[] slots);
    void Setup(Quest quest);
    void OnSubmit();
    void OnCancel();
    void SelectItem(Item item);
}

public class NormalSubmissionStrategy : ISubmissionStrategy
{
    private int _required;
    private TextMeshProUGUI _progressTMP;
    private QuestItemSelectedSlot[] _slots;

    public void Initialize(TextMeshProUGUI progressTMP, QuestItemSelectedSlot[] slots)
    {
        _progressTMP = progressTMP;
        _slots = slots;
    }
    
    public void Setup(Quest quest)
    {
        _progressTMP.gameObject.SetActive(true);
        _progressTMP.text = $"0 / 0";

        var clearCount = quest.Clear.Length;
        for (var i = 0; i < _slots.Length; i++) 
        {
            var active = i < clearCount
                       && quest.Clear[i].Type == ClearType.Wish;
            _slots[i].gameObject.SetActive(active);
            if (!active) continue;
            
            var clear = quest.Clear[i];
            var item  = AccountInfo.Instance.Items[clear.Target];
            _slots[i].Initialize(item, clear.Amount, QuestCondition.Normal);
        }
        
        _required = quest.Clear.Count(c => c.Type == ClearType.Wish);
    }

    public void OnSubmit()
    {
        var submitted = _slots
            .Where(s => !s.CanSelect)  
            .Select(s => s.Item)
            .ToArray();
        if (submitted.Length != _required) return;
        App.GetManager<QuestManager>().SubmitQuest();
    }

    public void OnCancel()
    {
        foreach (var slot in _slots)
        {
            slot.RemoveItem();
        }
    }

    public void SelectItem(Item item)
    {
        var slot = _slots.FirstOrDefault(x => x.Item == item && x.CanSelect);

        if (slot == null) return;
        if (!slot.CanSelect) return;
        
        slot.AddItem(item);
    }
}

public class ChoiceSubmissionStrategy : ISubmissionStrategy
{    
    private GameObject _progressObj;
    private QuestItemSelectedSlot[] _slots;
    private QuestItemSelectedSlot _selectedSlot;
    
    public void Initialize(TextMeshProUGUI progressTMP, QuestItemSelectedSlot[] slots)
    {
        _progressObj = progressTMP.gameObject;
        _slots = slots;
    }
    
    public void Setup(Quest quest)
    {
        _progressObj.SetActive(false);
        
        var count = quest.Clear.Length;
        for (var i = 0; i < _slots.Length; i++) 
        {
            var active = i < count && i < 2 && quest.Clear[i].Type == ClearType.Wish;
            _slots[i].gameObject.SetActive(active);
            if (!active) continue;
            
            _slots[i].Initialize(null, 1, QuestCondition.Choice);
            _slots[i].OnSlotSelected = slot => {
                _selectedSlot = slot;
            };
        }
    }
    
    public void OnSubmit()
    {
        if (_selectedSlot == null) return;
        App.GetManager<QuestManager>().SubmitQuest(_selectedSlot.Item);
    }
    
    public void OnCancel()
    {
        foreach (var slot in _slots)
        {
            slot.RemoveItem();
        }
    }

    public void SelectItem(Item item) { }
}

public class QuizSubmissionStrategy : ISubmissionStrategy
{
    private GameObject _progressObj;
    private QuestItemSelectedSlot[] _slots;
    
    public void Initialize(TextMeshProUGUI progressTMP, QuestItemSelectedSlot[] slots)
    {
        _progressObj = progressTMP.gameObject;
        _slots = slots;
    }
    
    public void Setup(Quest quest)
    {
        _progressObj.gameObject.SetActive(false);
        
        for (var i = 0; i < quest.Clear.Length && i < _slots.Length; i++)
        {
            var active = i == 0 && quest.Clear[i].Type == ClearType.Wish;
            _slots[i].gameObject.SetActive(active);
            if (!active) continue;
            
            _slots[i].Initialize(null, int.MaxValue, QuestCondition.Quiz);
            _slots[i].OnSlotSelected = slot => {
                var idx = slot.transform.GetSiblingIndex();
                var clear = quest.Clear[idx];
                var item  = AccountInfo.Instance.Items[clear.Target];
                slot.AddItem(item);
            };
        }
    }
    
    public void OnSubmit()
    {
        var item = _slots[0].Item;
        if (item == null) return;
        App.GetManager<QuestManager>().SubmitQuest(item);
    }
    
    public void OnCancel()
    {
        _slots[0].RemoveItem();
    }

    public void SelectItem(Item item)
    {
        _slots[0].AddItem(item);
    }
}
