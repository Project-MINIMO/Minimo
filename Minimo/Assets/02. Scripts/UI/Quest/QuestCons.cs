using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class QuestCons : MonoBehaviour
{
    [SerializeField] private Button _closeBtn;
    
    private SubQuestInfo[] _subQuests;
    private TitleData _titleData;

    private void Awake()
    {
        _titleData = App.GetData<TitleData>();
        _subQuests = GetComponentsInChildren<SubQuestInfo>();

        _closeBtn.onClick.AddListener(() => gameObject.SetActive(false));
    }

    public void ShowUI(DetailQuestData questData)
    {
        gameObject.SetActive(true);

        var quests = _titleData.DetailQuest.Values.Where(x => x.QuestID == questData.QuestID).ToList();
        
        var i = 0;
        
        for (; i < quests.Count; i++)
        {
            _subQuests[i].gameObject.SetActive(true);
            _subQuests[i].Initialize(quests[i]);
        }

        for (; i < _subQuests.Length; i++)
        {
            _subQuests[i].gameObject.SetActive(false);
        }
    }
}
