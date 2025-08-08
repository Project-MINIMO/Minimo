using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class LevelPanel : UIBase
{
    public override bool IsDefaultPanel => true;

    [SerializeField] private Image _fillImg;
    [SerializeField] private TextMeshProUGUI _levelTMP;
    [SerializeField] private Button _focusStarBtn;

    private const string LevelString = "{0}";
    private const float FillAnimDuration = 0.5f;
    
    private UIManager _uiManager;
    private RectTransform _rect;
    private int _prevLevel = 1;
    private Sequence _sequence;
    
    private StarManager _starManager;
    private FocusPanel _focusPanel;
    private int _currentStarIndex = 0;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);
        
        _uiManager = manager;
        _focusPanel = manager.GetPanel<FocusPanel>();
        _starManager = App.GetManager<StarManager>();
        
        _rect = GetComponent<RectTransform>();
        _focusStarBtn.onClick.AddListener(FocusStar);
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
    
    public void FocusStar()
    {
        var star = _starManager.Stars[_currentStarIndex];
        _currentStarIndex = (_currentStarIndex + 1) % _starManager.Stars.Count;
        _focusPanel.FocusOnWithoutOpen(star.transform.position);
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
            var dist1 = Mathf.Max(0f, 1f - currentFill);
            var dist2 = Mathf.Max(0f, newFill);
            var total = dist1 + dist2;

            float dur1 = 0f, dur2 = 0f;
            if (total > 0f)
            {
                dur1 = FillAnimDuration * (dist1 / total);
                dur2 = FillAnimDuration * (dist2 / total);
            }

            _sequence.Append(_fillImg.DOFillAmount(1f, dur1).SetEase(Ease.Linear));
            _sequence.AppendCallback(() =>
            {
                _levelTMP.text = string.Format(LevelString, newLevel);
                _fillImg.fillAmount = 0f;
            });

            _sequence.Append(_fillImg.DOFillAmount(newFill, dur2).SetEase(Ease.Linear));
            _sequence.Join(_levelTMP.DOScale(1.5f, 0.1f).SetEase(Ease.OutCubic)
                .OnComplete(() => _levelTMP.DOScale(1f, 0.1f).SetEase(Ease.InCubic)));
        }
        else
        {
            _sequence.Append(_fillImg.DOFillAmount(newFill, FillAnimDuration).SetEase(Ease.Linear));
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
