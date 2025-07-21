using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class QuestSubmissionView : MonoBehaviour
{
    [SerializeField] protected ItemInfoUpdater[] _infoUpdaters; 
    [SerializeField] protected Button _submitBtn;  
    
    protected QuestManager QuestManager;
    protected Quest Quest;
    
    public virtual void Initialize(QuestManager questManager, TitleData titleData)
    {
        QuestManager = questManager;

        _submitBtn.GetComponentInChildren<TextMeshProUGUI>()
            .SetText(titleData.GetString("STR_BUTTON_CONFIRM"));
        _submitBtn.onClick.AddListener(Submit);
    }

    public virtual void Setup(Quest quest)
    {
        Quest = quest;
    }
    
    protected abstract void Submit();
}