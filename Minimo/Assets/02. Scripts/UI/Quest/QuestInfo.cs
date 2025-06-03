using System.Linq;

using UnityEngine;
using TMPro;

public class QuestInfo : MonoBehaviour
{
    public int QuestType => _questData.Type;
    
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
        Debug.Log(data.Name);
        _questData = data;
        _detailData = _titleData.DetailQuest.Values.FirstOrDefault(x => x.QuestID == data.ID);
        
        _titleTMP.text = _titleData.GetString(data.Name);

        if (_detailData.ClearDesc1 != "-1" || !string.IsNullOrEmpty(_detailData.ClearDesc1))
        {
            _desc1TMP.text = _titleData.GetString(_detailData.ClearDesc1);
            _desc1TMP.gameObject.SetActive(true);
        }
        else
        {
            _desc1TMP.gameObject.SetActive(false);
        }
        
        if (_detailData.ClearDesc2 != "-1" || !string.IsNullOrEmpty(_detailData.ClearDesc2))
        {
            _desc2TMP.text = _titleData.GetString(_detailData.ClearDesc2);
            _desc2TMP.gameObject.SetActive(true);
        }
        else
        {
            _desc2TMP.gameObject.SetActive(false);
        }
        
        if (_detailData.ClearDesc3 != "-1" || !string.IsNullOrEmpty(_detailData.ClearDesc3))
        {
            _desc3TMP.text = _titleData.GetString(_detailData.ClearDesc3); 
            _desc3TMP.gameObject.SetActive(true);
        }
        else
        {
            _desc3TMP.gameObject.SetActive(false);
        }
    }
}
