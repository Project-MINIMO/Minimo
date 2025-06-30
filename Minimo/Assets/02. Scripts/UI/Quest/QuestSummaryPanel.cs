using System.Linq;

using UnityEngine;

public class QuestSummaryPanel : UIBase
{
    [SerializeField] private Transform _questParent;
    [SerializeField] private GameObject _questPrefab;
    
    [SerializeField] private UILongPressDetector _longPressDetector;
    
    private QuestManager _questManager;
    private QuestListPanel _questListPanel;
    
    public override void Initialize()
    {
        _questManager = App.GetManager<QuestManager>();
        _questListPanel = App.GetManager<UIManager>().GetPanel<QuestListPanel>();
        
        _longPressDetector.OnLongPress = () =>
        {
            _questListPanel.OpenPanel();
            ClosePanel();
        };
        
        var existingInfos = GetComponentsInChildren<QuestInfo>(true);
        foreach (var info in existingInfos)
        {
            info.gameObject.SetActive(false);
        }
    }
  
    public void UpdateQuest()
    {
        var existingInfos = GetComponentsInChildren<QuestInfo>(true);
        
        var quests = _questManager.ActiveQuests.OrderBy(x => x.ID).ToList();
  
        var i = 0;
        
        for (; i < quests.Count; i++)
        {
            var questInfo = i < existingInfos.Length 
                ? existingInfos[i] 
                : Instantiate(_questPrefab, _questParent).GetComponent<QuestInfo>();

            questInfo.gameObject.SetActive(true);
            questInfo.Initialize(quests[i]);
        }

        for (; i < existingInfos.Length; i++)
        {
            existingInfos[i].gameObject.SetActive(false);
        }
    }
}
