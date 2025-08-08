using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class TransactionHandler : MonoBehaviour
{
    [SerializeField] private Button _increaseBtn;
    [SerializeField] private Button _decreaseBtn;
    [SerializeField] private Button _transactionBtn;
        
    [SerializeField] private TextMeshProUGUI _quantityTMP;
    [SerializeField] private TextMeshProUGUI _priceTMP;
    
    protected virtual int Step => 1;
    
    protected int Quantity;
    protected int Price;
    
    protected string TransactionString;

    protected virtual void Awake()
    {
        _increaseBtn.onClick.AddListener(() => ChangeAmount(Step));
        _decreaseBtn.onClick.AddListener(() => ChangeAmount(-Step));
        _transactionBtn.onClick.AddListener(Transaction);
    }
    
    public virtual void Initialize()
    {
        UpdatePrice();
    }
    
    private void UpdatePrice()
    {
        _quantityTMP.SetText(GetQuantityText());
        
        Price = CalculatePrice();
        _priceTMP.text = string.Format(TransactionString, Price);
        
        UpdateVisibility();
    }

    protected virtual string GetQuantityText() => $"{Quantity}";
    
    protected abstract int CalculatePrice();
    
    private void UpdateVisibility()
    {
        _increaseBtn.gameObject.SetActive(Quantity < GetMaxQuantity());
        _decreaseBtn.gameObject.SetActive(Quantity > GetMinQuantity());
    }

    protected abstract int GetMaxQuantity();
    protected abstract int GetMinQuantity();
    
    protected virtual void Transaction()
    {
        gameObject.SetActive(false);
    }
    
    private void ChangeAmount(int amount)
    {
        Quantity += amount;
        UpdatePrice();
    }
    
}
