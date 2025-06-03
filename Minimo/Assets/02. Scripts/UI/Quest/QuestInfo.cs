using UnityEngine;
using TMPro;

public class QuestInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleTMP;
    [SerializeField] private TextMeshProUGUI _desc1TMP;
    [SerializeField] private TextMeshProUGUI _desc2TMP;
    [SerializeField] private TextMeshProUGUI _desc3TMP;
    
    private TitleData _titleData;
    
    private QuestData _questData;
    private DetailQuestData _detailData;

    private void Awake()
    {
        _titleData = App.GetData<TitleData>();
    }

    public void Initialize(QuestData data)
    {
        _questData = data;
        _detailData = _titleData.DetailQuest[data.ID];
        
        _titleTMP.text = _titleData.GetString(data.Name);

        if (_detailData.ClearDesc1 != "-1" || string.IsNullOrEmpty(_detailData.ClearDesc1))
        {
            _desc1TMP.text = _titleData.GetString(_detailData.ClearDesc1);
        }
        if (_detailData.ClearDesc2 != "-1" || string.IsNullOrEmpty(_detailData.ClearDesc2))
        {
            _desc2TMP.text = _titleData.GetString(_detailData.ClearDesc2);
        }
        if (_detailData.ClearDesc3 != "-1" || string.IsNullOrEmpty(_detailData.ClearDesc3))
        {
            _desc3TMP.text = _titleData.GetString(_detailData.ClearDesc3);
        }
    }
}
