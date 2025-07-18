using System.Collections.Generic;

using UnityEngine;
using TMPro;
using DG.Tweening;

public class BottomNotification : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _messageTMP1;
    [SerializeField] private TextMeshProUGUI _messageTMP2;
    [SerializeField] private float _slideHeight = 25f;
    
    private RectTransform _rect;
    private CanvasGroup _canvasGroup;
    
    private Vector2 _startPosition;
    private float _shownPositionY;
    
    private Sequence _sequence;

    private Dictionary<NotifyType, string> _notifications;

    private void Start()
    {
        _rect = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();

        _startPosition = _rect.anchoredPosition;
        _shownPositionY = _startPosition.y + _slideHeight;

        var titleData = App.GetData<TitleData>();
        _notifications = new Dictionary<NotifyType, string>()
        {
            [NotifyType.CapacityLack] = titleData.GetString("STR_PRODUCE_STORAGE"),
        };
        
        Reset();
    }

    public void ShowNotification(NotifyType type)
    {
        _messageTMP1.SetText(_notifications[type]);
        _messageTMP2.SetText(_notifications[type]);
        
        _sequence = DOTween.Sequence();
        _sequence
            .SetAutoKill(false)
            .OnStart(Reset)
            .Append(_rect.DOAnchorPosY(_shownPositionY, 0.5f).SetEase(Ease.OutCubic))
            .Join(_canvasGroup.DOFade(1f, 0.5f).SetEase(Ease.OutCubic))
            .AppendInterval(2f)
            .Append(_rect.DOAnchorPosY(_startPosition.y, 0.25f).SetEase(Ease.InCubic))
            .Join(_canvasGroup.DOFade(0f, 0.25f).SetEase(Ease.InCubic))
            .Play();
    }

    private void Reset()
    {
        _rect.anchoredPosition = _startPosition;
        _canvasGroup.alpha = 0f;
    }
}
