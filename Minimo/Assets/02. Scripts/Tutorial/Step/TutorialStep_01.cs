using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class TutorialStep_01 : TutorialStep
{
    [SerializeField] private GameObject _chiefMinimo;
    [SerializeField] private TutorialDialogue _dialogueBox;
    [SerializeField] private FocusPanel _focusPanel;
    [SerializeField] private GameObject _tileBtnObj;
    [SerializeField] private GameObject _tileBtnHighlightObj;
    [SerializeField] private GameObject _tileHighlightObj;
    [SerializeField] private TilePanel _tilePanel;
    [SerializeField] private Tilemap _villageTilemap;
    [SerializeField] private GameObject _tileSelectObj;
    [SerializeField] private Button _closeBtn;
    [SerializeField] private Button _cancelBtn;
    [SerializeField] private Button _confirmBtn;
    [SerializeField] private GameObject _confirmBtnObj;
    [SerializeField] private GameObject _blockTileObj;
    [SerializeField] private GameObject _blockTileObj2;
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private TutorialStep_05 _tutorialStep_05;
    [SerializeField] private SpriteRenderer _highlight;
    [SerializeField] private SpriteRenderer _highlight2;

    private bool _isConfirmed;
    private Animator _animator;
    
    protected override void OnStart()
    {
        _tileBtnHighlightObj.SetActive(false);
        _dialogueBox.ShowQuest();
        _chiefMinimo.GetComponent<TutorialInteractable>().onClick += OnClickedChief;
        _confirmBtn.onClick.AddListener(() => _isConfirmed = true);
        _animator = _chiefMinimo.GetComponentInChildren<Animator>(true);
        _animator.SetBool("IsLay", true);
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
        _tutorialStep_05.SpawnFarm();
        _animator.SetBool("IsLay", false);
        
        yield return new WaitForSeconds(2.3f);
        
        _animator.SetBool("IsTalk", true);
        yield return _dialogueBox.Show(
            "으으...방금 무슨일이 일어난거지?",
            "로켓이 날아왔고... 그리고... 행성이 부서졌잖아!",
            "자네는......",
            "......책임은 나중에 물을 테니 다른 미니모들을 구해주게",
            "자네의 도움이 필요하네.");
        _animator.SetBool("IsTalk", false);
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
        _dialogueBox.Show("타일을 설치해서 미니모가 무사한지 확인해주게");
        _tileBtnObj.SetActive(true);
        _tileBtnHighlightObj.SetActive(true);
        _closeBtn.enabled = false;
        _cancelBtn.enabled = false;
        _confirmBtn.enabled = false;
        _scrollRect.enabled = false;

        yield return new WaitUntil(() => _tilePanel.gameObject.activeInHierarchy);
        _dialogueBox.Hide();
        _tileBtnHighlightObj.SetActive(false);
        _tileHighlightObj.SetActive(true);
        
        yield return new WaitUntil(() => _tileSelectObj.activeInHierarchy);
        
        _scrollRect.enabled = true;
        _tileHighlightObj.SetActive(false);
        _highlight.gameObject.SetActive(true);
        _highlight2.gameObject.SetActive(true);
        
        _highlight.DOFade(0.7f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        _highlight2.DOFade(0.7f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        

        _blockTileObj2.SetActive(true);
        var tilePos1 = new Vector3Int(-4, -5, 0);
        var tilePos2 = new Vector3Int(-5, -5, 0);
        yield return new WaitUntil(() => _villageTilemap.GetTile(tilePos1) != null);
        yield return new WaitUntil(() => _villageTilemap.GetTile(tilePos2) != null);

        _highlight.DOKill();
        _highlight2.DOKill();
        _highlight.gameObject.SetActive(false);
        _highlight2.gameObject.SetActive(false);
        
        _blockTileObj2.SetActive(false);
        
        _blockTileObj.SetActive(true);
        _confirmBtnObj.SetActive(true);
        _confirmBtn.enabled = true;
        
        yield return new WaitUntil(() => _isConfirmed);
        
        _blockTileObj.SetActive(false);
        _confirmBtnObj.SetActive(false);
        
        CompleteStep();
    }

    public override void Cleanup()
    {
        _closeBtn.enabled = true;
        _cancelBtn.enabled = true;
    }
}
