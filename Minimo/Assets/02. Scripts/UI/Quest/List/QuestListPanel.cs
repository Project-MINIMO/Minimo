using UnityEngine;
using UnityEngine.UI;

public class QuestListPanel : UIBase
{
    public override bool IsUseBlur => true;
    
    [SerializeField] private Button _closeBtn;
    
    [SerializeField] private Button[] _menuBtns;
    [SerializeField] private QuestListBack[] _menuBacks;
    [SerializeField] private GameObject[] _menuActiveObjs;
    [SerializeField] private GameObject[] _alertObjs;

    private QuestSummaryPanel _questSummaryPanel;
    
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        _questSummaryPanel = manager.GetPanel<QuestSummaryPanel>();
        
        _closeBtn.onClick.AddListener(ClosePanel);
        
        SetButtonEvent();
    }
    
    private void SetButtonEvent()
    {
        for (var i = 0; i < _menuBtns.Length; i++)
        {
            var idx = i;

            _menuBtns[idx].onClick.AddListener(() => OnClickMenuBtn(idx));

            _menuBacks[idx].gameObject.SetActive(true);
            _menuBacks[idx].Initialize(idx);
            _menuBacks[idx].gameObject.SetActive(false);
        }
    }
    
    private void OnClickMenuBtn(int index)
    {
        for (var i = 0; i < _menuBtns.Length; i++)
        {
            var active = index == i;
            
            _menuBacks[i].gameObject.SetActive(active);
            _menuActiveObjs[i].SetActive(active);

            if (active)
            {
                _alertObjs[i].SetActive(false);
            }
        }
    }

    public override void OpenPanel()
    {
        base.OpenPanel();
        
        OnClickMenuBtn(0);
    }

    public void UpdateQuest(QuestType questType)
    {
        var index = GetQuestType(questType);
        
        if (!_menuBacks[index].gameObject.activeSelf)
        {
            _alertObjs[index].SetActive(true);
        }
        
        _menuBacks[index].UpdateQuest();
    }
    
    private int GetQuestType(QuestType questType) => questType switch
    {
        QuestType.Guide => 0,
        QuestType.Story => 0,
        QuestType.Constellation => 1,
        _ => 2
    };
}
