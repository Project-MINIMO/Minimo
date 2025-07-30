using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class StrayMinimoSpawner : MonoBehaviour
{
    [SerializeField] private Tilemap _groundTilemap; 
    
    private List<StrayMinimoObject> _minimoPool;
    private StrayMinimoObject _currentMinimo;
    
    private float _spawnInterval;
    private float _goldenSpawnRate;

    private void Awake()
    {
        var common = App.GetData<TitleData>().Common;
        _spawnInterval = common["MiaSpawnInterval"];
        _goldenSpawnRate = common["GoldMiaSpawnRate"] / 100f;
        
        _minimoPool = GetComponentsInChildren<StrayMinimoObject>().ToList();

        var i = 0;
        
        for (; i < _minimoPool.Count / 2; i++)
        {
            _minimoPool[i].Initialize(true, common);
        }
        
        for (; i < _minimoPool.Count; i++)
        {
            _minimoPool[i].Initialize(false, common);
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
        var spawnPos = GetRandomSpawnPosition();
        
        var isGolden = Random.Range(0, 1f) < _goldenSpawnRate;
        var strayMinimo = _minimoPool.First(x => x.IsGolden == isGolden);
        
        DespawnCurrentMinimo();

        strayMinimo.Spawn(spawnPos);
        _currentMinimo = strayMinimo;
        _minimoPool.Remove(_currentMinimo);
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
    
    private Vector3 GetRandomSpawnPosition()
    {
        var bounds = _groundTilemap.cellBounds;
        Vector3Int cell;

        do
        {
            var x = Random.Range(bounds.xMin, bounds.xMax);
            var y = Random.Range(bounds.yMin, bounds.yMax);
            cell  = new Vector3Int(x, y, 0);
        }
        while (_groundTilemap.HasTile(cell));
        
        return _groundTilemap.GetCellCenterWorld(cell);
    }
}
