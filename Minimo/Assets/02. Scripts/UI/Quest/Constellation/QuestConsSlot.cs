using UnityEngine;
using TMPro;

public class QuestConsSlot : QuestSlot
{
    [SerializeField] private GameObject[] _stateObjs;

    [SerializeField] private GameObject _completeBack;
    [SerializeField] private TextMeshProUGUI _completeTitleTMP;
    [SerializeField] private TextMeshProUGUI _completeDescriptionTMP;
    [SerializeField] private TextMeshProUGUI _completeDayTMP;
    
    [SerializeField] private GameObject _lockBack;
    [SerializeField] private TextMeshProUGUI _lockDescriptionTMP;

    private TitleData _titleData;
    
    protected override void Awake()
    {
        _titleData = App.GetData<TitleData>();
    }

    public void Initialize(QuestState state, Quest data)
    {
        base.Initialize(data);
        
        _completeTitleTMP.text = data.Name;
        SetCompleteDescription();

        for (var i = 0; i < _stateObjs.Length; i++)
        {
            _stateObjs[i].SetActive(i == (int)state);
        }
    }

    private void SetCompleteDescription()
    {
        _completeDescriptionTMP.text = QuestData.OpenLevel <= AccountInfo.Instance.level ? 
            _titleData.GetString("STR_QUEST_ALARM_PREQUEST") 
            : _titleData.GetFormatString("STR_QUEST_ALARM_LEVEL", AccountInfo.Instance.level.ToString());
    }
}
