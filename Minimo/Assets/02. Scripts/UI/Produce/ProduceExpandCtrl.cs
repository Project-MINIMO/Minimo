using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProduceExpandCtrl : MonoBehaviour
{
    [SerializeField] private Button _expandBtn;
    [SerializeField] private GameObject _expandBack;
    [SerializeField] private Button _confirmBtn;
    [SerializeField] private Button _cancelBtn;
    [SerializeField] private TextMeshProUGUI _priceTMP;
    
    private UseCashPanel _useCashPanel;
    private AdvancedPanel _advancedPanel;

    private int _currentPrice = 100;

    private void Start()
    {
        var uiManager = App.GetManager<UIManager>();
        
        _useCashPanel = uiManager.GetPanel<UseCashPanel>();
        _advancedPanel = uiManager.GetPanel<AdvancedPanel>();
        
        _expandBtn.onClick.AddListener(OnClickConfirm);
        
        UpdatePriceText();
    }
    
    private void OnClickConfirm()
    {
        _useCashPanel.OpenPanel(UseCashType.ProduceExpand, 
            _currentPrice, 
            Expand);
    }

    private void Expand()
    {
        //var isRemainDeactiveBtn = _advancedPanel.ExpandTaskBtn();
        //gameObject.SetActive(isRemainDeactiveBtn);
        
        _currentPrice += 100;
        UpdatePriceText();
    }

    private void UpdatePriceText()
    {
        _priceTMP.text = _currentPrice.ToString();
    }
}
