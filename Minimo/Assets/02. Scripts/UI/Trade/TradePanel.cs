using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TradePanel : UIBase
{
    public override bool IsDefaultPanel => true;
    [SerializeField] private Button _toggleBtn;
    [SerializeField] private GameObject _buttonBack;
    [SerializeField] private Button _menuBtn;
    [SerializeField] private Button _startBtn;
    [SerializeField] private Button _endBtn;

    private TradeMenuSlot[] _slots;
    
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);
        
        _slots = GetComponentsInChildren<TradeMenuSlot>(true);
        
        _toggleBtn.onClick.AddListener(() =>
        {
            var isActive = _buttonBack.activeSelf;
            _buttonBack.SetActive(!isActive);
        });
       
        _menuBtn.onClick.AddListener(() =>
        {
            _buttonBack.SetActive(false);
        });
        _startBtn.onClick.AddListener(() =>
        {
            _buttonBack.SetActive(false);
            TradeManager.Instance.StartTrade();
        });
        _endBtn.onClick.AddListener(() =>
        {
            _buttonBack.SetActive(false);
            TradeManager.Instance.EndTrade();
        });

        _buttonBack.SetActive(false);
        OnSaleStateChanged(false);
        
        TradeManager.Instance.OnSaleStateChanged += OnSaleStateChanged;
    }
    
    private void OnSaleStateChanged(bool isOnSale)
    {
        _menuBtn.gameObject.SetActive(!isOnSale);
        _startBtn.gameObject.SetActive(!isOnSale);
        _endBtn.gameObject.SetActive(isOnSale);

        if (!isOnSale)
        {
            foreach (var slot in _slots)
            {
                TradeManager.Instance.TradeItems.Remove(slot.Item);
                slot.ClearItem();
            }
        }
    }
}
