using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderBack : MonoBehaviour
{
    [SerializeField] private ItemInfoUpdater _itemInfo;
    [SerializeField] private ItemInfoUpdater[] _materialInfos;
    [SerializeField] private Button _increaseBtn;
    [SerializeField] private Button _decreaseBtn;
    [SerializeField] private TextMeshProUGUI _amountTMP;
    [SerializeField] private Button _cancelBtn;
    [SerializeField] private Button _confirmBtn;

    [SerializeField] private GameObject _inventoryBack;

    private int _quantity = 1;
    private OrderSlot _currentSlot;
    private Item _currentItem;
    
    private void Start()
    {
        _increaseBtn.onClick.AddListener(() => ChangeAmount(_quantity + 1));
        _decreaseBtn.onClick.AddListener(() => ChangeAmount(_quantity - 1));
        
        _cancelBtn.onClick.AddListener(() =>
        {
            _currentSlot = null;
            _currentItem = null;
            gameObject.SetActive(false);
        });
        _confirmBtn.onClick.AddListener(() =>
        {
            _currentSlot.SetItem(_currentItem);
            var count = OrderManager.Instance.GetAmount(_currentItem);
            if (count > _quantity)
            {
                OrderManager.Instance.RemoveOrder(_currentItem, count - _quantity);
            }
            else if (count < _quantity)
            {
                OrderManager.Instance.AddOrder(_currentItem, _quantity - count);
            }
            OrderManager.Instance.InvokeOrderChanged();
            _inventoryBack.SetActive(false);
            gameObject.SetActive(false);
        });
    }

    public void Initialize(Item item, OrderSlot slot)
    {
        gameObject.SetActive(true);
        _currentSlot = slot;
        _currentItem = item;
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

        ChangeAmount(1);
    }

    public void Initialize(OrderSlot slot)
    {
        Initialize(slot.Item, slot);
        ChangeAmount(slot.Amount);
    }
    
    private void ChangeAmount(int amount)
    {
        _quantity = amount;
        _amountTMP.text = _quantity.ToString();
        _increaseBtn.gameObject.SetActive(_quantity < 99);
        _decreaseBtn.gameObject.SetActive(_quantity > 0);
    }
}
