using UniRx;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainPanel : UIBase
{
    [SerializeField] private RectTransform[] _btnRects;
    [SerializeField] private float _openYPosition = -294;
    [SerializeField] private float _openSize = 2;
    [SerializeField] private float _duration = 0.3f;
    
    [SerializeField] private Button _mainBtn;
    [SerializeField] private Button _closeBtn;
    
    private RectTransform _mainRect;
    private Vector2[] _btnPositions;
    private float _closeYPosition;
    
    private bool _isOpened = false;

    public override void Initialize()
    {
        var screenStateManager = App.GetManager<ScreenStateManager>();
        screenStateManager.CurrentState.Subscribe((currentState) =>
        {
            if (currentState is ScreenState.Sky)
            {
                OpenPanel();
            }
            else
            {
                ClosePanel();
            }
        }).AddTo(gameObject);
        
        _mainRect = _mainBtn.GetComponent<RectTransform>();
        _closeYPosition = _mainRect.anchoredPosition.y;
        _btnPositions = new Vector2[_btnRects.Length];

        for (var i = 0; i < _btnRects.Length; i++)
        {
            _btnRects[i].GetComponent<Button>().onClick.AddListener(Close);
            
            _btnPositions[i] = _btnRects[i].anchoredPosition;
            _btnRects[i].DOAnchorPos(Vector2.zero, 0).SetEase(Ease.Linear);
            _btnRects[i].gameObject.SetActive(false);
        }
        
        _mainBtn.onClick.AddListener(Toggle);
        _closeBtn.onClick.AddListener(Close);
    }
   
    private void Toggle()
    {
        if (_isOpened)
        {
            Close();
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

    private void Close()
    {
        if (!_isOpened) return;
        
        _isOpened = false;
        
        foreach (var rect in _btnRects)
        {
            rect.DOAnchorPos(Vector2.zero, 0).SetEase(Ease.Linear);
            rect.gameObject.SetActive(false);
        }
        
        _mainRect.DOScale(Vector3.one, _duration).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                _closeBtn.gameObject.SetActive(false);
            });
        
        _mainRect.DOAnchorPosY(_closeYPosition, _duration).SetEase(Ease.Linear);
    }
}
