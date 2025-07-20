using UnityEngine;
using TMPro;

public class QuestConsSlot : QuestSlot
{
    [SerializeField] private GameObject[] _stateObjs;
    
    [SerializeField] private TextMeshProUGUI _completeDayTMP;
    [SerializeField] private TextMeshProUGUI _lockDescriptionTMP;
    
    private string _levelLockDescription;
    private string _preQuestLockDescription;
    
    protected override void Awake()
    {
        base.Awake();
        
        var titleData = App.GetData<TitleData>();
        _levelLockDescription = titleData.GetString("STR_QUEST_ALARM_LEVEL");
        _preQuestLockDescription = titleData.GetString("STR_QUEST_ALARM_PREQUEST");
    }

    public void Initialize(QuestState state, Quest data)
    {
        base.Initialize(data);

        if (state == QuestState.Locked)
        {
            SetLockDescription();
        }
        
        for (var i = 0; i < _stateObjs.Length; i++)
        {
            _stateObjs[i].SetActive(i == (int)state);
        }
    }

    private void SetLockDescription()
    {
        _lockDescriptionTMP.text = QuestData.OpenLevel <= AccountInfo.Instance.level
            ? _preQuestLockDescription
            : string.Format(_levelLockDescription, AccountInfo.Instance.level);
    }
}
