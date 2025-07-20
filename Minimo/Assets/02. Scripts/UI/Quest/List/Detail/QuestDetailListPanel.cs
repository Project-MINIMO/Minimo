using UnityEngine;
using UnityEngine.UI;

public class QuestDetailListPanel : QuestListPanel<QuestDetailSlot>
{
    public override bool IsUseBlur => true;
    
    [SerializeField] private Button _closeBtn;
    [SerializeField] private Button[] _menuBtns;
    [SerializeField] private QuestListBack[] _menuBacks;
    [SerializeField] private GameObject[] _menuActiveObjs;
    [SerializeField] private GameObject[] _alertObjs;
    
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);
        
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
    
    protected override void OnSlotSelected(Quest quest)
    {
        base.OnSlotSelected(quest);
        
        _questManager.CurrentQuest = quest;
    }
}
