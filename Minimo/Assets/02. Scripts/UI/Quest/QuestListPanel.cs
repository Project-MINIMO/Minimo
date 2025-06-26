using UnityEngine;
using UnityEngine.UI;

public class QuestListPanel : UIBase
{
    [SerializeField] private Button _closeBtn;
    
    [SerializeField] private Button[] _menuBtns;
    [SerializeField] private QuestListBack[] _menuBacks;
    [SerializeField] private GameObject[] _menuActiveObjs;
    [SerializeField] private GameObject[] _alertObjs;

    private QuestSummaryPanel _questSummaryPanel;
    
    public override void Initialize()
    {
        _questSummaryPanel = App.GetManager<UIManager>().GetPanel<QuestSummaryPanel>();
        
        _closeBtn.onClick.AddListener(() =>
        {
            _questSummaryPanel.OpenPanel();
            ClosePanel();
        });
        
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
            _menuBacks[i].gameObject.SetActive(index == i);
            _menuActiveObjs[i].SetActive(index == i);
            _alertObjs[i].SetActive(index == i);
        }
    }

    public override void OpenPanel()
    {
        base.OpenPanel();
        
        OnClickMenuBtn(0);
    }

    public void UpdateQuest(int questType)
    {
        if (!_menuBacks[questType].gameObject.activeSelf)
        {
            _alertObjs[questType].SetActive(true);
        }
        
        _menuBacks[questType].UpdateQuest();
    }
}
