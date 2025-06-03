using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestConfirmPanel : UIBase
{
    [SerializeField] private TextMeshProUGUI _titleTMP;
    [SerializeField] private TextMeshProUGUI _descTMP;
    
    [SerializeField] private Button _confirmBtn;
    [SerializeField] private Button _denyBtn;
    
    private QuestData _currentQuest;
    
    private TitleData _titleData;
    private QuestManager _questManager;
    
    public override void Initialize()
    {
        _titleData = App.GetData<TitleData>();
        _questManager = App.GetManager<QuestManager>();

        _confirmBtn.onClick.AddListener(() => _questManager.AddQuest(_currentQuest));
        _denyBtn.onClick.AddListener(ClosePanel);
    }

    public void OpenPanel(QuestData data)
    {
        base.OpenPanel();

        _currentQuest = data;
        
        _titleTMP.text = _titleData.GetString(data.Name);
    }
}
