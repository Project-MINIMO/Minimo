using UniRx;
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

    private TitleData _titleData;
    private Quest _quest;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        var questManager = App.GetManager<QuestManager>();
        questManager.CurrentQuest
            .Subscribe(quest =>
            {
                if (quest != null) _quest = quest;
                else ClosePanel();
            })
            .AddTo(this);
        
        _titleData = App.GetData<TitleData>();

        foreach (var view in _stateViews)
        {
            view.Initialize(questManager, _titleData);
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
        
        _infoUpdater.UpdateQuest(_quest);
        
        UpdateStateView();
        UpdateRewardSlots();
    }

    private void UpdateStateView()
    {
        for (var i = 0; i < _stateViews.Length; i++)
        {
            _stateViews[i].gameObject.SetActive(i == (int)_quest.Condition);
        }
        
        _stateViews[(int)_quest.Condition].Setup(_quest);
    }

    private void UpdateRewardSlots()
    {
        var i = 0;
        
        for (; i < _quest.Reward.Length; i++)
        {
            _resultInfos[i].gameObject.SetActive(true);
            
            var reward = _quest.Reward[i];
            _resultInfos[i].UpdateItem(reward.Target.Icon, reward.Amount);
        }

        for (; i < _resultInfos.Length; i++)
        {
            _resultInfos[i].gameObject.SetActive(false);
        }
    }
}
