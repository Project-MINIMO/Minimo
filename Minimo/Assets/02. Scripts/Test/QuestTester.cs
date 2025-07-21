using System.Linq;
using UnityEngine;

public class QuestTester : MonoBehaviour
{
    [Tooltip("추가하거나 제거할 Quest의 ID")]
    public int questId;

    private QuestManager _qm;
    private TitleData    _titleData;

    private void Awake()
    {
        if (!Application.isPlaying) return;
        _qm = App.GetManager<QuestManager>();
        _titleData = App.GetData<TitleData>();
    }

    /// <summary>
    /// 컨텍스트 메뉴에서 호출: 이 컴포넌트 우측 ⋮ 클릭 → Add Quest
    /// </summary>
    [ContextMenu("Add Quest")]
    public void AddQuest()
    {
        if (!_titleData.Quest.TryGetValue(questId, out var quest))
        {
            Debug.LogError($"[Tester] QuestData({questId})가 없음");
            return;
        }
        
        _qm.AddQuest(quest);
        Debug.Log($"[Tester] Quest({questId}) added");
    }

    [ContextMenu("Remove Quest")]
    public void RemoveQuest()
    {
        var quest = _qm.ActiveQuests.FirstOrDefault(q => q.ID == questId);
        if (quest == null)
        {
            Debug.LogWarning($"[Tester] ActiveQuests에 Quest({questId})가 없음");
            return;
        }

        _qm.RemoveQuest(quest);
        Debug.Log($"[Tester] Quest({questId}) removed");
    }
}