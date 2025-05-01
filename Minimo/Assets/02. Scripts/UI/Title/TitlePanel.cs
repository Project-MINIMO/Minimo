using System;
using MinimoShared;
using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TitlePanel : MonoBehaviour
{
    [SerializeField] private Button _startBtn;
    [SerializeField] private RectTransform _startTextRect;
    [SerializeField] private Image _titleFstImg;
    [SerializeField] private Image _titleSndImg;
    
    [SerializeField] private TitleLoadHandler _loadHandler;
    
    [SerializeField] private Image _blackBlur;
    [SerializeField] private Image _blackBlur2;

    private bool _isNew;
    
    private void Awake()
    {
        _startBtn.onClick.AddListener(OnClickStart);
        _startBtn.gameObject.SetActive(false);
    }

    private void Start()
    {
        _blackBlur2.DOFade(0, 2f).SetEase(Ease.Linear)
            .OnComplete(() => ShowTitle());
    }

    public async UniTask ShowTitle(bool isNew = false)
    {
        App.GetData<TitleData>().IsFirstLogin = isNew;
        _isNew = isNew;
        _loadHandler.Setup(10);
      
        _loadHandler.FinishLoad();
        _startBtn.gameObject.SetActive(true);
        
        var startPositionY = _startTextRect.anchoredPosition.y;
        _startTextRect.DOAnchorPosY(startPositionY + 10f, 0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.Linear);
    }

    private void OnClickStart()
    {
        _startTextRect.DOKill();
        _startBtn.gameObject.SetActive(false);
        
        Sequence sequence = DOTween.Sequence();
        sequence.Append(_titleFstImg.DOFade(0, 1))
            .Join(_titleSndImg.DOFade(1, 1))
            .AppendCallback(() => _blackBlur.gameObject.SetActive(true))
            .Append(_blackBlur.DOFade(1, 0.5f))
            .OnComplete(() =>
            {
                App.LoadScene(SceneName.Game);
            });
    }
}
