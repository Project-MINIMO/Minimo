using System.Collections.Generic;
using UnityEngine;

public class Star : InteractObject
{
    [Header("Line Settings")]
    [SerializeField] private LineRenderer _linePrefab;
    [SerializeField] private float _connectDistance = 2f;

    private static List<Star> _allStars = new();
    private readonly Queue<LineRenderer> _pool = new();
    private readonly List<LineRenderer> _activeLines = new();
    
    private ConstellationPanel _constellationPanel;

    private void Awake()
    {
        _constellationPanel = App.GetManager<UIManager>().GetPanel<ConstellationPanel>();
        _allStars.Add(this);
    }

    public override void OnDrag()
    {
        _constellationPanel.OpenPanel();
        Debug.Log("OnDrag");
        foreach (var line in _activeLines)
        {
            line.gameObject.SetActive(false);
            _pool.Enqueue(line);
        }
        
        _activeLines.Clear();
        
        var mouseScreenPos = Input.mousePosition;
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = transform.position.z;
        transform.position = mouseWorldPos;
        
        var maxDist2 = _connectDistance * _connectDistance;

        foreach (var other in _allStars)
        {
            if (other == this) continue;

            if ((other.transform.position - mouseWorldPos).sqrMagnitude <= maxDist2)
            {
                var line = _pool.Count > 0 ? _pool.Dequeue() : Instantiate(_linePrefab);
                
                line.gameObject.SetActive(true);
                line.SetPosition(0, mouseWorldPos);
                line.SetPosition(1, other.transform.position);

                _activeLines.Add(line);
            }
        }
    }
    
    public override void OnDragEnd()
    {
        _constellationPanel.ClosePanel();
    }
    
    public override void OnLongPress() { }
    
    public override void OnClickUp() { }
}
