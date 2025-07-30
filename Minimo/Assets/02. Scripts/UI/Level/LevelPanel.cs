using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class LevelPanel : UIBase
{
    public override bool IsDefaultPanel => true;

    [SerializeField] private Image _fillImg;
    [SerializeField] private TextMeshProUGUI _levelTMP;

    private const string LevelString = "Lv. {0}";
    private const float FillSpeed = 0.15f;
    
    private UIManager _uiManager;
    private RectTransform _rect;
    private int _prevLevel = 1;
    private Sequence _sequence;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);
        
        _uiManager = manager;
        
        _rect = GetComponent<RectTransform>();
        AccountInfo.Instance.Level.OnExpChanged += OnExpChanged;
    }
    
    public override void Show(bool isNew)
    {
        _rect.DOAnchorPosY(-25f, 0.3f).SetEase(Ease.OutCubic);
    }

    public override void Hide(bool isNew)
    {
        _rect.DOAnchorPosY(100, 0.3f).SetEase(Ease.InCubic);
    }

    private void OnExpChanged(int exp)
    {
        Show(false);
        
        var newLevel = exp / 100;
        var newExp = exp % 100;
    
        var newFill = newExp / 100f;
        var currentFill = _fillImg.fillAmount;

        _sequence.Kill();
        _fillImg.DOKill();
        
        _sequence = DOTween.Sequence();
        
        if (newLevel > _prevLevel)
        {
            var fillTo1 = 1f - currentFill;
            var duration1 = fillTo1 / FillSpeed;

            _sequence.Append(_fillImg.DOFillAmount(1f, duration1).SetEase(Ease.Linear));
            _sequence.AppendCallback(() => 
            { 
                _levelTMP.text = string.Format(LevelString, newLevel); 
                _fillImg.fillAmount = 0f; 
            });
            
            var duration2 = newFill / FillSpeed;
            _sequence.Append(_fillImg.DOFillAmount(newFill, duration2).SetEase(Ease.Linear));
            _sequence.Join(_levelTMP.DOScale(1.5f, 0.1f).SetEase(Ease.OutCubic));
            _sequence.Append(_levelTMP.DOScale(1f, 0.1f).SetEase(Ease.InCubic));
        }
        else
        {
            var duration = Mathf.Abs(newFill - currentFill) / FillSpeed;
            _sequence.Append(_fillImg.DOFillAmount(newFill, duration).SetEase(Ease.Linear));
        }
        
        _sequence.OnComplete(() =>
        {
            if (!_uiManager.IsOnlyDefaultPanelsInStack)
            {
                Hide(false);
            }
            
            _prevLevel = newLevel;
        });
        _sequence.Play();
    }
}
