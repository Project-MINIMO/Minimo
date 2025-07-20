#nullable enable
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestInfoUpdater : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _questNameTMP;
    [SerializeField] private TextMeshProUGUI _questDescriptionTMP;
    [SerializeField] private TextMeshProUGUI _questConditionTMP;
    [SerializeField] private Image _iconImg;

    public void UpdateQuest(Quest quest)
    {
        _questNameTMP?.SetText(quest.Name);
        _questDescriptionTMP?.SetText(quest.Description);
        _questConditionTMP?.SetText(quest.ClearDescription);

        if (!_iconImg) return;
        _iconImg.sprite = quest.Icon;
        _iconImg.gameObject.SetActive(true);
    }
    
    public void UpdateQuestGroup(Quest quest)
    {
        _questNameTMP?.SetText(quest.Group.Name);

        if (!_iconImg) return;
        _iconImg.sprite = quest.Icon;
        _iconImg.gameObject.SetActive(true);
    }
}
