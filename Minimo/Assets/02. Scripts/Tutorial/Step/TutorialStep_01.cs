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
    [SerializeField] private GameObject _chiefMinimoTextObj;
    [SerializeField] private TextMeshProUGUI _chiefMinimoText;
    [SerializeField] private GameObject[] _hideUIs;
    [SerializeField] private GameObject _tileBtnObj;
    [SerializeField] private GameObject _tileBtnHighlightObj;
    [SerializeField] private GameObject _tileHighlightObj;
    [SerializeField] private TilePanel _tilePanel;
    [SerializeField] private Tilemap _highlightTilemap;
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

    private bool _isConfirmed;
    
    private void Awake()
    {
        _chiefMinimoTextObj.SetActive(false);
    }
    
    protected override void OnStart()
    {
        foreach (var ui in _hideUIs)
        {
            ui.gameObject.SetActive(false);
        }
        
        _tileBtnHighlightObj.SetActive(false);
        _chiefMinimoTextObj.SetActive(true);
        _chiefMinimo.GetComponent<TutorialInteractable>().onClick += OnClickedChief;
        _chiefMinimoText.text = "......";
        _confirmBtn.onClick.AddListener(() => _isConfirmed = true);
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
        _tutorialStep_05.SpawnFarm();
        //chiefminimo dorotate로 세우기
        _chiefMinimoTextObj.SetActive(false);
        
        yield return _dialogueBox.Show(
            "방금 무슨일이 일어난거지?",
            "당신은... (침묵)",
            "일단 마을 미니모들을 구해줘야해",
            "아직 제정신이 아니라서...",
            "나 대신 마을 미니모들을 도와줄 수 있어?");
        
        
        _focusPanel.FocusOn(Vector3.down,
            targetZoom: 4,
            duration: 2,
            onComplete: () =>
            {
                StartCoroutine(ProgressQuest());
            },
            closeOnComplete: true);
    }

    private IEnumerator ProgressQuest()
    {
        _chiefMinimoText.text = "저기 보이는 타일을 설치해서 미니모가 무사한지 확인해줘";
        _chiefMinimoTextObj.SetActive(true);
        _tileBtnObj.SetActive(true);
        _tileBtnHighlightObj.SetActive(true);
        _closeBtn.enabled = false;
        _cancelBtn.enabled = false;
        _confirmBtn.enabled = false;
        _scrollRect.enabled = false;

        yield return new WaitUntil(() => _tilePanel.gameObject.activeInHierarchy);
        _chiefMinimoTextObj.SetActive(false);
        _tileBtnHighlightObj.SetActive(false);
        _tileHighlightObj.SetActive(true);
        
        yield return new WaitUntil(() => _tileSelectObj.activeInHierarchy);
        
        _scrollRect.enabled = true;
        _tileHighlightObj.SetActive(false);
        _highlightTilemap.gameObject.SetActive(true); 
        DOTween.To(
                () => _highlightTilemap.color.a,
                alpha =>
                {
                    var currentColor = _highlightTilemap.color;
                    currentColor.a = alpha;
                    _highlightTilemap.color = currentColor;
                },
                0.7f,
                0.5f
            )
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .SetId(_highlightTilemap);

        _blockTileObj2.SetActive(true);
        var tilePos1 = new Vector3Int(-4, -5, 0);
        var tilePos2 = new Vector3Int(-5, -5, 0);
        yield return new WaitUntil(() => _villageTilemap.GetTile(tilePos1) != null);
        yield return new WaitUntil(() => _villageTilemap.GetTile(tilePos2) != null);
        
        DOTween.Kill(_highlightTilemap);
        _highlightTilemap.gameObject.SetActive(false);
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
