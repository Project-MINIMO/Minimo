using System.Linq;
using System.Collections.Generic;

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
    
    private List<QuestSummaryInfo> _questInfos = new();

    private int _questIndex;
    
    public void Initialize(int index)
    {
        _questManager = App.GetManager<QuestManager>();

        _questIndex = index;
    }

    private void OnEnable()
    {
        if (_questManager == null) return;
        
        _scrollRect.verticalNormalizedPosition = 1;
        
        UpdateQuest();
    }

    public void UpdateQuest()
    {
        _questInfos.Clear();
        
        var existingInfos = GetComponentsInChildren<QuestSummaryInfo>(true);
        
        var quests = _questManager.ActiveQuests.Where(x => x.ID == _questIndex).ToList();
  
        var i = 0;
        
        for (; i < quests.Count; i++)
        {
            var questInfo = i < existingInfos.Length 
                ? existingInfos[i] 
                : Instantiate(_questPrefab, _questParent).GetComponent<QuestSummaryInfo>();

            questInfo.Initialize(quests[i]);
            _questInfos.Add(questInfo);
        }

        for (; i < existingInfos.Length; i++)
        {
            existingInfos[i].gameObject.SetActive(false);
        }
    }
}
