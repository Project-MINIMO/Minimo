using System.Collections.Generic;
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
    
    private Dictionary<QuestType, UIBase> _panelMap;
    QuestManager  _questManager;
    
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);
        _questManager = App.GetManager<QuestManager>();
        var consPanel = manager.GetPanel<QuestConsPanel>();
        var submissionPanel = manager.GetPanel<QuestSubmissionPanel>();
        _panelMap = new Dictionary<QuestType, UIBase>()
        {
            [QuestType.Guide] = submissionPanel,
            [QuestType.Story] = submissionPanel,
            [QuestType.Constellation] = consPanel,
            [QuestType.Side] = submissionPanel,
            [QuestType.Wish] = submissionPanel,
        };
        
        var slots = GetComponentsInChildren<QuestListSlot>(true);
        foreach (var slot in slots)
        {
            slot.OnSlotSelected += OnSlotSelected;
        }
        
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
        _questManager.CurrentQuest = null;
    }
    
    private void OnSlotSelected(Quest quest)
    {
        _questManager.CurrentQuest = quest;
        _panelMap[quest.Type].OpenPanel();
    }
}
