using System.Linq;
using System.Collections.Generic;

using UnityEngine;

public class MinimoSpawner : MonoBehaviour
{
    [SerializeField] private CameraBoundsUpdater _mapBounds;
    [SerializeField] private GameObject[] _minimoPrefabs;
    
    private List<Minimo> _minimoDatas;
    private readonly List<MinimoObject> _activeMinimos = new(); 
    private readonly Queue<Minimo> _pendingQueue = new(); 
    private const int MaxMinimoCount = 10;

    private int _prevLevel;
    
    private IndicatorPanel _indicatorPanel;

    private void Awake()
    {
        _minimoDatas = App.GetData<TitleData>().UserMinimo.Values.ToList();
        AccountInfo.Instance.Level.OnLevelUp += OnLevelUp;
        _indicatorPanel = App.GetManager<UIManager>().GetPanel<IndicatorPanel>();
    }

    private void Start()
    {
        OnLevelUp(AccountInfo.Instance.Level.Count);
    }

    private void OnLevelUp(int level)
    {
        if (_minimoDatas.Count <= 0) return;
        
        for (var i = 0; i < level - _prevLevel; i++)
        {
            var data = _minimoDatas[Random.Range(0, _minimoDatas.Count)];
            _minimoDatas.Remove(data);
            EnqueueOrSpawn(data);
        }
        
        _prevLevel = level;
    }

    private void EnqueueOrSpawn(Minimo data)
    {
        if (_activeMinimos.Count < MaxMinimoCount)
        {
            SpawnMinimo(data);
        }
        else
        {
            _pendingQueue.Enqueue(data);
        }
    }
    
    private void SpawnMinimo(Minimo data)
    {
        var spawnPos = _mapBounds.GetRandomOutsideMapPoint();
        var randomPrefab = _minimoPrefabs[Random.Range(0, _minimoPrefabs.Length)];
        var minimoObject = Instantiate(randomPrefab, spawnPos, Quaternion.identity, transform);

        var instance = minimoObject.GetComponent<MinimoObject>();
        instance.Initialize(data);
        _activeMinimos.Add(instance);
        
        instance.OnAcquired += HandleMinimoAcquired;
        _indicatorPanel.CreateIndicator(minimoObject.transform);
    }
 
    private void HandleMinimoAcquired(MinimoObject instance)
    {
        instance.OnAcquired -= HandleMinimoAcquired;
        _activeMinimos.Remove(instance);
        TrySpawnFromQueue();
    }
    
    private void TrySpawnFromQueue()
    {
        if (_pendingQueue.Count == 0 || _activeMinimos.Count >= MaxMinimoCount) return;

        var nextData = _pendingQueue.Dequeue();
        SpawnMinimo(nextData);
    }
}
