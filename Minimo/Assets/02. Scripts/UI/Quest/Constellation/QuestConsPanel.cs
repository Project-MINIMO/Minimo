using System.Linq;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestConsPanel : UIBase
{
    [SerializeField] private QuestUIGrouper _grouper;
    [SerializeField] private Button _closeBtn;

    [SerializeField] private TextMeshProUGUI _titleTMP;
    [SerializeField] private Image _iconImg;
    [SerializeField] private Sprite[] _sprites;

    private QuestConsInfo[] _questInfos;
    private TitleData _titleData;

    public override void Initialize()
    {
        _titleData = App.GetData<TitleData>();
        _questInfos = GetComponentsInChildren<QuestConsInfo>();

        _closeBtn.onClick.AddListener(() =>
        {
            _grouper.OpenListPanel();
            ClosePanel();
        });
    }

    public void OpenPanel(DetailQuestData questData)
    {
        _grouper.CloseAllExcept(this);
        
        base.OpenPanel();

        var quests = _titleData.DetailQuest.Values.Where(x => x.ID / 10 == questData.ID / 10).ToList();
        
        var i = 0;
        
        for (; i < quests.Count; i++)
        {
            _questInfos[i].gameObject.SetActive(true);
            _questInfos[i].Initialize(quests[i]);
        }

        for (; i < _questInfos.Length; i++)
        {
            _questInfos[i].gameObject.SetActive(false);
        }
        
        var questTitle = _titleData.Quest[questData.ID / 10].Name;
        _titleTMP.text = _titleData.GetString(questTitle);
        
        var randomNum = Random.Range(0, _sprites.Length);
        _iconImg.sprite = _sprites[randomNum];
    }
}
