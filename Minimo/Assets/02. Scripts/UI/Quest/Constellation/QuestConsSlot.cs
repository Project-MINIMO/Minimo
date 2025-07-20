using UnityEngine;
using TMPro;

public class QuestConsSlot : QuestSlot
{
    [SerializeField] private GameObject[] _stateObjs;
    
    [SerializeField] private TextMeshProUGUI _completeDayTMP;
    [SerializeField] private TextMeshProUGUI _lockDescriptionTMP;

    private TitleData _titleData;
    
    protected override void Awake()
    {
        base.Awake();
        
        _titleData = App.GetData<TitleData>();
    }

    public void Initialize(QuestState state, Quest data)
    {
        base.Initialize(data);
        
        SetLockDescription();

        for (var i = 0; i < _stateObjs.Length; i++)
        {
            _stateObjs[i].SetActive(i == (int)state);
        }
    }

    private void SetLockDescription()
    {
        _lockDescriptionTMP.text = QuestData.OpenLevel <= AccountInfo.Instance.level ? 
            _titleData.GetString("STR_QUEST_ALARM_PREQUEST") 
            : _titleData.GetFormatString("STR_QUEST_ALARM_LEVEL", AccountInfo.Instance.level.ToString());
    }
}
