using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class VisitMinimoSpawner : MonoBehaviour
{
    private List<VisitMinimoObject> _minimoPool;
    private VisitMinimoObject _currentMinimo;

    private EditManager _editManager;
    private float _spawnInterval;

    private void Awake()
    {
        _spawnInterval = 35;

        _editManager = App.GetManager<EditManager>();
        _minimoPool = GetComponentsInChildren<VisitMinimoObject>().ToList();

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
        var randomAdvanced = GetRandomAdvanced();
        if (randomAdvanced == null) return;
        var randomItem = GetRandomItem(randomAdvanced);
        
        var visitMinimo = _minimoPool[0];
        
        DespawnCurrentMinimo();

        visitMinimo.Spawn(randomItem, randomAdvanced);
        _currentMinimo = visitMinimo;
        _minimoPool.Remove(_currentMinimo);
    }

    private ProduceAdvanced GetRandomAdvanced()
    {
        return _editManager.ActiveAdvanceds.Count == 0 
            ? null 
            : _editManager.ActiveAdvanceds[Random.Range(0, _editManager.ActiveAdvanceds.Count)];
    }

    private Item GetRandomItem(ProduceAdvanced advanced)
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
}
