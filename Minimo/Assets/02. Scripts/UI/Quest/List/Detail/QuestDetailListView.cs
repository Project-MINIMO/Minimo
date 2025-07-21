using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestDetailListView : MonoBehaviour
{
    [SerializeField] private ScrollRect _scrollRect;
    
    [SerializeField] private TextMeshProUGUI _titleTMP;
    [SerializeField] private TextMeshProUGUI _buttonTMP;
    [SerializeField] private GameObject _alertObj;
    
    private QuestManager _questManager;

    private readonly Queue<QuestDetailSlot> _slotPool = new();
    private readonly Dictionary<Quest, QuestDetailSlot> _activeMap = new();
    
    public void Initialize(int index)
    {
        var titleString = GetTitleString(index);
        _titleTMP.SetText(titleString);
        _buttonTMP.SetText(titleString);
        
        var slots = GetComponentsInChildren<QuestDetailSlot>(true).ToList();
        foreach (var slot in slots)
        {
            slot.gameObject.SetActive(false);
            _slotPool.Enqueue(slot);
        }
    }

    private string GetTitleString(int index) => index switch
    {
        0 => App.GetData<TitleData>().GetString("STR_QUEST_MAIN"),
        1 => App.GetData<TitleData>().GetString("STR_QUEST_SIDE"),
        2 => App.GetData<TitleData>().GetString("STR_QUEST_CONSTELLATION"),
        _ => string.Empty
    };

    private void OnEnable()
    {
        _scrollRect.verticalNormalizedPosition = 1;
        _alertObj.SetActive(false);
    }
    
    public void AddQuest(Quest quest)
    {
        var slot = _slotPool.Dequeue();

        slot.gameObject.SetActive(true);
        slot.Initialize(quest);
        _activeMap[quest] = slot;
        
        _alertObj.SetActive(true);
    }

    public void RemoveQuest(Quest quest)
    {
        if (!_activeMap.TryGetValue(quest, out var slot)) return;
        
        slot.gameObject.SetActive(false);
        _activeMap.Remove(quest);
        _slotPool.Enqueue(slot);
    }
}
