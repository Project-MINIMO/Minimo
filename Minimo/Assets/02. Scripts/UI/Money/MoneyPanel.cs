using UnityEngine;
using TMPro;

public class MoneyPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _goldTMP;
    [SerializeField] private TextMeshProUGUI _cashTMP;
    
    private const string ThousandSuffix = "K";
    private const string MillionSuffix  = "M";
    private const string BillionSuffix  = "B";
    private const string TrillionSuffix = "T";
    
    private void Awake()
    {
        AccountInfo.Instance.Gold.OnGoldChanged += OnGoldChanged;
        AccountInfo.Instance.OnCashChanged += OnCashChanged;
        OnGoldChanged(AccountInfo.Instance.Gold.Count);
        OnCashChanged(AccountInfo.Instance.Cash);
    }
    
    private void OnGoldChanged(long value)
    {
        _goldTMP.SetText(FormatNumber(value));
    }
    
    private void OnCashChanged(long value)
    {
        _cashTMP.SetText(FormatNumber(value));
    }
    
    private string FormatNumber(long value)
    {
        return value switch
        {
            >= 1_000_000_000_000 => (value / 1_000_000_000_000f).ToString("0.#") + TrillionSuffix,
            >= 1_000_000_000 => (value / 1_000_000_000f).ToString("0.#") + BillionSuffix,
            >= 1_000_000 => (value / 1_000_000f).ToString("0.#") + MillionSuffix,
            >= 1_000 => (value / 1_000f).ToString("0.#") + ThousandSuffix,
            _ => value.ToString()
        };
    }
}
