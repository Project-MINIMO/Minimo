using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestConsInfo : MonoBehaviour
{
    private enum QuestState
    {
        Locked,
        InProgress,
        Completed,
    }
    
    [SerializeField] private Button _infoBtn;
    [SerializeField] private TextMeshProUGUI _titleTMP;

    [SerializeField] private GameObject _completeBack;
    [SerializeField] private TextMeshProUGUI _completeTitleTMP;
    [SerializeField] private TextMeshProUGUI _completeDescriptionTMP;
    [SerializeField] private TextMeshProUGUI _completeDayTMP;
    
    [SerializeField] private GameObject _lockBack;
    [SerializeField] private TextMeshProUGUI _lockDescriptionTMP;

    private DetailQuestData _detailQuestData;
    private TitleData _titleData;
    private QuestManager _questManager;
    private QuestSubmissionPanel _submissionPanel;
    
    private void Awake()
    {
        _titleData = App.GetData<TitleData>();
        _questManager = App.GetManager<QuestManager>();
        _submissionPanel = App.GetManager<UIManager>().GetPanel<QuestSubmissionPanel>();
        
        _infoBtn.onClick.AddListener(OnClickInfoBtn);
    }

    public void Initialize(DetailQuestData questData)
    {
        _detailQuestData = questData;

        _titleTMP.text = _titleData.GetString($"STR_QUEST_{questData.Name}");
        _completeTitleTMP.text = _titleData.GetString($"STR_QUEST_{questData.Name}");

        var state = GetQuestState();
        _infoBtn.gameObject.SetActive(state == QuestState.InProgress);
        _lockBack.SetActive(state == QuestState.Locked);
        _completeBack.SetActive(state == QuestState.Completed);

        if (state == QuestState.Completed)
        {
            SetCompleteDescription();
        }
    }

    private void OnClickInfoBtn()
    {
        _submissionPanel.OpenPanel(_detailQuestData);
    }
    
    private QuestState GetQuestState()
    {
        if (_detailQuestData.PreQuestID == -1) return QuestState.InProgress;
        if (_questManager.ActiveQuests.Contains(_detailQuestData)) return QuestState.InProgress;
        
        var next = _questManager.ActiveQuests.FirstOrDefault(x => x.ID / 10 == _detailQuestData.ID / 10);
        return next?.ID > _detailQuestData.ID ? QuestState.Completed : QuestState.Locked;
    }

    private void SetCompleteDescription()
    {
        _completeDescriptionTMP.text = _detailQuestData.OpenLevel <= AccountInfo.Instance.level ? 
            _titleData.GetString("STR_QUEST_ALARM_PREQUEST") 
            : _titleData.GetFormatString("STR_QUEST_ALARM_LEVEL", AccountInfo.Instance.level.ToString());
    }
}
