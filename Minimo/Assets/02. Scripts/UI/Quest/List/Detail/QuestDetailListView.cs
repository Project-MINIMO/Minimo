using System.Collections.Generic;
using System.Linq;

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
    private List<QuestDetailSlot> _slots;

    private int _questIndex;
    
    public void Initialize(int index, QuestManager questManager)
    {
        _questIndex = index;
        questManager.OnQuestsUpdated += UpdateQuest;

        var titleString = GetTitleString(index);
        _titleTMP.SetText(titleString);
        _buttonTMP.SetText(titleString);
        
        _slots = GetComponentsInChildren<QuestDetailSlot>(true).ToList();
        foreach (var slot in _slots)
        {
            slot.gameObject.SetActive(false);
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

    private void UpdateQuest(List<Quest> quests)
    {
        var filteredQuests = quests.Where(x => CheckQuestType(x.Type)).ToList();
        var activeSlots = _slots.Count(x => x.gameObject.activeSelf);

        switch (filteredQuests.Count)
        {
            case var count when count == activeSlots:
                return;
            
            case var count when count > activeSlots && !gameObject.activeInHierarchy:
                _alertObj.SetActive(true);
                break;
        }

        var i = 0;
        
        for (; i < filteredQuests.Count; i++)
        {
            var questInfo = _slots[i];
            questInfo.gameObject.SetActive(true);
            questInfo.Initialize(filteredQuests[i]);
        }

        for (; i < _slots.Count; i++)
        {
            _slots[i].gameObject.SetActive(false);
        }
    }

    private bool CheckQuestType(QuestType questType) => questType switch
    {
        QuestType.Guide => _questIndex == 0,
        QuestType.Story => _questIndex == 0,
        QuestType.Side => _questIndex == 1,
        QuestType.Wish => _questIndex == 1,
        QuestType.Constellation => _questIndex == 2,
        _ => false
    };
}
