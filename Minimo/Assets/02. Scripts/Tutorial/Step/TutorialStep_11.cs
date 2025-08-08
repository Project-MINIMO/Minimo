using System.Collections;
using UnityEngine;

public class TutorialStep_11 : TutorialStep
{
    [SerializeField] private GameObject _chiefMinimo;
    [SerializeField] private TutorialDialogue _dialogueBox;
    [SerializeField] private FocusPanel _focusPanel;
    [SerializeField] private ProduceManager _produceManager;
    [SerializeField] private LevelPanel _levelPanel;
    [SerializeField] private GameObject _indicatorHandlers;
    [SerializeField] private StarManager _starManager;
    
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
            "미니모들이 마을로 쉽게 찾을 수 있도록 소원별을 배치해주게.",
            "사람들의 소원을 이뤄주거나, 마을을 성장하여 얻을 수 있네.",
            "소원별이 서로 가까이 있으면 더욱 미니모에게 잘 전해질 수 있을걸세.");
        _animator.SetBool("IsTalk", false);
        _dialogueBox.Hide();
        StartCoroutine(ProgressQuest());
    }
    
    private IEnumerator ProgressQuest()
    {
        _levelPanel.FocusStar();
        _indicatorHandlers.SetActive(true);
        
        yield return new WaitUntil(() => _starManager.IsConnected);
     
        CompleteStep();
    }
  
    public override void Cleanup()
    {
        
    }
}