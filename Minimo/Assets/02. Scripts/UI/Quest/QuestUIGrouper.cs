using UnityEngine;

public class QuestUIGrouper : MonoBehaviour
{
    [SerializeField] private QuestSummaryPanel _summaryPanel;
    [SerializeField] private QuestListPanel _listPanel;
    [SerializeField] private QuestSubmissionPanel _submissionPanel;
    [SerializeField] private QuestConsPanel _consPanel;

    public void OpenSummaryPanel()
    {
        _summaryPanel.OpenPanel();
    }

    public void OpenListPanel()
    {
        _listPanel.OpenPanel();
    }
    
    public void CloseAllExcept(UIBase exceptPanel)
    {
        if (_summaryPanel != exceptPanel) _summaryPanel?.ClosePanel();
        if (_listPanel != exceptPanel) _listPanel?.ClosePanel();
        if (_submissionPanel != exceptPanel) _submissionPanel?.ClosePanel();
        if (_consPanel != exceptPanel) _consPanel?.ClosePanel();
    }
}
