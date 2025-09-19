using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OrderManager : Singleton<OrderManager>
{
    private enum OrderState
    {
        None,
        Ordering,
    }
    
    public event Action OnOrderChanged;
    
    public List<(Item item, int amount)> OrderItems = new();
    public int ProcessCount;
    public Item ProcessItem;
    public List<ProduceObject> CompletedSpots = new();
    
    private ProduceManager _produceManager;
    private EditManager _editManager;
    private MinimoManager _minimoManager;
    private TitleData _titleData;

    public ProduceData CurrentOrderOption { get; private set; }
    
    private OrderState _currentState = OrderState.None;
    private int _currentIndex = 0;

    private void Start()
    {
        _produceManager = App.GetManager<ProduceManager>();
        _editManager = App.GetManager<EditManager>();
        _minimoManager = App.GetManager<MinimoManager>();
        _titleData = App.GetData<TitleData>();
    }

    private void Update()
    {
        switch (_currentState)
        {
            case OrderState.None:
                if (TryStartOrder()) _currentState = OrderState.Ordering;
                break;
            
            case OrderState.Ordering:
                TryContinueOrder();
                TryContinueHarvest();
                break;
        }
    }
    
    public int GetAmount(Item item)
    {
        var entry = OrderItems.FirstOrDefault(x => x.item == item);
        return entry.item != null ? entry.amount : 0;
    }
    
    private bool TryStartOrder()
    {
        if (OrderItems.Count == 0) return false;

        _currentIndex = GetOrderableIndex();
        if (_currentIndex == -1) return false;

        CurrentOrderOption = _titleData.Produce.Values
            .FirstOrDefault(x => x.ResultItems[0].ID == OrderItems[_currentIndex].Item1.ID);
        if (CurrentOrderOption == null) return false;
        ProcessItem = OrderItems[_currentIndex].Item1;
        _currentState = OrderState.Ordering;
        return true;
    }

    private int GetOrderableIndex()
    {
        for (var i = 0; i < OrderItems.Count; i++)
        {
            if (IsOrderable(i))
            {
                return i;
            }
        }

        return -1;
    }
    
    private bool IsOrderable(int index)
    {
        var item = OrderItems[index].Item1;
        foreach (var material in item.MaterialCodes)
        {
            var materialItem = AccountInfo.Instance.Items[material];
            if (materialItem.Count == 0) return false;
        }

        return true;
    }
    
    private MinimoObject GetNearestMinimo(Vector3 position)
    {
        var healthyMinimos = _minimoManager.ActiveMinimos
            .Where(x => x.Agent.Energy >= 40)
            .Select(x => x.Agent)
            .ToList();
        if (!healthyMinimos.Any()) return null;
        
        MinimoObject closest = null;
        var minDistance = float.MaxValue;
        foreach (var minimo in healthyMinimos)
        {
            var dist = Vector3.Distance(minimo.transform.position, position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = minimo;
            }
        }
        return closest;
    }
    
    private void TryContinueOrder()
    {
        if (!IsOrderable(_currentIndex)) return;
        if (OrderItems[_currentIndex].amount <= 0) return;
        
        var spots = _editManager.ActiveProduces
            .Where(x => x.BuildingData.ID == OrderItems[_currentIndex].Item1.BuildingCode)
            .Where(x => x.AllTasks.Count < x.MaxSlotCount)
            .ToList();

        if (!spots.Any())
        {
            return;
        }
        
        foreach (var spot in spots)
        {
            var minimo = GetNearestMinimo(spot.transform.position);
            if (minimo == null)
            {
                continue;
            }
            minimo.ApplyState(MinimoState.Order);
        }

        OnOrderChanged?.Invoke();
    }

    private void TryContinueHarvest()
    {
        if (CompletedSpots.Count == 0) return;
        foreach (var spot in CompletedSpots)
        {
            if (ProcessCount == 0) break;
            TryStartHarvest(spot);
        }
    }
    
    public void Order(ProduceObject spot)
    {
        _produceManager.RequestPlant(spot, CurrentOrderOption, OnSuccessOrder);
        ProcessCount++;
        
        var newAmount = OrderItems[_currentIndex].amount - 1;
        if (newAmount > 0)
        {
            OrderItems[_currentIndex] = (OrderItems[_currentIndex].item, newAmount);
        }
        else
        {
            OrderItems.RemoveAt(_currentIndex);
        }

        OnOrderChanged?.Invoke();
    }
    
    private void OnSuccessOrder(ProduceTask task)
    {
        task.OnCompleted += OnCompleted;
    }
    
    private void OnCompleted(ProduceObject produceObject)
    {
        CompletedSpots.Add(produceObject);
    }

    private bool TryStartHarvest(ProduceObject produceObject)
    {
        if (ProcessCount == 0) return false;
        
        var minimo = GetNearestMinimo(produceObject.transform.position);
        if (minimo == null) return false;
        
        minimo.ApplyState(MinimoState.Harvest);
        return true;
    }

    public void Harvest(ProduceObject spot)
    {
        _produceManager.Harvest(spot);
        ProcessCount--;
        CompletedSpots.Remove(spot);
        OnOrderChanged?.Invoke();
    }
    
    public void AddOrder(Item item, int amount)
    {
        var index = OrderItems.FindIndex(x => x.item == item);
        if (index >= 0)
        {
            OrderItems[index] = (item, OrderItems[index].amount + amount);
        }
        else
        {
            OrderItems.Add((item, amount));
        }
    }
    
    public void RemoveOrder(Item item, int amount)
    {
        var index = OrderItems.FindIndex(x => x.item == item);
        if (index >= 0)
        {
            var newAmount = OrderItems[index].amount - amount;
            if (newAmount > 0)
            {
                OrderItems[index] = (item, newAmount);
            }
            else
            {
                OrderItems.RemoveAt(index);
            }
        }
    }
    
    public void InvokeOrderChanged() => OnOrderChanged?.Invoke();
}
