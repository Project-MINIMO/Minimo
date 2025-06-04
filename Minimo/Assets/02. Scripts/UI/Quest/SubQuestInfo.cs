using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SubQuestInfo : MonoBehaviour
{
    [SerializeField] private Button _infoBtn;
    [SerializeField] private Image _iconImg;
    [SerializeField] private TextMeshProUGUI _titleTMP;

    [SerializeField] private GameObject _levelBack;
    [SerializeField] private GameObject _preQuestBack;

    private DetailQuestData _detailQuestData;
    private TitleData _titleData;
    private QuestManager _questManager;
    
    private void Awake()
    {
        _titleData = App.GetData<TitleData>();
        _questManager = App.GetManager<QuestManager>();
        
        _infoBtn.onClick.AddListener(OnClickInfoBtn);
    }

    public void Initialize(DetailQuestData questData)
    {
        _detailQuestData = questData;

        _titleTMP.text = _titleData.GetString(questData.SubName);
        _preQuestBack.SetActive(!IsUnlocked());
    }

    private void OnClickInfoBtn()
    {
        
    }
    
    private bool IsUnlocked()
    {
        if (_detailQuestData.PreQuestID == -1) return true;
        
        if (_questManager.ActiveQuests.Contains(_detailQuestData)) return true;

        var next = _questManager.ActiveQuests.FirstOrDefault(x => x.QuestID == _detailQuestData.QuestID);
        return next?.ID > _detailQuestData.ID;
    }
}
