using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CapacityHandler : TransactionHandler
{
    [SerializeField] private TextMeshProUGUI _titleTMP;
    [SerializeField] private TextMeshProUGUI _currentCapacityTMP;
    [SerializeField] private Button _closeBtn;

    protected override int Step => 10;
    
    private UseCashPanel _useCashPanel;
    
    private int _expandCost;
    private int _currentCapacity;
    
    protected override void Awake()
    {
        base.Awake();
        
        _useCashPanel = App.GetManager<UIManager>().GetPanel<UseCashPanel>();
        
        var titleData = App.GetData<TitleData>();
        _expandCost = titleData.Common["StorageExpandCost"];
        TransactionString = titleData.GetString("STR_STORAGE_EXPAND_COST");
        _titleTMP.text = titleData.GetString("STR_STORTAGE_UI_EXPAND_DESC");
        
        _closeBtn.onClick.AddListener(() => gameObject.SetActive(false));
    }

    public override void Initialize()
    {
        gameObject.SetActive(true);
        _currentCapacity = AccountInfo.Instance.Capacity;
        Quantity = _currentCapacity + 10;
        _currentCapacityTMP.SetText($"{AccountInfo.Instance.CurrentItemCounts}/{_currentCapacity}");
        
        base.Initialize();
    }
    
    protected override int CalculatePrice() => Mathf.Max(_expandCost * (Quantity - _currentCapacity), 0);

    protected override int GetMaxQuantity() => 9999; //TODO : 레벨별로 다른 최대용량
    protected override int GetMinQuantity() => _currentCapacity + 10;

    protected override void Transaction()
    {
        if (Price <= AccountInfo.Instance.blueStar)
        {
            AccountInfo.Instance.blueStar -= Price;
            AccountInfo.Instance.AddCapacity(Quantity - _currentCapacity);
            
            base.Transaction();
        }
        else
        {
            _useCashPanel.OpenPanel();
        }
    }
}
