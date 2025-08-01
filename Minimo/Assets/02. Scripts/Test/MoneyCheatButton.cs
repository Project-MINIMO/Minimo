using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyCheatButton : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Button _moneyCheatButton;
    
    private void Awake()
    {
        _moneyCheatButton.onClick.AddListener(OnMoneyCheatButtonClicked);
    }
    
    private void OnMoneyCheatButtonClicked()
    {
        // Add 1000 gold and 100 cash to the account
        AccountInfo.Instance.Gold.AddCount(10000);
        AccountInfo.Instance.AddCash(10000);
    }
}
