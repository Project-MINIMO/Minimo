using System.Collections;
using UnityEngine;

public class TutorialStep_12: TutorialStep
{
    [SerializeField] private GameObject _chiefMinimo;
    [SerializeField] private TutorialDialogue _dialogueBox;
    [SerializeField] private FocusPanel _focusPanel;
    [SerializeField] private MinimoSpawner _minimoSpawner;
    
    private Animator _animator;
    private MinimoObject _minimoObject;
    
    protected override void OnStart()
    {
        _chiefMinimo.GetComponent<TutorialInteractable>().onClick += OnClickedChief;
        _dialogueBox.onClick += OnClickedChief;
        _dialogueBox.ShowQuest();
        _animator = _chiefMinimo.GetComponentInChildren<Animator>(true);
        _minimoObject = _minimoSpawner.SpawnTutorialSwinMinimo();
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
            "저기 떠다니는 미니모가 보이네!",
            "어서 마을로 데려오게!");
        _animator.SetBool("IsTalk", false);
        _dialogueBox.Hide();
        _focusPanel.FocusOn(_minimoObject.transform.position + Vector3.up * 0.5f,
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
        _dialogueBox.Show("얼른 떠도는 미니모를 클릭해 마을로 불러들이게.");
        yield return new WaitUntil(() => _minimoObject.CurrentState == MinimoState.Idle);
        _dialogueBox.Hide();
        CompleteStep();
    }
  
    public override void Cleanup()
    {
        
    }
}