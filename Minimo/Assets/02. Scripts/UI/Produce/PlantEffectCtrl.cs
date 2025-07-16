using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlantEffectCtrl : MonoBehaviour
{
    [SerializeField] private RectTransform _iconRect;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Image[] _iconImg;

    private Vector2 _startPosition;
    private Sequence _sequence;
    
    private void Awake()
    {
        _startPosition = _iconRect.anchoredPosition;
        _sequence = DOTween.Sequence();
        _sequence
            .SetAutoKill(false)
            .OnStart(() =>
            {
                _iconRect.anchoredPosition = _startPosition;
                _iconRect.sizeDelta = Vector2.one;
                _canvasGroup.alpha = 1;
            })
            .Append(_iconRect.DOScale(1f, 0.2f).SetEase(Ease.OutElastic))
            .AppendInterval(0.3f)
            .Append(_iconRect.DOAnchorPos(Vector2.zero, 0.3f).SetEase(Ease.InQuad))
            .Join(_iconRect.DOScale(0f, 0.3f).SetEase(Ease.InQuad))
            .Join(_canvasGroup.DOFade(0, 0.2f).SetEase(Ease.InQuad));
    }

    public void PlayEffect(int[] itemID)
    {
        SetItemIcons(itemID);
        
        _sequence.Restart();
    }

    private void SetItemIcons(int[] itemID)
    {
        var i = 0;
        
        for (; i < itemID.Length; i++)
        {
            _iconImg[i].gameObject.SetActive(true);
            _iconImg[i].sprite = AccountInfo.Instance.Items[itemID[i]].Icon;
        }

        for (; i < _iconImg.Length; i++)
        {
            _iconImg[i].gameObject.SetActive(false);
        }
    }
}
