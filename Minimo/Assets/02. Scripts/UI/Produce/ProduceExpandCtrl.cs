using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProduceSlotExpandCtrl : MonoBehaviour
{
    [SerializeField] private Button _expandBtn;
    [SerializeField] private GameObject _expandBack;
    [SerializeField] private Button _confirmBtn;
    [SerializeField] private Button _cancelBtn;
    [SerializeField] private TextMeshProUGUI _priceTMP;
    
    private UseCashPanel _useCashPanel;

    private int _currentPrice = 100;

    private void Start()
    {
        _useCashPanel = App.GetManager<UIManager>().GetPanel<UseCashPanel>();
        
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
