using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProduceSlotExpandHandler : MonoBehaviour
{
    [SerializeField] private Button _expandBtn;
    
    [SerializeField] private GameObject _confirmBack;
    [SerializeField] private Button _confirmBtn;
    [SerializeField] private Button _cancelBtn;
    
    [SerializeField] private TextMeshProUGUI _priceTMP;
    
    private ProduceManager _produceManager;
    private ProduceElevated _produceObject;
    private UseCashPanel _useCashPanel;
    
    private int _currentPrice;

    private void Awake()
    {
        _produceManager = App.GetManager<ProduceManager>();
        _useCashPanel = App.GetManager<UIManager>().GetPanel<UseCashPanel>();
        
        _expandBtn.onClick.AddListener(Expand);
        _confirmBtn.onClick.AddListener(Confirm);
        _cancelBtn.onClick.AddListener(() =>
        {
            _expandBtn.gameObject.SetActive(true);
            _confirmBack.SetActive(false);
        });
    }

    private void OnEnable()
    {
        if (_produceManager == null) return;
        if (_produceManager.CurrentObject == null) return;

        _produceObject = _produceManager.CurrentObject as ProduceElevated;
        
        _expandBtn.gameObject.SetActive(true);
        _confirmBack.SetActive(false);
        UpdatePriceText();
    }

    private void Expand()
    {
        if (_currentPrice <= AccountInfo.Instance.Cash)
        {
            _expandBtn.gameObject.SetActive(false);
            _confirmBack.SetActive(true);
        }
        else
        {
            _useCashPanel.OpenPanel();
        }
    }

    private void Confirm()
    {
        _expandBtn.gameObject.SetActive(true);
        _confirmBack.SetActive(false);
        _produceObject.AddSlotCount();
        
        UpdatePriceText();
    }

    private void UpdatePriceText()
    {
        _currentPrice = _produceObject.MaxSlotCount * 100;
        _priceTMP.text = _currentPrice.ToString();
    }
}
