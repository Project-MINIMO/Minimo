using System.Collections;
using DG.Tweening;
using UnityEngine;

public class TutorialStep_07 : TutorialStep
{
    [SerializeField] private GameObject _primaryPanel;
    [SerializeField] private SpriteRenderer _highlight;
    [SerializeField] private SpriteRenderer _highlight2;
    [SerializeField] private TutorialDialogue _dialogueBox;
    
    private Item _wheatItem;
    
    private void Start()
    {
        _wheatItem = AccountInfo.Instance.Items[3];
    }
    
    protected override void OnStart()
    {
        StartCoroutine(ProgressQuest());
        _dialogueBox.Show("밭에 별곡을 심고, 다시 수확해주게.");
    }
   
    private IEnumerator ProgressQuest()
    {
        _highlight.gameObject.SetActive(true);
        _highlight.DOFade(0.7f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        _highlight2.gameObject.SetActive(true);
        _highlight2.DOFade(0.7f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        
        yield return new WaitUntil(() => _primaryPanel.activeSelf);
        
        _highlight.DOKill();
        _highlight2.DOKill();
        _highlight.gameObject.SetActive(false);
        _highlight2.gameObject.SetActive(false);
        
        yield return new WaitUntil(() => _wheatItem.Count >= 3);
        _dialogueBox.Hide();
        
        CompleteStep();
    }

    public override void Cleanup()
    {
        AccountInfo.Instance.Level.AddCount(100);
    }
}
