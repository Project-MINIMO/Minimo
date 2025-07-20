using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class QuestCompactSlot : QuestSlot
{
    public event Action<QuestCompactSlot> OnSlotOpened;
    
    [SerializeField] private Button _toggleBtn;
    
    [SerializeField] private TextMeshProUGUI _descTMP;
    [SerializeField] private RectTransform _rect;
    [SerializeField] private Image _detailBackgroundImg;
    [SerializeField] private Image _defaultBackgroundImg;
    [SerializeField] private GameObject[] _iconObjs;
    [SerializeField] private TextMeshProUGUI _nameTMP;
    
    private const float ExpandedHeight = 153;
    private const float CollapsedHeight = 69;
    private readonly Vector2 _titleExpandedPos = new(36.2f, -47.4f);
    private readonly Vector2 _titleCollapsedPos = new(96f, -16.2f);
    
    private bool _isExpanded;

    protected override void Awake()
    {
        base.Awake();
        
        _toggleBtn.onClick.AddListener(Toggle);
    }

    public override void Initialize(Quest data)
    {
        base.Initialize(data);
        
        _iconObjs[GetIndex(data.Type)].SetActive(true);
    }
    
    private int GetIndex(QuestType type) => (int)type switch
    {
        <= 1 => 0,
        2 => 1,
        <= 4 => 2,
        _ => -1
    };

    private void Toggle()
    {
        if (_isExpanded)
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
        if (_isExpanded) return;
        
        _isExpanded = true;
        OnSlotOpened?.Invoke(this);

        DOTween.To(() => _rect.sizeDelta.y, 
                y =>
                {
                    _rect.sizeDelta = new Vector2(_rect.sizeDelta.x, y);
                },
                ExpandedHeight,
                0.3f)
            .SetEase(Ease.Linear);
        
        _detailBackgroundImg.DOFade(1f, 0.25f).SetEase(Ease.Linear);
        _defaultBackgroundImg.DOFade(0f, 0.25f).SetEase(Ease.Linear);
        
        _nameTMP.rectTransform.DOAnchorPos(_titleExpandedPos, 0.3f).SetEase(Ease.Linear);
        _descTMP.DOFade(1, 0.2f).SetEase(Ease.Linear).SetDelay(0.1f);
        
        DOTween.To(() => _nameTMP.fontSize, 
                x => _nameTMP.fontSize = x, 
                32f, 
                0.3f) 
            .SetEase(Ease.Linear);
        
        _selectBtn.gameObject.SetActive(true);
    }

    public void Close()
    {
        if (!_isExpanded) return;
        
        _isExpanded = false;
        DOTween.To(() => _rect.sizeDelta.y, 
                y =>
                {
                    _rect.sizeDelta = new Vector2(_rect.sizeDelta.x, y);
                },
                CollapsedHeight,
                0.3f)
            .SetEase(Ease.Linear);
        
        _detailBackgroundImg.DOFade(0f, 0.25f).SetEase(Ease.Linear);
        _defaultBackgroundImg.DOFade(1f, 0.25f).SetEase(Ease.Linear);
        
        _nameTMP.rectTransform.DOAnchorPos(_titleCollapsedPos, 0.3f).SetEase(Ease.Linear);
        _descTMP.DOFade(0, 0.2f).SetEase(Ease.Linear);
        
        DOTween.To(() => _nameTMP.fontSize, 
                x => _nameTMP.fontSize = x, 
                20f, 
                0.3f) 
            .SetEase(Ease.Linear);
        
        _selectBtn.gameObject.SetActive(false);
    }
}
