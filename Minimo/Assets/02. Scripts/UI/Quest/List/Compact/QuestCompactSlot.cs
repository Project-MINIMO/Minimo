using System;

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class QuestCompactSlot : QuestSlot
{
    public event Action<QuestCompactSlot> OnSlotOpened;
    
    [SerializeField] private Button _toggleBtn;
    
    [SerializeField] private TextMeshProUGUI _nameTMP;  
    [SerializeField] private TextMeshProUGUI _descTMP;
    
    [SerializeField] private Image _detailBackgroundImg;
    [SerializeField] private Image _defaultBackgroundImg;
    [SerializeField] private GameObject[] _iconObjs;
    
    private const float ExpandedHeight = 153;
    private const float CollapsedHeight = 69;
    private readonly Vector2 _titleExpandedPos = new(36.2f, -47.4f);
    private readonly Vector2 _titleCollapsedPos = new(96f, -16.2f);
    
    private RectTransform _rect;
    private bool _isExpanded;

    protected override void Awake()
    {
        base.Awake();

        _rect = GetComponent<RectTransform>();
        _toggleBtn.onClick.AddListener(() => AnimateToggle(!_isExpanded));
        
        var titleData = App.GetData<TitleData>();
        for (var i = 0; i < _iconObjs.Length; i++)
        {
            var text = _iconObjs[i].GetComponentInChildren<TextMeshProUGUI>();
            text.SetText(GetTitleString(i, titleData));
        }
    }
    
    private string GetTitleString(int index, TitleData titleData) => index switch
    {
        0 => titleData.GetString("STR_QUEST_MAIN"),
        1 => titleData.GetString("STR_QUEST_SIDE"),
        2 => titleData.GetString("STR_QUEST_CONSTELLATION"),
        _ => string.Empty
    };

    public override void Initialize(Quest data)
    {
        base.Initialize(data);
        
        _iconObjs[GetIndex(data.Type)].SetActive(true);
        var sequence = DOTween.Sequence();
        sequence.AppendInterval(0.1f)
            .OnComplete(() => AnimateToggle(false)).Play();
    }
    
    private int GetIndex(QuestType questType) => questType switch
    {
        QuestType.Guide => 0,
        QuestType.Story => 0,
        QuestType.Side => 1,
        QuestType.Wish => 1,
        QuestType.Constellation => 2,
        _ => 0
    };
  
    public void Close() => AnimateToggle(false);

    private void AnimateToggle(bool isOpen)
    {
        if (_isExpanded == isOpen) return;
        if (isOpen) OnSlotOpened?.Invoke(this);
        _isExpanded = isOpen;

        _rect.DOSizeDelta(new Vector2(_rect.sizeDelta.x, isOpen
            ? ExpandedHeight
            : CollapsedHeight),
            0.2f).SetEase(Ease.Linear);
        
        _detailBackgroundImg.DOFade(isOpen ? 1 : 0, 0.15f).SetEase(Ease.Linear);
        _defaultBackgroundImg.DOFade(isOpen ? 0 : 1, 0.15f).SetEase(Ease.Linear);
        
        _nameTMP.rectTransform.DOAnchorPos(isOpen 
            ? _titleExpandedPos 
            : _titleCollapsedPos, 
            0.2f).SetEase(Ease.Linear);
        _descTMP.DOFade(isOpen ? 1 : 0, 0.1f).SetEase(Ease.Linear);
        _nameTMP.DOFontSize(isOpen ? 32 : 20, 0.2f).SetEase(Ease.Linear);
        
        _selectBtn.gameObject.SetActive(isOpen);
    }
}
