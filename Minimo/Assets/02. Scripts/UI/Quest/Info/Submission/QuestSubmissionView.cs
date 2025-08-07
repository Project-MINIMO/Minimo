using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class QuestSubmissionView : MonoBehaviour
{
    [SerializeField] protected QuestSubmissionSlot[] SubmissionSlots; 
    [SerializeField] protected Button SubmitBtn;  
    
    protected QuestManager QuestManager;
    protected Quest Quest;
    private QuestSubmissionPanel _submissionPanel;
    
    public virtual void Initialize(QuestManager questManager, QuestSubmissionPanel submissionPanel, TitleData titleData)
    {
        QuestManager = questManager;
        _submissionPanel = submissionPanel;

        SubmitBtn.GetComponentInChildren<TextMeshProUGUI>().SetText("제출하기");
        SubmitBtn.onClick.AddListener(Submit);
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