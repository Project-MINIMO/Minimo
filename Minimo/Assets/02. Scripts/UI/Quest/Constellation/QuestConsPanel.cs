using System.Linq;

using UnityEngine;
using UnityEngine.UI;

public class QuestConsPanel : QuestPanel<QuestConsSlot>
{
    public override bool IsUseBlur => true;
    
    [SerializeField] private Button _closeBtn;
    [SerializeField] private QuestTransitioner _transitioner;
    [SerializeField] private QuestInfoUpdater _infoUpdater;

    private TitleData _titleData;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        _titleData = App.GetData<TitleData>();
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
        
        SetupQuestInfos(_questManager.CurrentQuest);
    }

    private void SetupQuestInfos(Quest questData)
    {
        var quests = _titleData.Quest.Values
            .Where(x => x.Group.ID == questData.Group.ID)
            .ToList();

        var i = 0;
        var activeIndex = int.MaxValue;
        
        for (; i < quests.Count; i++)
        {
            _slots[i].gameObject.SetActive(true);
            if (quests[i] == questData)
            {
                activeIndex = i;
            }
            _slots[i].Initialize(GetQuestState(i, activeIndex), quests[i]);
        }
        
        for (; i < _slots.Count; i++)
        {
            _slots[i].gameObject.SetActive(false);
        }
        
        _infoUpdater.UpdateQuestGroup(questData);
    }

    private QuestState GetQuestState(int index, int activeIndex) => (index, activeIndex) switch
    {
        var (i, a) when i < a => QuestState.Completed,
        var (i, a) when i == a => QuestState.InProgress,
        var (i, a) when i > a => QuestState.Locked,
    };
}
