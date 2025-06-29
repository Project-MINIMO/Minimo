using System.Linq;

using UnityEngine;
using UnityEngine.UI;

public class QuestConsPanel : UIBase
{
    [SerializeField] private Button _closeBtn;
    
    private SubQuestInfo[] _subQuests;
    private TitleData _titleData;

    public override void Initialize()
    {
        _titleData = App.GetData<TitleData>();
        _subQuests = GetComponentsInChildren<SubQuestInfo>();

        _closeBtn.onClick.AddListener(ClosePanel);
    }

    public void OpenPanel(DetailQuestData questData)
    {
        base.OpenPanel();

        var quests = _titleData.DetailQuest.Values.Where(x => x.QuestID == questData.QuestID).ToList();
        
        var i = 0;
        
        for (; i < quests.Count; i++)
        {
            _subQuests[i].gameObject.SetActive(true);
            _subQuests[i].Initialize(quests[i]);
        }

        for (; i < _subQuests.Length; i++)
        {
            _subQuests[i].gameObject.SetActive(false);
        }
    }
}
