using UnityEngine;
using UnityEngine.UI;

public class QuestSubmissionPanel : UIBase
{
    public override bool IsUseBlur => true;

    [SerializeField] private Button _closeBtn;
    [SerializeField] private QuestTransitioner _transitioner;
    [SerializeField] private QuestInfoUpdater _infoUpdater;
    [SerializeField] private QuestSubmissionView[] _stateViews;
    [SerializeField] private ItemInfoUpdater[] _resultInfos;
    
    private QuestManager _questManager;
    private TitleData _titleData;
    private Quest _questData;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        _questManager = App.GetManager<QuestManager>();
        _titleData = App.GetData<TitleData>();

        foreach (var view in _stateViews)
        {
            view.Initialize(_questManager, _titleData);
        }

        _closeBtn.onClick.AddListener(ClosePanel);
    }
    
    public override void Show(bool isNew)
    {
        base.Show(isNew);

        _transitioner.Open(isNew);
    }

    public override void OpenPanel()
    {
        base.OpenPanel();  
      
        _questData = _questManager.CurrentQuest;
        _infoUpdater.UpdateQuest(_questData);
        
        UpdateStateView();
        UpdateRewardSlots();
    }

    private void UpdateStateView()
    {
        for (var i = 0; i < _stateViews.Length; i++)
        {
            _stateViews[i].gameObject.SetActive(i == (int)_questData.Condition);
        }
        
        _stateViews[(int)_questData.Condition].Setup(_questData);
    }

    private void UpdateRewardSlots()
    {
        var i = 0;
        
        for (; i < _questData.Reward.Length; i++)
        {
            _resultInfos[i].gameObject.SetActive(true);
            
            var reward = _questData.Reward[i];
            _resultInfos[i].UpdateItem(reward.Target.Icon, reward.Amount);
        }

        for (; i < _resultInfos.Length; i++)
        {
            _resultInfos[i].gameObject.SetActive(false);
        }
    }
}
