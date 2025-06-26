using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestInfo : MonoBehaviour
{
    public int QuestType => _questData.Type;

    [SerializeField] private Button _button;
    
    [SerializeField] private TextMeshProUGUI _titleTMP;
    [SerializeField] private TextMeshProUGUI _descTMP;
 
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
        
        _button.onClick.AddListener(()=>
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
        _questData = _titleData.Quest[data.QuestID];
        _detailData = data;
        
        var lastUnderscore = data.SubName.LastIndexOf('_'); // _SUB
        var secondLastUnderscore = data.SubName.LastIndexOf('_', lastUnderscore - 1); // _01

        var result = data.SubName.Substring(0, secondLastUnderscore);
        _titleTMP.text = _titleData.GetString(result);

        _descTMP.text = _titleData.GetString(_detailData.ClearDesc1);
    }
}
