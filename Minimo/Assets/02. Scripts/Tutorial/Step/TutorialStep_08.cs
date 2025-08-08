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
            "고맙네. 이걸로 배고픈 미니모를 위해 백미를 만들어줄 수 있겠군.",
            "그런데 백미제조기가 충돌로 인해 부서진 모양이로구만...",
            "백미제조기가 있어야 별곡으로부터 백미를 도정할 수 있다네.",
            "우선 백미제조기를 배치해주게.");
        
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
        _chiefMinimoText.text = "우선 백미제조기를 배치해주게.";
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