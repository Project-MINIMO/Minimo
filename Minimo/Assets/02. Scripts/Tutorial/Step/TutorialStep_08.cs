using System.Collections;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialStep_08 : TutorialStep
{
    [SerializeField] private GameObject _chiefMinimo;
    [SerializeField] private TutorialDialogue _dialogueBox;
    [SerializeField] private FocusPanel _focusPanel;
    [SerializeField] private GameObject _chiefMinimoTextObj;
    [SerializeField] private TextMeshProUGUI _chiefMinimoText;
    [SerializeField] private GameObject _buildingBtnObj;
    [SerializeField] private GameObject _buildingBtnHighlightObj;
    [SerializeField] private GameObject _buildingPanel;
    [SerializeField] private GameObject _buildingHighlightObj;
    [SerializeField] private Button _closeBtn;
    [SerializeField] private Button _cancelBtn;
    [SerializeField] private Button _deleteBtn;
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private GameObject _editPanel;
    [SerializeField] private EditManager _editManager;
    
    protected override void OnStart()
    {
        _chiefMinimoTextObj.SetActive(true);
        _chiefMinimo.GetComponent<TutorialInteractable>().onClick += OnClickedChief;
        _chiefMinimoText.text = "......";
    }
    
    private void OnClickedChief()
    {
        _chiefMinimo.GetComponent<TutorialInteractable>().onClick -= OnClickedChief;
        _focusPanel.FocusOn(_chiefMinimo.transform.position,
            targetZoom: 1,
            duration: 1.5f,
            onComplete: () =>
            {
                StartCoroutine(ConversationSequence());
            },
            closeOnComplete: false);
    }
    
    private IEnumerator ConversationSequence()
    {
        _chiefMinimoTextObj.SetActive(false);
        
        yield return _dialogueBox.Show(
            "아까 전 충돌로 인해 건물이 부서지거나 떠내려갔어",
            "마을을 다시 재건하는 것을 도와줘",
            "다양한 소원공방(건물)을 배치해줘");
        
        _focusPanel.FocusOn(Vector3.down,
            targetZoom: 4,
            duration: 1.5f,
            onComplete: () =>
            {
                StartCoroutine(ProgressQuest());
            },
            closeOnComplete: true);
    }
    
    private IEnumerator ProgressQuest()
    {
        _chiefMinimoText.text = "다양한 소원공방(건물)을 배치해줘";
        _chiefMinimoTextObj.SetActive(true);
        _buildingBtnObj.SetActive(true);
        _buildingBtnHighlightObj.SetActive(true);
        _closeBtn.enabled = false;
        _scrollRect.enabled = false;

        yield return new WaitUntil(() => _buildingPanel.activeInHierarchy);
        
        _chiefMinimoTextObj.SetActive(false);
        _buildingBtnHighlightObj.SetActive(false);
        _buildingHighlightObj.SetActive(true);
        
        yield return new WaitUntil(() => _editPanel.activeInHierarchy);

        _cancelBtn.enabled = false;
        _deleteBtn.enabled = false;
        _scrollRect.enabled = true;
        _buildingHighlightObj.SetActive(false);
        
        yield return new WaitUntil(() => _editManager.ActiveProduces.Any(x => x.BuildingData.ID == 1));
        
        _cancelBtn.enabled = true;
        _deleteBtn.enabled = true;
        _closeBtn.enabled = true;
        CompleteStep();
    }
  
    public override void Cleanup()
    {
        AccountInfo.Instance.Level.AddCount(100);
    }
}