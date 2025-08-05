using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using DG.Tweening;

public class VisitMinimoSpawner : MonoBehaviour
{
    [SerializeField] private CameraBoundsUpdater _mapBounds;
    [SerializeField] private Transform _spaceshipTrans;
    [SerializeField] private GameObject _spaceshipShineObj;
    
    private List<VisitMinimoObject> _minimoPool;
    private VisitMinimoObject _currentMinimo;
    
    private VisitMinimoObject _spawnedMinimo;
    private List<VisitMinimoObject> _despawnedMinimos = new();

    private EditManager _editManager;
    private float _spawnInterval;
    private bool _isSpaceshipSpawning;

    private void Awake()
    {
        _spawnInterval = 180;

        _editManager = App.GetManager<EditManager>();
        _minimoPool = GetComponentsInChildren<VisitMinimoObject>(true).ToList();
        foreach (var minimo in _minimoPool)
        {
            minimo.Initialize(this);
        }

        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(_spawnInterval);
            SpawnMinimo();
        }
    }

    private void SpawnMinimo()
    {
        var randomProduce = GetRandomAdvanced();
        if (randomProduce == null) return;
        var randomItem = GetRandomItem(randomProduce);
        
        var visitMinimo = _minimoPool[0];
        
        DespawnCurrentMinimo();

        visitMinimo.Spawn(randomItem, randomProduce);
        _currentMinimo = visitMinimo;
        _spawnedMinimo = visitMinimo;
        
        if (!_isSpaceshipSpawning)
        {
            StartCoroutine(SpawnSpaceship());
        }
        
        _minimoPool.Remove(_currentMinimo);
    }

    private ProduceObject GetRandomAdvanced()
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

    private void DespawnCurrentMinimo()
    {
        if (_currentMinimo != null)
        {
            _currentMinimo.Despawn();
            _minimoPool.Add(_currentMinimo);
            _currentMinimo = null;
        }
    }

    public void CallSpaceship(VisitMinimoObject minimo)
    {
        _despawnedMinimos.Add(minimo);

        if (!_isSpaceshipSpawning)
        {
            StartCoroutine(SpawnSpaceship());
        }
    }

    private IEnumerator SpawnSpaceship()
    {
        _isSpaceshipSpawning = true;
        _spaceshipTrans.position = new Vector3(_mapBounds.GetOutsideMapWidth().Item1 - 2, 2.68f, 0);
        _spaceshipTrans.DOMoveX(1, 2f);
        yield return new WaitForSeconds(2);
        _spaceshipShineObj.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        if (_spawnedMinimo != null)
        {
            _spawnedMinimo.Land();
            _spawnedMinimo = null;
        }
        foreach (var minimo in _despawnedMinimos.ToList())
        {
            minimo.Depart();
            _despawnedMinimos.Remove(minimo);
        }
        yield return new WaitForSeconds(2);
        
        _spaceshipShineObj.SetActive(false);
        _spaceshipTrans.DOMoveX(_mapBounds.GetOutsideMapWidth().Item2 + 2, 2f);
        yield return new WaitForSeconds(2);
        _isSpaceshipSpawning = false;

        if (_spawnedMinimo != null || _despawnedMinimos.Any())
        {
            StartCoroutine(SpawnSpaceship());
        }
    }
}
