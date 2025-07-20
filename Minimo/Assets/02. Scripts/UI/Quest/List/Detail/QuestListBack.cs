using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestListBack : MonoBehaviour
{
    [SerializeField] private Transform _questParent;
    [SerializeField] private GameObject _questPrefab;
    [SerializeField] private ScrollRect _scrollRect;
    
    [SerializeField] private TextMeshProUGUI _buttonTMP;
    [SerializeField] private TextMeshProUGUI _titleTMP;
    
    private QuestManager _questManager;

    private int _questIndex;
    
    public void Initialize(int index)
    {
        _questManager = App.GetManager<QuestManager>();
        _questManager.OnQuestsUpdated += UpdateQuest;
        _questIndex = index;
    }

    private void OnEnable()
    {
        if (_questManager == null) return;
        
        _scrollRect.verticalNormalizedPosition = 1;
    }

    private void UpdateQuest(List<Quest> quests)
    {
        var filteredQuests = quests.Where(x => CheckQuestType(x.Type)).ToList();

        var existingInfos = GetComponentsInChildren<QuestDetailSlot>(true);
        
        var i = 0;
        
        for (; i < filteredQuests.Count; i++)
        {
            var questInfo = i < existingInfos.Length 
                ? existingInfos[i] 
                : Instantiate(_questPrefab, _questParent).GetComponent<QuestDetailSlot>();

            questInfo.gameObject.SetActive(true);
            questInfo.Initialize(filteredQuests[i]);
        }

        for (; i < existingInfos.Length; i++)
        {
            existingInfos[i].gameObject.SetActive(false);
        }
    }

    private bool CheckQuestType(QuestType questType) => questType switch
    {
        QuestType.Guide => _questIndex == 0,
        QuestType.Story => _questIndex == 0,
        QuestType.Constellation => _questIndex == 1,
        _ => _questIndex == 2
    };
}
