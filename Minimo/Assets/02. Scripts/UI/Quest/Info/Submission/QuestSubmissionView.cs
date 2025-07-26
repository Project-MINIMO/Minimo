using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class QuestSubmissionView : MonoBehaviour
{
    [SerializeField] protected ItemInfoUpdater[] _infoUpdaters; 
    [SerializeField] protected Button _submitBtn;  
    
    protected QuestManager QuestManager;
    protected Quest Quest;
    private QuestSubmissionPanel _submissionPanel;
    
    public virtual void Initialize(QuestManager questManager, QuestSubmissionPanel submissionPanel, TitleData titleData)
    {
        QuestManager = questManager;
        _submissionPanel = submissionPanel;

        _submitBtn.GetComponentInChildren<TextMeshProUGUI>()
            .SetText(titleData.GetString("STR_BUTTON_CONFIRM"));
        _submitBtn.onClick.AddListener(Submit);
    }

    public virtual void Setup(Quest quest)
    {
        Quest = quest;
    }

    protected virtual void Submit()
    {
        _submissionPanel.ClosePanel();
    }
}