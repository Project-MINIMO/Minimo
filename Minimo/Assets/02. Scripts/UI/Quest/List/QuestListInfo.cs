using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestListInfo : MonoBehaviour
{
    [SerializeField] private Button _openBtn;
    
    [SerializeField] private TextMeshProUGUI _titleTMP;
    [SerializeField] private Image _iconImg;
    [SerializeField] private Sprite[] _sprites;
    
    private TitleData _titleData;
    
    private QuestData _questData;
    private DetailQuestData _detailData;
    
    private QuestConsPanel _consPanel;
    private QuestSubmissionPanel _submissionPanel;

    private void Awake()
    {
        _titleData = App.GetData<TitleData>();
        _consPanel = App.GetManager<UIManager>().GetPanel<QuestConsPanel>();
        _submissionPanel = App.GetManager<UIManager>().GetPanel<QuestSubmissionPanel>();
        
        _openBtn.onClick.AddListener(()=>
        {
            if (_questData.Type == 1)
            {
                _consPanel.OpenPanel(_detailData);
            }
            else
            {
                _submissionPanel.OpenPanel(_detailData);
            }
        });
    }

    public void Initialize(DetailQuestData data)
    {
        _questData = _titleData.Quest[data.ID / 10];
        _detailData = data;
        
        if (data.Type == (int)QuestType.Constellation)
        {
            var questTitle = _titleData.Quest[data.ID / 10].Name;
            _titleTMP.text = _titleData.GetString(questTitle);
        }
        else
        {
            _titleTMP.text = _titleData.GetString($"STR_QUEST_{data.Name}");
        }
        
        var randomNum = Random.Range(0, _sprites.Length);
        _iconImg.sprite = _sprites[randomNum];
    }
}