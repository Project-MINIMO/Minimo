using System;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager : Singleton<OrderManager>
{
    public event Action OnOrderChanged;
    
    public List<int> OrderItems = new();
    public List<int> ProcessItems = new();
    
    public void AddOrder(int itemId)
    {
        OrderItems.Add(itemId);
    }
    
    public void RemoveOrder(int itemId)
    {
        OrderItems.Remove(itemId);
    }
    
    public void InvokeOrderChanged() => OnOrderChanged?.Invoke();
}
