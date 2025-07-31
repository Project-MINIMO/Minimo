using System.Linq;
using System.Collections.Generic;

using UnityEngine;

public class MinimoSpawner : MonoBehaviour
{
    [SerializeField] private CameraBoundsUpdater _mapBounds;
    [SerializeField] private GameObject _minimoPrefab;
    
    private List<Minimo> _minimoDatas;
    private readonly List<MinimoObject> _activeMinimos = new(); 
    private readonly Queue<Minimo> _pendingQueue = new(); 
    private const int MaxMinimoCount = 10;

    private int _prevLevel;

    private void Awake()
    {
        _minimoDatas = App.GetData<TitleData>().UserMinimo.Values.ToList();
        AccountInfo.Instance.Level.OnLevelUp += OnLevelUp;
    }

    private void Start()
    {
        OnLevelUp(AccountInfo.Instance.Level.Count);
    }

    private void OnLevelUp(int level)
    {
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
        var minimoObject = Instantiate(_minimoPrefab, spawnPos, Quaternion.identity, transform);

        var instance = minimoObject.GetComponent<MinimoObject>();
        instance.Initialize(data);
        _activeMinimos.Add(instance);
        
        instance.OnAcquired += HandleMinimoAcquired;
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
