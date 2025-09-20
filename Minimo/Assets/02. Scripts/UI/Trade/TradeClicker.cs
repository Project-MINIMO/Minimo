using System;
using UnityEngine;
using UnityEngine.UI;

public class TradeClicker : MonoBehaviour
{
    [SerializeField] private Button targetButton;
    [SerializeField] private int requiredClicks = 5; 
    private int _currentClicks = 0;
    private Action _triggerFunction;
    
    private void Start()
    {
        targetButton.onClick.AddListener(OnButtonClicked);
    }
    
    public void StartClicking(Action triggerFunction, int clicksNeeded)
    {
        requiredClicks = clicksNeeded;
        _currentClicks = 0;
        _triggerFunction = triggerFunction;
        gameObject.SetActive(true);
    }

    private void OnButtonClicked()
    {
        _currentClicks++;
        
        if (_currentClicks >= requiredClicks)
        {
            TriggerFunction();
            _currentClicks = 0;
        }
    }

    private void TriggerFunction()
    {
        _triggerFunction?.Invoke();
        gameObject.SetActive(false);
    }
}
