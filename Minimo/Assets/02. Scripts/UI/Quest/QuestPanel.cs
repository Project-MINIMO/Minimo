using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class QuestPanel : UIBase
{
    [SerializeField] private Transform _questParent;
    [SerializeField] private GameObject _questPrefab;
    
    private QuestManager _questManager;
    
    private List<QuestInfo> _questList;
    
    public override void Initialize()
    {
        _questManager = App.GetManager<QuestManager>();
    }

    public void UpdateQuest()
    {
        _questList.Clear();
        
        var existingInfos = GetComponentsInChildren<QuestInfo>(true);
        
        var quests = _questManager.ActiveQuests.OrderBy(x => x.ID).ToList();
  
        var i = 0;
        
        for (; i < quests.Count; i++)
        {
            var questInfo = i < existingInfos.Length 
                ? existingInfos[i] 
                : Instantiate(_questPrefab, _questParent).GetComponent<QuestInfo>();

            questInfo.Initialize(quests[i]);
            _questList.Add(questInfo);
        }

        for (; i < existingInfos.Length; i++)
        {
            existingInfos[i].gameObject.SetActive(false);
        }
    }
}
