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
        _dialogueBox.onClick += OnClickedChief;
        _dialogueBox.ShowQuest();
        _animator = _chiefMinimo.GetComponentInChildren<Animator>(true);
    }
    
    private void OnClickedChief()
    {
        _chiefMinimo.GetComponent<TutorialInteractable>().onClick -= OnClickedChief;
        _dialogueBox.onClick -= OnClickedChief;
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
            "이제 반죽 제조기도 배치해주게.",
            "그리고 음식을 만들 반죽을 만들어주게나.");
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
        _dialogueBox.Show("반죽 제조기도 배치해주게.");
        yield return new WaitUntil(() => _produceManager.CurrentObject != null 
                                         && _produceManager.CurrentObject.BuildingData.ID == 2);
        _dialogueBox.Show("음식을 만들 반죽을 만들어주게나.");
        yield return new WaitUntil(() => _secondaryPanel.activeSelf);
        _secondaryGuide.SetActive(true);
        _dialogueBox.Hide();
     
        CompleteStep();
    }
  
    public override void Cleanup()
    {
        
    }
}