using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ExpandCtrl : MonoBehaviour
{
    [SerializeField] private Button _openBtn;
    [SerializeField] private Button[] _closeBtns;
    
    [SerializeField] private TextMeshProUGUI _titleTMP;
    [SerializeField] private TextMeshProUGUI _descriptionTMP;
    [SerializeField] private TextMeshProUGUI _completeTMP;
    [SerializeField] private TextMeshProUGUI _resultTMP;
    
    [SerializeField] private RectTransform _popUpRect;
    
    [SerializeField] private CapacityHandler _capacityHandler;

    private RectTransform _rect;
    
    private float _completeTime;
    private bool _isCompleteExpand;
    
    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        
        _openBtn.onClick.AddListener(Show);
        foreach (var button in _closeBtns)
        {
            button.onClick.AddListener(() =>
            {
                _rect.DOScale(0, 0.1f).SetEase(Ease.OutCirc)
                    .OnComplete(() => gameObject.SetActive(false));
            });
        }
    }

    public void Initialize(string title, string description, string complete)
    {
        var titleData = App.GetData<TitleData>();
        _titleTMP.text = titleData.GetString(title);
        _descriptionTMP.text = titleData.GetString(description);
        _completeTMP.text = titleData.GetString(complete);
    }

    private void Update()
    {
        if (!gameObject.activeInHierarchy) return;
        if (!_isCompleteExpand) return;
        if (Time.time - _completeTime < 2f) return;

        _isCompleteExpand = false;
        _rect.DOScale(0, 0.1f).SetEase(Ease.OutCirc)
            .OnComplete(() => gameObject.SetActive(false));
    }

    private void Show()
    {
        _isCompleteExpand = false;
        _rect.localScale = Vector3.zero;
        _popUpRect.localScale = Vector3.zero;
        gameObject.SetActive(true);
        
        _rect.DOScale(1, 0.1f).SetEase(Ease.OutCirc);
        _capacityHandler.Initialize(Hide);
    }

    private void Hide()
    {
        _isCompleteExpand = true;
        _resultTMP.SetText(AccountInfo.Instance.StorageCapacity.ToString());
        _completeTime = Time.time;
        _popUpRect.DOScale(1f, 0.1f).SetEase(Ease.OutCirc);
    }
}
