using System.Collections;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

public class MinimoSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] _minimoPrefabs;
    [SerializeField] private float _spawnDelaySeconds = 10f;
    
    private List<Minimo> _minimoDatas;
    private readonly Queue<Minimo> _spawnQueue = new();
    private readonly Dictionary<int, MinimoObject> _minimoInstances = new();
    private MinimoObject _currentMinimo;
    
    private IndicatorPanel _indicatorPanel;
    private Coroutine _spawnCoroutine;
    private int _prevLevel = 4;

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
            _spawnQueue.Enqueue(data);
        }

        if (level >= _prevLevel)
        {
            _prevLevel = level;
        }
        
        if (_spawnCoroutine == null && _currentMinimo == null)
        {
            _spawnCoroutine = StartCoroutine(SpawnMinimoRoutine());
        }
    }
    
    private IEnumerator SpawnMinimoRoutine()
    {
        while (_spawnQueue.Count > 0)
        {
            var data = _spawnQueue.Dequeue();
            SpawnMinimo(data);
            
            yield return new WaitUntil(() => _currentMinimo == null);
            yield return new WaitForSeconds(_spawnDelaySeconds);
        }

        _spawnCoroutine = null;
    }

    private void SpawnMinimo(Minimo data)
    {
        var randomNum = Random.Range(0, _minimoPrefabs.Length);
        MinimoObject instance;
        
        if (_minimoInstances.TryGetValue(randomNum, out var cachedInstance))
        {
            instance = cachedInstance;
            _minimoInstances.Remove(randomNum);
        }
        else
        {
            var randomPrefab = _minimoPrefabs[randomNum];
            var minimoObject = Instantiate(randomPrefab, Vector3.one * 999, Quaternion.identity, transform);
            instance = minimoObject.GetComponent<MinimoObject>();
        }
        
        instance.Initialize(data, randomNum);
        _currentMinimo = instance;
        
        instance.OnAcquired += HandleMinimoAcquired;
        instance.OnExpired += HandleMinimoExpired;
        
        _indicatorPanel.CreateIndicator(instance.transform);
    }
    
    public void SpawnTutorialMinimo()
    {
        var data = _minimoDatas[Random.Range(0, _minimoDatas.Count)];
        _minimoDatas.Remove(data);
        
        var randomNum = Random.Range(0, _minimoPrefabs.Length);
        MinimoObject instance;
        
        if (_minimoInstances.TryGetValue(randomNum, out var cachedInstance))
        {
            instance = cachedInstance;
            _minimoInstances.Remove(randomNum);
        }
        else
        {
            var randomPrefab = _minimoPrefabs[randomNum];
            var minimoObject = Instantiate(randomPrefab, new Vector3(-4, 0.75f, 0), Quaternion.identity, transform);
            instance = minimoObject.GetComponent<MinimoObject>();
        }
        
        instance.Initialize(data, randomNum, MinimoState.Idle);
    }
 
    private void HandleMinimoAcquired(MinimoObject instance)
    {
        if (_currentMinimo != instance) return;
        
        instance.OnAcquired -= HandleMinimoAcquired;
        instance.OnExpired -= HandleMinimoExpired;
        
        _currentMinimo = null;
    }
    
    private void HandleMinimoExpired(MinimoObject instance)
    {
        if (_currentMinimo != instance) return;

        instance.OnAcquired -= HandleMinimoAcquired;
        instance.OnExpired -= HandleMinimoExpired;

        _spawnQueue.Enqueue(_currentMinimo.Data);
        _minimoInstances.Add(_currentMinimo.MinimoIndex, _currentMinimo);
        _currentMinimo = null;
    }
}
