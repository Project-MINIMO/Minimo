using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderSlot : MonoBehaviour
{
    public event Action<OrderSlot> OnSlotSelected;
    public Item Item { get; private set; }
    public int Amount => OrderManager.Instance.OrderItems.Count(x => x == Item.ID);
    
    [SerializeField] private Button _slotBtn;
    [SerializeField] private ItemInfoUpdater _itemInfo;
    [SerializeField] private ItemInfoUpdater[] _materialInfos;
    [SerializeField] private TextMeshProUGUI _orderTMP;
    [SerializeField] private TextMeshProUGUI _processTMP;
    
    private void Start()
    {
        _slotBtn.onClick.AddListener(() => OnSlotSelected?.Invoke(this));
        OrderManager.Instance.OnOrderChanged += OnOrderChanged;
        _orderTMP.gameObject.SetActive(false);
        _processTMP.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (Item != null)
        {
            _itemInfo.UpdateItem(Item);
            var i = 0;
            for (; i < Item.MaterialCodes.Length; i++)
            {
                _materialInfos[i].gameObject.SetActive(true);
                _materialInfos[i].UpdateItem(Item.MaterialCodes[i]);
            }
            for (; i < _materialInfos.Length; i++)
            {
                _materialInfos[i].gameObject.SetActive(false);
            }
        }
    }

    public void SetItem(Item item)
    {
        Item = item;
        
        _itemInfo.UpdateItem(item);
        var i = 0;
        for (; i < item.MaterialCodes.Length; i++)
        {
            _materialInfos[i].gameObject.SetActive(true);
            _materialInfos[i].UpdateItem(item.MaterialCodes[i]);
        }
        for (; i < _materialInfos.Length; i++)
        {
            _materialInfos[i].gameObject.SetActive(false);
        }
    }

    private void OnOrderChanged()
    {
        if (Item == null) return;
        
        var orderAmount = Amount;
        var processAmount = OrderManager.Instance.ProcessItems.Count(x => x == Item.ID);

        if (orderAmount == 0 && processAmount == 0)
        {
            Item = null;
            _itemInfo.ClearItem();
            foreach (var materialInfo in _materialInfos)
            {
                materialInfo.ClearItem();
            }
            _orderTMP.gameObject.SetActive(false);
            _processTMP.gameObject.SetActive(false);
            return;
        }
            
        _orderTMP.gameObject.SetActive(true);
        _processTMP.gameObject.SetActive(true);
        _orderTMP.text = $"주문 : {orderAmount}";
        _processTMP.text = $"처리 중 : {processAmount}";
    }
}
