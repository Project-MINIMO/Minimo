using UnityEngine;
using UnityEngine.UI;

public class QuestDetailListPanel : QuestListPanel<QuestDetailSlot>
{
    public override bool IsUseBlur => true;
    
    [SerializeField] private Button _closeBtn;
    [SerializeField] private Toggle[] _menuTogs;
    [SerializeField] private QuestDetailListView[] _menuBacks;
    
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);
        
        for (var i = 0; i < _menuTogs.Length; i++)
        {
            var index = i;
            
            _menuBacks[index].gameObject.SetActive(true);
            _menuBacks[index].Initialize(index);
            _menuBacks[index].gameObject.SetActive(false);
            
            _menuTogs[index].onValueChanged.AddListener(isOn => 
                _menuBacks[index].gameObject.SetActive(isOn));
        }
        
        _closeBtn.onClick.AddListener(ClosePanel);
    }
}
