using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestInfo : MonoBehaviour
{
    public int QuestType => _questData.Type;

    [SerializeField] private Button _button;
    
    [SerializeField] private TextMeshProUGUI _titleTMP;
    [SerializeField] private TextMeshProUGUI _desc1TMP;
    [SerializeField] private TextMeshProUGUI _desc2TMP;
    [SerializeField] private TextMeshProUGUI _desc3TMP;
 
    private TitleData _titleData;
    
    private QuestData _questData;
    private DetailQuestData _detailData;
    
    private QuestPanel _questPanel;

    private void Awake()
    {
        _titleData = App.GetData<TitleData>();
        _questPanel = App.GetManager<UIManager>().GetPanel<QuestPanel>();
        
        _button.onClick.AddListener(()=>
        {
            if (_questData.Type == 1)
            {
                _questPanel.OpenCons(_detailData);
            }
            else
            {
                _questPanel.OpenSide(_detailData);
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

        if (_detailData.ClearDesc1 == "-1" || string.IsNullOrEmpty(_detailData.ClearDesc1))
        {
            _desc1TMP.gameObject.SetActive(false);
        }
        else
        {
            _desc1TMP.text = _titleData.GetString(_detailData.ClearDesc1);
            _desc1TMP.gameObject.SetActive(true);
        }
        
        if (_detailData.ClearDesc2 == "-1" || string.IsNullOrEmpty(_detailData.ClearDesc2))
        {
            _desc2TMP.gameObject.SetActive(false);
        }
        else
        {
            _desc2TMP.text = _titleData.GetString(_detailData.ClearDesc2);
            _desc2TMP.gameObject.SetActive(true);
        }
        
        if (_detailData.ClearDesc3 == "-1" || string.IsNullOrEmpty(_detailData.ClearDesc3))
        {
            _desc3TMP.gameObject.SetActive(false);
        }
        else
        {
            _desc3TMP.text = _titleData.GetString(_detailData.ClearDesc3); 
            _desc3TMP.gameObject.SetActive(true);
        }
    }
}
