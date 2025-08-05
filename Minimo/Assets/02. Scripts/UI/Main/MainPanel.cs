using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainPanel : UIBase
{
    public override bool IsDefaultPanel => true;
    
    [SerializeField] private RectTransform[] _btnRects;
    [SerializeField] private float _openYPosition = -294;
    [SerializeField] private float _openSize = 2;
    [SerializeField] private float _duration = 0.3f;
    
    [SerializeField] private Button _mainBtn;
    [SerializeField] private Button _closeBtn;
    
    private RectTransform _panelRect;
    private RectTransform _mainRect;
    private Vector2[] _btnPositions;
    private readonly Vector2 _hidePosition = new(0, -300);
    private float _closeYPosition;
    
    private bool _isOpened;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        _panelRect = GetComponent<RectTransform>();
        _mainRect = _mainBtn.GetComponent<RectTransform>();
        _closeYPosition = _mainRect.anchoredPosition.y;
        _btnPositions = new Vector2[_btnRects.Length];

        /*
        for (var i = 0; i < _btnRects.Length; i++)
        {
            _btnPositions[i] = _btnRects[i].anchoredPosition;
            _btnRects[i].DOAnchorPos(Vector2.zero, 0).SetEase(Ease.Linear);
            _btnRects[i].gameObject.SetActive(false);
        }
        */
        
        _mainBtn.onClick.AddListener(Toggle);
        _closeBtn.onClick.AddListener(() => Close(_duration));
    }

    public override void Show(bool isNew)
    {
        _panelRect.DOAnchorPos(Vector2.zero, _duration).SetEase(Ease.OutCubic);

        //Close(0);
    }

    public override void Hide(bool isNew)
    {
        _panelRect.DOAnchorPos(_hidePosition, _duration).SetEase(Ease.InCubic);
        
        //Close(0);
    }

    private void Toggle()
    {
        if (_isOpened)
        {
            Close(_duration);
        }
        else
        {
            Open();
        }
    }

    private void Open()
    {
        if (_isOpened) return;
        
        _isOpened = true;
        _mainRect.DOKill();
        _mainRect.DOScale(_openSize, _duration).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                _closeBtn.gameObject.SetActive(true);
                for (var i = 0; i < _btnRects.Length; i++)
                {
                    _btnRects[i].gameObject.SetActive(true);
                    _btnRects[i].DOAnchorPos(_btnPositions[i], _duration).SetEase(Ease.Linear);
                }
            });
        
        _mainRect.DOAnchorPosY(_openYPosition, _duration).SetEase(Ease.Linear);
    }

    private void Close(float duration)
    {
        if (!_isOpened) return;
        
        _isOpened = false;
        _mainRect.DOKill();
        _mainRect.DOScale(Vector3.one, duration).SetEase(Ease.Linear)
            .OnPlay(() =>
            {
                foreach (var rect in _btnRects)
                {
                    rect.DOKill();
                    rect.DOAnchorPos(Vector2.zero, 0).SetEase(Ease.Linear);
                    rect.gameObject.SetActive(false);
                }
            })
            .OnComplete(() =>
            {
                _closeBtn.gameObject.SetActive(false);
            });
        
        _mainRect.DOAnchorPosY(_closeYPosition, duration).SetEase(Ease.Linear);
    }
}
