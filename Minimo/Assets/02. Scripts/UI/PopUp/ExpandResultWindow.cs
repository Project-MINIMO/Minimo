using UnityEngine;
using DG.Tweening;
using TMPro;

public abstract class ExpandResultWindow : PopUpWindow
{
    [SerializeField] private TextMeshProUGUI _titleTMP;
    [SerializeField] private TextMeshProUGUI _descriptionTMP;
    [SerializeField] private TextMeshProUGUI _resultTMP;
    
    private RectTransform _rect;
    private float _completeTime;
    private bool _isCompleteExpand;

    protected override void Awake()
    {
        base.Awake();
        
        _rect = GetComponent<RectTransform>();

        var titleData = App.GetData<TitleData>();
        _titleTMP.text = titleData.GetString("STR_EXPAND_COMPLETE_TITLE");
        _descriptionTMP.text = GetDescription(titleData);
    }
    
    public override void Show()
    {
        base.Show();

        _completeTime = Time.time;
        _isCompleteExpand = false;
        _resultTMP.SetText(GetCapacity().ToString());
        _rect.localScale = Vector2.one;
    }

    private void Update()
    {
        if (!gameObject.activeInHierarchy) return;
        if (_isCompleteExpand) return;
        if (Time.time - _completeTime < 2f) return;

        _isCompleteExpand = true;
        _rect.DOScale(0, 0.1f).SetEase(Ease.OutCirc)
            .OnComplete(Assign);
    }

    protected abstract string GetDescription(TitleData title);
    protected abstract int GetCapacity();
}
