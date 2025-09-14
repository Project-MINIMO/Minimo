using System.Collections;
using UnityEngine;

public class TutorialStep_13: TutorialStep
{
    [SerializeField] private GameObject _chiefMinimo;
    [SerializeField] private TutorialDialogue _dialogueBox;
    [SerializeField] private FocusPanel _focusPanel;
    [SerializeField] private VisitMinimoSpawner _spawner;
    
    private Animator _animator;
    private VisitMinimoObject _visitMinimo;
    
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
        
        StartCoroutine(ConversationSequence());
    }
    
    private IEnumerator ConversationSequence()
    {
        _visitMinimo = _spawner.SpawnTutoriMinimo();
        
        yield return new WaitForSeconds(3f);
        
        _focusPanel.FocusOn(_visitMinimo.transform.position + Vector3.up * 0.5f,
            targetZoom: 1,
            duration: 1f,
            onComplete: null,
            closeOnComplete: false);
        
        yield return new WaitForSeconds(3f);
        
        _focusPanel.FocusOn(_chiefMinimo.transform.position + Vector3.up * 0.5f,
            targetZoom: 1,
            duration: 1.5f,
            onComplete: () =>
            {
                StartCoroutine(ConversationSequence2());
            },
            closeOnComplete: false);
    }

    private IEnumerator ConversationSequence2()
    {
        _animator.SetBool("IsTalk", true);
        yield return _dialogueBox.Show(
            "저 친구는 다른 마을에서 온 미니모일세",
            "때때로 서로 필요한 음식들을 교환하고있지",
            "이런 상황이지만 그들이 필요한 물건을 만들어줄 수 있겠나?");
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
        _dialogueBox.Show("저 친구가 원하는 물건을 제작하여 건네주게.");
        yield return new WaitUntil(() => _visitMinimo.CurrentState == VisitMinimoState.Hide);
        
        _dialogueBox.Show("고맙네. 이제 우리를 도와 마을을 재건해주게.");

        yield return new WaitForSeconds(3);

        _dialogueBox.Hide();
        CompleteStep();
    }
  
    public override void Cleanup()
    {
        
    }
}