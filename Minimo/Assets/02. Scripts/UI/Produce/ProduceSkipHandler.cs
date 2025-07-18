using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;

public class ProduceSkipHandler : MonoBehaviour
{
    [SerializeField] private Button _skipBtn;

    [SerializeField] private GameObject _confirmBack;
    [SerializeField] private Button _confirmBtn;
    [SerializeField] private Button _cancelBtn;
    
    [SerializeField] private TextMeshProUGUI _priceTMP;
    
    private ProduceManager _produceManager;
    private ProduceTask _produceTask;
    private UseCashPanel _useCashPanel;

    private float _globalTimeSkipCostRatio;
    private int _modifiedPrice;
    private int _currentPrice;
    private int _skipCost;

    private void Awake()
    {
        _produceManager = App.GetManager<ProduceManager>();
        _useCashPanel = App.GetManager<UIManager>().GetPanel<UseCashPanel>();
        
        _skipCost = App.GetData<TitleData>().Common["TimeSkipCost"];
        
        _skipBtn.onClick.AddListener(Skip);
        _confirmBtn.onClick.AddListener(Confirm);
        _cancelBtn.onClick.AddListener(() =>
        {
            _skipBtn.gameObject.SetActive(true);
            _confirmBack.SetActive(false);
        });
        
        App.GetManager<MinimoManager>()
            .GlobalTimeSkipCostRatio
            .Subscribe(value =>
            {
                _globalTimeSkipCostRatio = value;
            })
            .AddTo(this);
    }

    private void OnEnable()
    {
        if (_produceManager == null) return;
        if (_produceManager.CurrentObject == null) return;

        _produceTask = _produceManager.CurrentObject.ActiveTask;
        if (_produceTask == null) return;

        _produceTask.OnRemainTimeChanged += UpdatePriceText;
        UpdatePriceText(_produceTask.RemainTime);
        
        _skipBtn.gameObject.SetActive(true);
        _confirmBack.SetActive(false);
    }

    private void OnDisable()
    {
        if (_produceTask == null) return;
        
        _produceTask.OnRemainTimeChanged -= UpdatePriceText;
    }

    private void Skip()
    {
        if (_modifiedPrice <= AccountInfo.Instance.blueStar)
        {
            _skipBtn.gameObject.SetActive(false);
            _confirmBack.SetActive(true);
        }
        else
        {
            _useCashPanel.OpenPanel();
        }
    }

    private void Confirm()
    {
        _skipBtn.gameObject.SetActive(true);
        _confirmBack.SetActive(false);
        
        App.LogBox("blue", "시간 단축 비용 로그", new()
        {
            { "기존 시간 단축 비용", _currentPrice.ToString() },
            { "시간 단축 비율", _globalTimeSkipCostRatio.ToString() },
            { "재계산된 비용", (_globalTimeSkipCostRatio * _currentPrice).ToString() },
            { "반올림된 최종 비용", _modifiedPrice.ToString() },
        });
       
        _produceManager.Skip();
    }

    private void UpdatePriceText(float remainTime)
    {
        _currentPrice = (int)(remainTime / _skipCost) + 1;
        var modifiedPrice = _globalTimeSkipCostRatio * _currentPrice;
        _modifiedPrice = Mathf.RoundToInt(modifiedPrice);
        _priceTMP.text = _modifiedPrice.ToString();
    }
}
