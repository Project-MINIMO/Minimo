using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MinimoRequest
{
    public bool IsSpecial { get; private set; }
    public Item RequestedItem { get; private set; }
    public bool IsServed { get; private set; }
    public float WaitTime { get; private set; }
    public bool IsLeaving => Timer >= WaitTime;
    public float Timer { get; private set; }
    
    public MinimoRequest(bool isSpecial, Item item, float waitTime)
    {
        IsSpecial = isSpecial;
        RequestedItem = item;
        IsServed = false;
        WaitTime = waitTime;
        Timer = 0f;
    }
    
    public bool Update(float deltaTime)
    {
        Timer += deltaTime;
        return (Timer >= WaitTime);
    }

    public void Serve()
    {
        IsServed = true;
    }
}

public class TradeManager : Singleton<TradeManager>
{
    public event Action<bool> OnSaleStateChanged;
    public event Action<MinimoRequest> OnNewRequest;
    
    [HideInInspector] public List<MinimoRequest> ActiveRequests = new();
    [HideInInspector] public List<Item> TradeItems = new();
    public float CurrentTime { get; private set; }
    
    [Header("거래 관련 설정")]
    [InspectorName("전체 판매 시간")] public float SaleTime = 120f;         
    
    [Header("미니모 스폰 관련 설정")]
    [InspectorName("몇 초마다 스폰 시도")] public float SpawnInterval = 5f;          
    [InspectorName("특별 미니모 등장 확률"), Range(0f, 1f)] public float SpecialSpawnChance = 0.2f; 
    [InspectorName("동시에 존재 가능한 방문 미니모 수")] public int MaxVisitorCount = 5;            
    
    [Header("미니모 행동 관련")]
    [InspectorName("주문 후 대기 시간")] public float VisitorWaitTime = 60f; 
    
    private float _spawnTimer;
    private bool _isTrading;
    private EditManager _editManager;
    
    protected void Awake()
    {
        base.Awake();
        
        _editManager = App.GetManager<EditManager>();
    }
    
    public void StartTrade()
    {
        _isTrading = true;
        CurrentTime = SaleTime;
        _spawnTimer = 0f;
        
        OnSaleStateChanged?.Invoke(true);
    }
    
    public void EndTrade()
    {
        _isTrading = false;
        ActiveRequests.Clear();
        
        OnSaleStateChanged?.Invoke(false);
    }

    private void Update()
    {
        if (!_isTrading) return;
        
        if (CurrentTime <= 0)
        {
            EndTrade();
            return;
        }
        
        CurrentTime -= Time.deltaTime;
        
        _spawnTimer += Time.deltaTime;
        if (_spawnTimer >= SpawnInterval)
        {
            _spawnTimer = 0f;
            TrySpawnMinimo();
        }
        
        for (var i = ActiveRequests.Count - 1; i >= 0; i--)
        {
            if (ActiveRequests[i].Update(Time.deltaTime))
            {
                ActiveRequests.RemoveAt(i);
            }
        }
    }
    
    private void TrySpawnMinimo()
    {
        if (ActiveRequests.Count >= MaxVisitorCount) return;
        
        var isSpecial = Random.value < SpecialSpawnChance;
        
        var requestedItem = GetRandomRequestedItem();
        if (requestedItem == null) return;
        var request = new MinimoRequest(isSpecial, requestedItem, VisitorWaitTime);

        ActiveRequests.Add(request);
        OnNewRequest?.Invoke(request);
    }
    
    private Item GetRandomRequestedItem()
    {
        if (TradeItems.Count > 0)
        {
            var index = Random.Range(0, TradeItems.Count);
            return TradeItems[index];
        }
        else
        {
            var randomProduce = GetRandomProduce();
            if (randomProduce == null)
            {
                Debug.LogWarning("생산 가능한 아이템이 없습니다.");
                return null;
            }
            var randomItem = GetRandomItem(randomProduce);
            return randomItem;
        }
    }
    
    private ProduceObject GetRandomProduce()
    {
        return _editManager.ActiveProduces.Count == 0 
            ? null 
            : _editManager.ActiveProduces[Random.Range(0, _editManager.ActiveProduces.Count)];
    }

    private Item GetRandomItem(ProduceObject advanced)
    {
        var randomTask = advanced.ProduceData[Random.Range(0, advanced.ProduceData.Count)];
        return AccountInfo.Instance.Items[randomTask.ResultItems[0].ID];
    }

    public void Serve(MinimoRequest request)
    {
        var item = request.RequestedItem;
        if (item.Count <= 0) return;
        
        item.AddCount(-1);
        AccountInfo.Instance.Level.AddCount(item.Exp * 5);
        AccountInfo.Instance.Gold.AddCount(item.SellCost * 5);
        ActiveRequests.Remove(request);
        request.Serve();
    }
}
