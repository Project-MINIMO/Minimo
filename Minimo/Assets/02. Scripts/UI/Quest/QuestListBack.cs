using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestListBack : MonoBehaviour
{
    [SerializeField] private Transform _questParent;
    [SerializeField] private GameObject _questPrefab;
    [SerializeField] private ScrollRect _scrollRect;
    
    [SerializeField] private TextMeshProUGUI _buttonTMP;
    [SerializeField] private TextMeshProUGUI _titleTMP;
    
    private QuestManager _questManager;

    private int _questIndex;
    
    public void Initialize(int index)
    {
        _questManager = App.GetManager<QuestManager>();

        _questIndex = index;
    }

    private void OnEnable()
    {
        if (_questManager == null) return;
        
        _scrollRect.verticalNormalizedPosition = 1;
        
        UpdateQuest();
    }

    public void UpdateQuest()
    {
        var quests = _questManager.ActiveQuests.Where(x => CheckQuestType(x.Type)).ToList();

        var existingInfos = GetComponentsInChildren<QuestListInfo>(true);
        
        var i = 0;
        
        for (; i < quests.Count; i++)
        {
            var questInfo = i < existingInfos.Length 
                ? existingInfos[i] 
                : Instantiate(_questPrefab, _questParent).GetComponent<QuestListInfo>();

            questInfo.gameObject.SetActive(true);
            questInfo.Initialize(quests[i]);
        }

        for (; i < existingInfos.Length; i++)
        {
            existingInfos[i].gameObject.SetActive(false);
        }
    }

    private bool CheckQuestType(int questType) => questType switch
    {
        0 => _questIndex == 0,
        1 => _questIndex == 0,
        2 => _questIndex == 1,
        _ => _questIndex == 2
    };
}
