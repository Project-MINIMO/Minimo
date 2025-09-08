using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StarManager : ManagerBase
{
    private readonly struct StarPair
    {
        private readonly Star A;
        private readonly Star B;

        public StarPair(Star a, Star b)
        {
            if (a.GetInstanceID() < b.GetInstanceID())
            {
                A = a;
                B = b;
            }
            else
            {
                A = b;
                B = a;
            }
        }

        public bool Contains(Star star) => A == star || B == star;
        public override bool Equals(object obj) => obj is StarPair other && A == other.A && B == other.B;
        public override int GetHashCode() => (A, B).GetHashCode();
    }

    [SerializeField] private CameraBoundsUpdater _mapBounds;
    [SerializeField] private Star _starObj;

    [SerializeField] private LineObject _lineObjectPrefab;
    [SerializeField] private Transform _lineParent;
    [SerializeField] private float _connectDistance = 2f;

    public bool IsConnected => _activeLines.Count > 0;
    public List<Star> Stars { get; } = new();
    private readonly Dictionary<StarPair, LineObject> _activeLines = new();
    private readonly Queue<LineObject> _linePool = new();
    private readonly Queue<Star> _starPool = new();  

    private ConstellationPanel _constellationPanel;
    private IndicatorPanel _indicatorPanel;

    private int _prevLevel;
    private bool _addStarMode;

    private void Start()
    {
        var uiManager = App.GetManager<UIManager>();
        _constellationPanel = uiManager.GetPanel<ConstellationPanel>();
        _indicatorPanel = uiManager.GetPanel<IndicatorPanel>();

        AccountInfo.Instance.Level.OnLevelUp += OnLevelUp;
        OnLevelUp(AccountInfo.Instance.Level.Count);
    }
    
    private void OnLevelUp(int level)
    {
        /*
        for (var i = 0; i < level - _prevLevel; i++)
        {
            var newStar = Instantiate(_starObj,
                _mapBounds.GetRandomOutsideMapPoint(),
                Quaternion.identity,
                transform);
            newStar.Initialize(this, _constellationPanel);
            Stars.Add(newStar);
            _indicatorPanel.CreateIndicator(newStar.transform);
        }

        _prevLevel = level;
        */
    }

    public void UpdateConnections(Star draggedStar, Vector3 position)
    {
        float maxDist2 = _connectDistance * _connectDistance;

        var pairsToRemove = _activeLines.Keys
            .Where(pair => pair.Contains(draggedStar))
            .ToList();

        foreach (var pair in pairsToRemove)
        {
            var line = _activeLines[pair];
            line.gameObject.SetActive(false);
            _linePool.Enqueue(line);
            _activeLines.Remove(pair);
        }

        foreach (var other in Stars)
        {
            if (other == draggedStar) continue;

            if ((other.transform.position - draggedStar.transform.position).sqrMagnitude <= maxDist2)
            {
                var pair = new StarPair(draggedStar, other);

                if (!_activeLines.ContainsKey(pair))
                {
                    var lineObj = _linePool.Count > 0
                        ? _linePool.Dequeue()
                        : Instantiate(_lineObjectPrefab, _lineParent);

                    lineObj.gameObject.SetActive(true);

                    lineObj.Initialize(
                        draggedStar.transform.position,
                        other.transform.position,
                        draggedStar.ActiveColor,
                        other.ActiveColor,
                        draggedStar,
                        other,
                        this // pass spawner reference
                    );

                    _activeLines[pair] = lineObj;
                }
            }
        }
    }

    public void RemoveLine(LineObject line)
    {
        var toRemove = _activeLines.FirstOrDefault(kv => kv.Value == line);
        if (!toRemove.Equals(default(KeyValuePair<StarPair, LineObject>)))
        {
            _activeLines.Remove(toRemove.Key);
            line.gameObject.SetActive(false);
            _linePool.Enqueue(line);
        }
    }

    public void ToggleAddStarMode(bool isActive)
    {
        _addStarMode = isActive;
    }

    private void Update()
    {
        if (!_addStarMode) return;
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0f;
            CreateOrReuseStar(worldPos);
        }
    }
    
    private void CreateOrReuseStar(Vector3 pos)
    {
        if (Stars.Count >= 100)
        {
            App.Notification(NotifyType.StarsLimit);
            return;
        }
        
        Star star;
        if (_starPool.Count > 0)
        {
            star = _starPool.Dequeue();
            star.gameObject.SetActive(true);
            star.transform.position = pos;
        }
        else
        {
            star = Instantiate(_starObj, pos, Quaternion.identity, transform);
        }

        star.Initialize(this, _constellationPanel);
        Stars.Add(star);
    }

    public void ResetStars()
    {
        foreach (var star in Stars)
        {
            if (star != null)
            {
                star.gameObject.SetActive(false);
                _starPool.Enqueue(star);
            }
        }
        Stars.Clear();

        foreach (var kv in _activeLines)
        {
            var line = kv.Value;
            if (line != null)
            {
                line.gameObject.SetActive(false);
                _linePool.Enqueue(line);
            }
        }
        _activeLines.Clear();
    }
}
