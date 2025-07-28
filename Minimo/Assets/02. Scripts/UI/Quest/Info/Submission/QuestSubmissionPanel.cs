using UnityEditor.iOS.Extensions.Common;
using UnityEngine;
using UnityEngine.UI;

public class QuestSubmissionPanel : QuestInfoPanel
{
    [SerializeField] private QuestSubmissionView[] _stateViews;
    [SerializeField] private ItemInfoUpdater[] _resultInfos;
    [SerializeField] private Button _guideBtn;
    
    private UIManager _uiManager;
    private BuildingPanel _buildingPanel;
    
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        _uiManager = manager;
        _buildingPanel = manager.GetPanel<BuildingPanel>();
        
        var titleData = App.GetData<TitleData>();
        foreach (var view in _stateViews)
        {
            view.Initialize(QuestManager, this, titleData);
        }
        
        _guideBtn.onClick.AddListener(Guide);
    }

    public override void OpenPanel()
    {
        base.OpenPanel();  

        _infoUpdater.UpdateQuest(SelectedQuest);
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

    private void Guide()
    {
        _uiManager.PopAllPanels();
        if (SelectedQuest.Clear[0].Type == ClearType.Build)
        {
            _buildingPanel.OpenPanel();
        }
    }
}
