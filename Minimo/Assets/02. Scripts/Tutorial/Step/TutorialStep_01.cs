using System.Collections;
using UnityEngine;

public class TutorialStep_01 : TutorialStep
{
    [SerializeField] private GameObject _chiefMinimo;
    [SerializeField] private TutorialDialogue _dialogueBox;
    [SerializeField] private FocusPanel _focusPanel;

    protected override void OnStart()
    {
        _focusPanel.FocusOn(_chiefMinimo.transform.position);
        _chiefMinimo.GetComponent<TutorialInteractable>().onClick += OnClickedChief;
    }

    private void OnClickedChief()
    {
        StartCoroutine(ConversationSequence());
    }

    private IEnumerator ConversationSequence()
    {
        yield return _dialogueBox.Show("방금 무슨일이 일어난거지?");
        yield return _dialogueBox.Show("당신은... (침묵)");
        yield return _dialogueBox.Show("일단 마을 미니모들을 구해줘야해");
        yield return _dialogueBox.Show("나 대신 도와줄 수 있어?");
        
        Cleanup();
        CompleteStep();
    }

    public override void Cleanup()
    {
        _chiefMinimo.GetComponent<TutorialInteractable>().onClick -= OnClickedChief;
    }
}
