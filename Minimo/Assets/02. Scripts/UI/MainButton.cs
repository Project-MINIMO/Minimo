using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainButton : MonoBehaviour
{
    [SerializeField] private Button _closeBtn;
    [SerializeField] private RectTransform[] _btnRects;
    [SerializeField] private float _openYPosition = -294;
    [SerializeField] private float _openSize = 2;
    [SerializeField] private float _duration = 0.3f;

    private RectTransform _rect;
    private bool _isOpen;
    private Vector2[] _btnPositions;
    private float _closeYPosition;
    
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(Toggle);
        _rect = GetComponent<RectTransform>();
        _closeBtn.onClick.AddListener(Close);
        _closeYPosition = _rect.anchoredPosition.y;

        _btnPositions = new Vector2[_btnRects.Length];

        for (var i = 0; i < _btnRects.Length; i++)
        {
            _btnPositions[i] = _btnRects[i].anchoredPosition;
            _btnRects[i].DOAnchorPos(Vector2.zero, 0).SetEase(Ease.Linear);
            _btnRects[i].gameObject.SetActive(false);
        }
    }

    private void Toggle()
    {
        if (_isOpen)
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
        if (_isOpen) return;
        
        _isOpen = true;
        _rect.DOKill();
        _rect.DOScale(_openSize, _duration).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                _closeBtn.gameObject.SetActive(true);
                for (var i = 0; i < _btnRects.Length; i++)
                {
                    _btnRects[i].gameObject.SetActive(true);
                    _btnRects[i].DOAnchorPos(_btnPositions[i], _duration).SetEase(Ease.Linear);
                }
            });
        
        _rect.DOAnchorPosY(_openYPosition, _duration).SetEase(Ease.Linear);
    }

    private void Close()
    {
        if (!_isOpen) return;
        
        _isOpen = false;
        
        foreach (var rect in _btnRects)
        {
            rect.DOAnchorPos(Vector2.zero, 0).SetEase(Ease.Linear);
            rect.gameObject.SetActive(false);
        }
        
        _rect.DOScale(Vector3.one, _duration).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                _closeBtn.gameObject.SetActive(false);
            });
        
        _rect.DOAnchorPosY(_closeYPosition, _duration).SetEase(Ease.Linear);
    }
}
