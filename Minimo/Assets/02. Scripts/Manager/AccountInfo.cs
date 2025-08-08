using System;
using System.Linq;

using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class AccountInfo : Singleton<AccountInfo>
{
    public Dictionary<int, Item> Items { get; private set; }
    public UserLevel Level { get; private set; }
    public UserGold Gold { get; private set; }
    public long Cash { get; private set; } = 100;
    public int StorageCapacity { get; private set; } = 100;
    public int MinimoCapacity { get; private set; } = 5;
    public Vector2Int straySpeed = new(5, 7);

    private GetItemPanel _itemPanel;
    public event Action<int> OnStorageCapacityChanged;
    public event Action<int> OnMinimoCapacityChanged;
    public event Action<long> OnCashChanged; 
    public int CurrentItemCounts => Items.Values.Where(item => item.Level > 0).Sum(item => item.Count);
    public bool Tutorial = true;
    public bool Prolog = true;
    
    [SerializeField] private Sprite LevelIcon;
    [SerializeField] private Sprite GoldIcon;

    protected override void Awake()
    {
        base.Awake();
        
        Level = new UserLevel(LevelIcon);
        Gold = new UserGold(GoldIcon);
        
        // Firebase에서 초기 SLP 값 로드
        InitializeCashFromFirebase();
    }
    
    private void InitializeCashFromFirebase()
    {
        var firebaseManager = App.GetManager<FirebaseManager>();
        if (firebaseManager != null)
        {
            // 초기 SLP 값 설정
            Cash = firebaseManager.GetCurrentSLP();
            
            // SLP 업데이트 이벤트 구독
            firebaseManager.OnSLPUpdate += OnSLPUpdate;
        }
        else
        {
            Debug.LogWarning("FirebaseManager is not initialized. Using default cash value.");
        }
    }
    
    private void OnSLPUpdate(int newSLP)
    {
        Cash = newSLP;
        OnCashChanged?.Invoke(Cash);
    }

    public void AddItems(Dictionary<int, Item> items)
    {
        Items = items;
    }
    
    public bool CanKeepItem(int id)
    {
        var cankeep = CurrentItemCounts < StorageCapacity;
        if (!cankeep)
        {
            App.Notification(NotifyType.StorageCapacityLack);
        }

        return cankeep;
    }

    public void AddItem(int id, int amount, Transform trans)
    {
        Items[id].AddCount(amount);

        if (_itemPanel == null)
        {
            _itemPanel = App.GetManager<UIManager>().GetItem;
        }
        _itemPanel.EnqueueItem(id, trans);
        OnStorageCapacityChanged?.Invoke(StorageCapacity);
    }

    public void AddItem(Item item, int amount)
    {
        item.AddCount(amount);
        OnStorageCapacityChanged?.Invoke(StorageCapacity);
    }

    public void RemoveItem(int id, int amount)
    {
        Items[id].AddCount(-amount);
        OnStorageCapacityChanged?.Invoke(StorageCapacity);
    }
    
    public void RemoveItem(Item item, int amount)
    {
        item.AddCount(-amount);
        OnStorageCapacityChanged?.Invoke(StorageCapacity);
    }

    public void AddStorageCapacity(int amount)
    {
        StorageCapacity += amount;
        OnStorageCapacityChanged?.Invoke(StorageCapacity);
    }
    
    public void AddMinimoCapacity(int amount)
    {
        MinimoCapacity += amount;
        OnMinimoCapacityChanged?.Invoke(MinimoCapacity);
    }
    
    // SLP(Cash) 추가 메서드
    public async void AddCash(int amount)
    {
        var firebaseManager = App.GetManager<FirebaseManager>();
        if (firebaseManager != null)
        {
            bool success = await firebaseManager.UpdateSLP(amount, "add");
            if (!success)
            {
                Debug.LogError("Failed to update SLP in Firebase");
            }
        }
        else
        {
            // Firebase 매니저가 없는 경우 로컬에서만 업데이트
            Cash += amount;
            Cash = (long)Mathf.Clamp(Cash, 0, long.MaxValue);
            OnCashChanged?.Invoke(Cash);
        }
    }
    
    [ContextMenu("AddGold1000000")]
    public void AddGold1000000()
    {
        Gold.AddCount(1000000);
    }
    
    [ContextMenu("AddLevel1")]
    public void AddLevel1()
    {
        Level.AddCount(100);
    }
    
    [ContextMenu("AddCash1000")]
    public void AddCash1000()
    {
        AddCash(1000);
    }
}
