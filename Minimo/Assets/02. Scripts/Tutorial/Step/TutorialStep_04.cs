using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialStep_04 : TutorialStep
{
    [SerializeField] private GameObject _chiefMinimo;
    [SerializeField] private TutorialDialogue _dialogueBox;
    [SerializeField] private FocusPanel _focusPanel;
    [SerializeField] private TutorialStep_06 _tutorialStep_06;

    protected override void OnStart()
    {
        _chiefMinimo.GetComponent<TutorialInteractable>().onClick += OnClickedChief;
        _dialogueBox.ShowQuest();
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
        _dialogueBox.Hide();
        
        yield return _dialogueBox.Show(
            "별이 사람들의 소원이라는 이야기를 들은 적이 있나?",
            "우리 미니모는 소원별 사이에서 태어나 사람들의 소원을 이루어주는 별의 요정이라네.",
            "하지만 마을이 이래서야 어떻게 사람들의 소원을 이뤄줄 수 있겠나?",
            "고의는 아니었겠네만은... 자네에게 분명히 책임이 있네",
            "우리 미니모들이 사람들의 소원을 이룰 수 있도록 도와주게.",
            "그러면 우리도 자네의 로켓 수리를 도와주겠네.",
            "고맙네. 우선 밭에서 작물을 수확해주게.");
        
        _tutorialStep_06.MoveChief();
        
        _focusPanel.FocusOn(Vector3.zero,
            targetZoom: 4,
            duration: 1.5f,
            onComplete: () =>
            {
                Cleanup();
                CompleteStep();
            },
            closeOnComplete: true);
    }

    public override void Cleanup()
    {

    }
}
