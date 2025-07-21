using UnityEngine;

public class QuestSubmissionPanel : QuestInfoPanel
{
    [SerializeField] private QuestSubmissionView[] _stateViews;
    [SerializeField] private ItemInfoUpdater[] _resultInfos;
    
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);
        
        var titleData = App.GetData<TitleData>();
        foreach (var view in _stateViews)
        {
            view.Initialize(QuestManager, titleData);
        }
    }

    public override void OpenPanel()
    {
        base.OpenPanel();  

        UpdateStateView();
        UpdateRewardSlots();
    }

    private void UpdateStateView()
    {
        for (var i = 0; i < _stateViews.Length; i++)
        {
            _stateViews[i].gameObject.SetActive(i == (int)SelectedQuest.Condition);
        }
        
        _stateViews[(int)SelectedQuest.Condition].Setup(SelectedQuest);
    }

    private void UpdateRewardSlots()
    {
        var i = 0;
        
        for (; i < SelectedQuest.Reward.Length; i++)
        {
            _resultInfos[i].gameObject.SetActive(true);
            
            var reward = SelectedQuest.Reward[i];
            _resultInfos[i].UpdateItem(reward.Target.Icon, reward.Amount);
        }

        for (; i < _resultInfos.Length; i++)
        {
            _resultInfos[i].gameObject.SetActive(false);
        }
    }
}
