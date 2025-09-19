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
        Waiting,
        Completed
    }
    
    public event Action OnOrderChanged;
    
    public List<int> OrderItems = new();
    public List<int> ProcessItems = new();
    
    private ProduceManager _produceManager;
    private EditManager _editManager;
    private MinimoManager _minimoManager;
    
    public ProduceObject CurrentOrderSpot { get; private set; }
    public ProduceData CurrentOrderOption { get; private set; }
    
    private OrderState _currentState = OrderState.None;
    private int _currentIndex = 0;

    private void Start()
    {
        _produceManager = App.GetManager<ProduceManager>();
        _editManager = App.GetManager<EditManager>();
        _minimoManager = App.GetManager<MinimoManager>();
    }

    private void Update()
    {
        switch (_currentState)
        {
            case OrderState.None:
                if (TryStartOrder()) _currentState = OrderState.Waiting;
                break;

            case OrderState.Waiting:
                return;
            
            case OrderState.Completed:
                if (TryStartHarvest()) _currentState = OrderState.Waiting;
                break;
        }
    }
    
    private bool TryStartOrder()
    {
        if (OrderItems.Count == 0) return false;
        _currentIndex = GetOrderableIndex();
        if (_currentIndex == -1) return false;
        CurrentOrderSpot = GetOrderSpot();
        if(CurrentOrderSpot == null) return false;
        
        CurrentOrderOption = CurrentOrderSpot.ProduceData
            .FirstOrDefault(x => x.ResultItems[0].ID == OrderItems[0]);
        
        var minimo = GetNearestMinimo();
        if (minimo == null)
        {
            CurrentOrderSpot = null;
            CurrentOrderOption = null;
            return false;
        }
        minimo.ApplyState(MinimoState.Order);
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
        var item = AccountInfo.Instance.Items[OrderItems[index]];
        foreach (var material in item.MaterialCodes)
        {
            var materialItem = AccountInfo.Instance.Items[material];
            if (materialItem.Count == 0) return false;
        }

        return true;
    }

    private ProduceObject GetOrderSpot()
    {
        var spots = _editManager.ActiveProduces
            .Where(x => x.BuildingData.ID == AccountInfo.Instance.Items[OrderItems[0]].BuildingCode)
            .Where(x => x.AllTasks.Count < x.MaxSlotCount)
            .ToList();
        if (!spots.Any()) return null;
        
        var idleSpot = spots.FirstOrDefault(x => x.CurrentState == ProduceState.Idle);
        return idleSpot != null ? idleSpot : spots[0];
    }
    
    private MinimoObject GetNearestMinimo()
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
            var dist = Vector3.Distance(minimo.transform.position, CurrentOrderSpot.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = minimo;
            }
        }
        return closest;
    }
    
    private bool TryStartHarvest()
    {
        if (ProcessItems.Count == 0) return false;
        
        var minimo = GetNearestMinimo();
        if (minimo == null) return false;
        
        minimo.ApplyState(MinimoState.Harvest);
        return true;
    }

    public void AddOrder(int itemId)
    {
        OrderItems.Add(itemId);
    }
    
    public void RemoveOrder(int itemId)
    {
        OrderItems.Remove(itemId);
    }
    
    public void InvokeOrderChanged() => OnOrderChanged?.Invoke();

    public void Order()
    {
        _produceManager.RequestPlant(CurrentOrderSpot, CurrentOrderOption, OnSuccessOrder);
        ProcessItems.Add(OrderItems[0]);
        OrderItems.RemoveAt(0);
        
        OnOrderChanged?.Invoke();
    }
    
    private void OnSuccessOrder(ProduceTask task)
    {
        task.OnStateChanged += OnStateChanged;
    }
    
    private void OnStateChanged(ITaskState state)
    {
        if (state == CompletedState.Instance)
        {
            _currentState = OrderState.Completed;
        }
    }
    
    public void FailOrder()
    {
        CurrentOrderSpot = null;
        CurrentOrderOption = null;
        _currentState = OrderState.None;
    }

    public void Harvest()
    {
        _produceManager.Harvest(CurrentOrderSpot);
        ProcessItems.RemoveAt(0);
        
        CurrentOrderSpot = null;
        CurrentOrderOption = null;
        _currentState = OrderState.None;
        
        OnOrderChanged?.Invoke();
    }

    public void FailHarvest()
    {
        _currentState = OrderState.Completed;
    }
}
