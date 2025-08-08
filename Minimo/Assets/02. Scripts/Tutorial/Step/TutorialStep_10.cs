using System.Collections;
using UnityEngine;

public class TutorialStep_10 : TutorialStep
{
    [SerializeField] private GameObject _chiefMinimo;
    [SerializeField] private TutorialDialogue _dialogueBox;
    [SerializeField] private FocusPanel _focusPanel;
    [SerializeField] private ProduceManager _produceManager;
    [SerializeField] private GameObject _secondaryPanel;
    [SerializeField] private GameObject _secondaryGuide;
    
    private Animator _animator;
    
    protected override void OnStart()
    {
        _chiefMinimo.GetComponent<TutorialInteractable>().onClick += OnClickedChief;
        _dialogueBox.ShowQuest();
        _animator = _chiefMinimo.GetComponentInChildren<Animator>(true);
    }
    
    private void OnClickedChief()
    {
        _chiefMinimo.GetComponent<TutorialInteractable>().onClick -= OnClickedChief;
        _dialogueBox.Hide();
        
        _focusPanel.FocusOn(_chiefMinimo.transform.position + Vector3.up * 0.5f,
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
        _animator.SetBool("IsTalk", true);
        yield return _dialogueBox.Show(
            "반죽 제조기도 배치해주게.",
            "만약 배치할 곳이 부족하다면 타일을 이용하여 마을을 넓혀주게.");
        _animator.SetBool("IsTalk", false);
        _dialogueBox.Hide();
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
        yield return new WaitUntil(() => _produceManager.CurrentObject != null 
                                         && _produceManager.CurrentObject.BuildingData.ID == 2);
        yield return new WaitUntil(() => _secondaryPanel.activeSelf);
        _secondaryGuide.SetActive(true);
     
        CompleteStep();
    }
  
    public override void Cleanup()
    {
        
    }
}