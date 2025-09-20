using UnityEngine;
using UnityEngine.UI;

public class TradeBack : MonoBehaviour
{
    [SerializeField] private Image _fillImg;
    [SerializeField] private TradeClicker _clicker;
    private TradeMinimoSlot[] _slots;
    
    private void Awake()
    {
        TradeManager.Instance.OnSaleStateChanged += OnSaleStateChanged;
        TradeManager.Instance.OnNewRequest += OnNewRequest;
        
        _slots = GetComponentsInChildren<TradeMinimoSlot>(true);
        foreach (var slot in _slots)
        {
            slot.OnSlotSelected += OnSlotSelected;
        }
        gameObject.SetActive(false);
    }
    
    private void OnSaleStateChanged(bool isOnSale)
    {
        gameObject.SetActive(isOnSale);
        _clicker.gameObject.SetActive(false);
       
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
    
    private void OnSlotSelected(MinimoRequest request)
    {
        if (request == null) return;
        if (request.IsServed) return;
        if (request.IsLeaving) return;
        if (request.RequestedItem.Count <= 0) return;

        if (request.IsSpecial)
        {
            _clicker.StartClicking(() =>
            {
                TradeManager.Instance.Serve(request);
            }, 5);
        }
        else
        {
            TradeManager.Instance.Serve(request);
        }
    }
}
