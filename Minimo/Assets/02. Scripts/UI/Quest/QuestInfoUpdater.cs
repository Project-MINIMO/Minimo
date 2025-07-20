#nullable enable
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestInfoUpdater : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI? _questNameTMP;
    [SerializeField] private TextMeshProUGUI? _questNameTMP2;
    [SerializeField] private TextMeshProUGUI? _questDescriptionTMP;
    [SerializeField] private TextMeshProUGUI? _questConditionTMP;
    [SerializeField] private Image? _iconImg;

    public void UpdateQuest(Quest quest)
    {
        _questNameTMP?.SetText(quest.Name);
        _questNameTMP2?.SetText(quest.Name);
        _questDescriptionTMP?.SetText(quest.Description);
        _questConditionTMP?.SetText(quest.ClearDescription);

        if (_iconImg == null) return;
        _iconImg.sprite = quest.Group?.Icon;
        _iconImg.gameObject.SetActive(true);
    }
    
    public void UpdateQuestGroup(Quest quest)
    {
        _questNameTMP?.SetText(quest.Group.Name);

        if (_iconImg == null) return;
        _iconImg.sprite = quest.Group?.Icon;
        _iconImg.gameObject.SetActive(true);
    }
}
