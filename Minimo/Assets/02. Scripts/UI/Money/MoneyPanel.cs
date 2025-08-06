using UnityEngine;
using TMPro;

public class MoneyPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _goldTMP;
    [SerializeField] private TextMeshProUGUI _cashTMP;

    private void Awake()
    {
        AccountInfo.Instance.Gold.OnGoldChanged += OnGoldChanged;
        AccountInfo.Instance.OnCashChanged += OnCashChanged;
        OnGoldChanged(AccountInfo.Instance.Gold.Count);
        OnGoldChanged(AccountInfo.Instance.Cash);
    }
    
    private void OnGoldChanged(long value)
    {
        _goldTMP.SetText(value.ToString());
    }
    
    private void OnCashChanged(long value)
    {
        _cashTMP.SetText(value.ToString());
    }
}
