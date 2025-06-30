using System.Linq;

using UnityEngine;

public class QuestSummaryPanel : UIBase
{
    [SerializeField] private QuestUIGrouper _grouper;
    [SerializeField] private Transform _questParent;
    [SerializeField] private GameObject _questPrefab;
    
    [SerializeField] private UILongPressDetector _longPressDetector;
    
    private QuestManager _questManager;
    private QuestListPanel _questListPanel;
    
    public override void Initialize()
    {
        _questManager = App.GetManager<QuestManager>();
        _questListPanel = App.GetManager<UIManager>().GetPanel<QuestListPanel>();
        
        _longPressDetector.OnLongPress = _questListPanel.OpenPanel;
        
        var existingInfos = GetComponentsInChildren<QuestSummaryInfo>(true);
        foreach (var info in existingInfos)
        {
            info.gameObject.SetActive(false);
        }
    }

    public override void OpenPanel()
    {
        base.OpenPanel();
        
        _grouper.CloseAllExcept(this);
    }
  
    public void UpdateQuest()
    {
        var existingInfos = GetComponentsInChildren<QuestSummaryInfo>(true);
        
        var quests = _questManager.ActiveQuests.OrderBy(x => x.ID).ToList();
  
        var i = 0;
        
        for (; i < quests.Count; i++)
        {
            var questInfo = i < existingInfos.Length 
                ? existingInfos[i] 
                : Instantiate(_questPrefab, _questParent).GetComponent<QuestSummaryInfo>();

            questInfo.gameObject.SetActive(true);
            questInfo.Initialize(quests[i]);
        }

        for (; i < existingInfos.Length; i++)
        {
            existingInfos[i].gameObject.SetActive(false);
        }
    }
}
