using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialStep_04 : TutorialStep
{
    [SerializeField] private GameObject _chiefMinimo;
    [SerializeField] private TutorialDialogue _dialogueBox;
    [SerializeField] private FocusPanel _focusPanel;
    [SerializeField] private GameObject _chiefMinimoTextObj;
    [SerializeField] private TextMeshProUGUI _chiefMinimoText;
    [SerializeField] private TutorialStep_06 _tutorialStep_06;
    
    private void Awake()
    {
        _chiefMinimoTextObj.SetActive(false);
    }
    
    protected override void OnStart()
    {
        _chiefMinimo.GetComponent<TutorialInteractable>().onClick += OnClickedChief;
        _chiefMinimoTextObj.SetActive(true);
        _chiefMinimoText.text = "......";
    }
    
    private void OnClickedChief()
    {
        _chiefMinimo.GetComponent<TutorialInteractable>().onClick -= OnClickedChief;
        
        _focusPanel.FocusOn(_chiefMinimo.transform.position,
            targetZoom: 1,
            duration: 2,
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
            "미니모는 사람들의 소원을 들어주는 별의 요정이야.",
            "의도한 것은 아니겠지만 너에게도 책임은 있으니깐",
            "우리 미니모들이 힘을 낼 수 있도록 도와줄래?",
            "우리도 너의 로켓 수리를 도와줄게",
            "고마워 우선 밭에서 작물을 수확해줘");
        
        _focusPanel.FocusOn(Vector3.zero,
            targetZoom: 4,
            duration: 2,
            onComplete: () =>
            {
                Cleanup();
                CompleteStep();
            },
            closeOnComplete: true);
    }

    public override void Cleanup()
    {
        StartCoroutine(_tutorialStep_06.MoveChief());
    }
}
