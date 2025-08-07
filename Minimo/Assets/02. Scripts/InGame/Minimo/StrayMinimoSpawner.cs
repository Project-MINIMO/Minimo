using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Random = UnityEngine.Random;

public class StrayMinimoSpawner : MonoBehaviour
{
    [SerializeField] private CameraBoundsUpdater _mapBounds; 
    
    [SerializeField] private float _spawnInterval = 5; 
    [SerializeField] private float _spawnProbability = 0.5f;
    [SerializeField] private float _respawnCooldown = 30;
    
    private List<StrayMinimoObject> _minimoPool;
    private StrayMinimoObject _currentMinimo;
    private IndicatorPanel _indicatorPanel;
    private PopUpPanel _popUpPanel;
    
    private EditManager _editManager;
    private float _goldenSpawnRate;
    private float _lastDeathTime = float.MinValue;

    private void Awake()
    {
        _editManager = App.GetManager<EditManager>();
        var uiManager = App.GetManager<UIManager>();
        _indicatorPanel = uiManager.GetPanel<IndicatorPanel>();
        _popUpPanel = uiManager.GetPanel<PopUpPanel>();
        
        var common = App.GetData<TitleData>().Common;
        _goldenSpawnRate = common["GoldMiaSpawnRate"] / 100f;
        
        _minimoPool = GetComponentsInChildren<StrayMinimoObject>().ToList();

        var i = 0;
        
        for (; i < _minimoPool.Count / 2; i++)
        {
            _minimoPool[i].Initialize(true, common);
            _minimoPool[i].OnDead += OnDead;
        }
        
        for (; i < _minimoPool.Count; i++)
        {
            _minimoPool[i].Initialize(false, common);
            _minimoPool[i].OnDead += OnDead;
        }
        
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(_spawnInterval); 

            if (_currentMinimo != null) continue;
            if (Time.time - _lastDeathTime < _respawnCooldown) continue;
            if (!IsAnyCompleteAdvances()) continue;
            if (Random.Range(0f, 1f) > _spawnProbability) continue;

            SpawnMinimo();
        }
    }

    private void SpawnMinimo()
    {
        var spawnPos = GetRandomSpawnPosition();

        var isGolden = Random.Range(0f, 1f) < _goldenSpawnRate;
        var strayMinimo = _minimoPool.FirstOrDefault(x => x.IsGolden == isGolden);
        if (strayMinimo == null) return;

        _currentMinimo = strayMinimo;
        _minimoPool.Remove(strayMinimo);

        _currentMinimo.Spawn(spawnPos);
        
        _indicatorPanel.CreateStrayIndicator(strayMinimo.transform);
        _popUpPanel.OpenPanel(PopUpType.StrayWarning);
    }
    
    private Vector3 GetRandomSpawnPosition()
    {
        var corners = _mapBounds.GetCorners();
        return corners[Random.Range(0, corners.Length)];
    }
    
    private bool IsAnyCompleteAdvances()
    {
        return _editManager.ActiveProduces.Any(x => x.CurrentState == ProduceState.Complete);
    }

    private void OnDead(bool isFail)
    {
        _currentMinimo = null;
        _lastDeathTime = Time.time;
    }
}