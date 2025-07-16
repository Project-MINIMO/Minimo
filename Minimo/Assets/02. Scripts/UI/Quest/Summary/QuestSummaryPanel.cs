using System.Linq;

using UnityEngine;
using DG.Tweening;

public class QuestSummaryPanel : UIBase
{
    public override bool IsDefaultPanel => true;

    [SerializeField] private Transform _questParent;
    [SerializeField] private GameObject _questPrefab;
    
    [SerializeField] private UILongPressDetector _longPressDetector;
    
    private QuestManager _questManager;
    private QuestListPanel _questListPanel;
    
    private RectTransform _panelRect;
    private readonly Vector2 _showPosition = new(-2, 0);
    private readonly Vector2 _hidePosition = new(-370, 0);
    
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        _panelRect = GetComponent<RectTransform>();
        
        _questManager = App.GetManager<QuestManager>();
        _questListPanel = manager.GetPanel<QuestListPanel>();
        
        _longPressDetector.OnLongPress = _questListPanel.OpenPanel;
        
        var existingInfos = GetComponentsInChildren<QuestSummaryInfo>(true);
        foreach (var info in existingInfos)
        {
            info.gameObject.SetActive(false);
        }
    }

    public override void Show(bool isNew)
    {
        _panelRect.DOAnchorPos(_showPosition, 0.3f).SetEase(Ease.OutCubic);
    }

    public override void Hide(bool isNew)
    {
        _panelRect.DOAnchorPos(_hidePosition, 0.3f).SetEase(Ease.InCubic);
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
