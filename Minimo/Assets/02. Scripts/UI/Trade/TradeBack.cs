using UnityEngine;
using UnityEngine.UI;

public class TradeBack : MonoBehaviour
{
    [SerializeField] private Image _fillImg;
    
    private TradeMinimoSlot[] _slots;
    
    private void Awake()
    {
        TradeManager.Instance.OnSaleStateChanged += OnSaleStateChanged;
        TradeManager.Instance.OnNewRequest += OnNewRequest;
        
        _slots = GetComponentsInChildren<TradeMinimoSlot>(true);
        gameObject.SetActive(false);
    }
    
    private void OnSaleStateChanged(bool isOnSale)
    {
        gameObject.SetActive(isOnSale);
       
        foreach (var slot in _slots)
        {
            slot.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (TradeManager.Instance.CurrentTime > 0)
            _fillImg.fillAmount = TradeManager.Instance.CurrentTime / TradeManager.Instance.SaleTime;
        else
            _fillImg.fillAmount = 0;
    }
    
    private void OnNewRequest(MinimoRequest request)
    {
        foreach (var slot in _slots)
        {
            if (!slot.gameObject.activeSelf)
            {
                slot.Initialize(request);
                break;
            }
        }
    }
}
