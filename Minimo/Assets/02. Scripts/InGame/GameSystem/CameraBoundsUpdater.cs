using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(BoxCollider2D))]
public class CameraBoundsUpdater : MonoBehaviour
{
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private Vector3 _margin = Vector2.one * 0f;

    private BoxCollider2D _collider;
    private EditManager _editManager;
    
    private Bounds _localBounds;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        _editManager = App.GetManager<EditManager>();
        
        CalculateBounds();
    }

    private void LateUpdate()
    {
        if (!_editManager.IsTileEditing.Value) return;
        
        CalculateBounds();
    }

    private void CalculateBounds()
    {
        _tilemap.CompressBounds();
        
        _localBounds = _tilemap.localBounds;

        var sizeWithMargin = _localBounds.size + _margin * 2f;
        
        _collider.offset = _localBounds.center;
        _collider.size = sizeWithMargin;
    }

    public Vector3 GetRandomPoint(Vector3 origin, float radius)
    {
        var mapMin = _tilemap.transform.TransformPoint(_localBounds.min);
        var mapMax = _tilemap.transform.TransformPoint(_localBounds.max);
        var mapLeft = mapMin.x;
        var mapRight = mapMax.x;
        var mapBottom = mapMin.y;
        var mapTop = mapMax.y;

        var colliderBounds = _collider.bounds;
        var colliderLeft = colliderBounds.min.x + 2;
        var colliderRight = colliderBounds.max.x - 2;
        var colliderBottom = colliderBounds.min.y + 2;
        var colliderTop = colliderBounds.max.y - 2;

        float xZoneMin, xZoneMax;
        if (origin.x < mapLeft)
        {
            xZoneMin = colliderLeft;
            xZoneMax = mapLeft;
        }
        else if (origin.x > mapRight)
        {
            xZoneMin = mapRight;
            xZoneMax = colliderRight;
        }
        else
        {
            xZoneMin = colliderLeft;
            xZoneMax = colliderRight;
        }
        
        float yZoneMin, yZoneMax;
        if (origin.y < mapBottom)
        {
            yZoneMin = colliderBottom;
            yZoneMax = mapBottom;
        }
        else if (origin.y > mapTop)
        {
            yZoneMin = mapTop;
            yZoneMax = colliderTop; 
        }
        else
        {
            yZoneMin = colliderBottom;
            yZoneMax = colliderTop;
        }
        
        var xMin = Mathf.Max(xZoneMin, origin.x - radius);
        var xMax = Mathf.Min(xZoneMax, origin.x + radius);
        var yMin = Mathf.Max(yZoneMin, origin.y - radius);
        var yMax = Mathf.Min(yZoneMax, origin.y + radius);

        if (xMin > xMax || yMin > yMax)
        {
            return origin;
        }
        
        var newX = Random.Range(xMin, xMax);
        var newY = Random.Range(yMin, yMax);
        return new Vector3(newX, newY, origin.z);
    }
    
    public Vector3 GetRandomOutsideMapPoint()
    {
        var colliderBounds = _collider.bounds;
        var mapMin = _tilemap.transform.TransformPoint(_localBounds.min) + Vector3.one * 2;
        var mapMax = _tilemap.transform.TransformPoint(_localBounds.max) - Vector3.one * 2;

        var regions = new System.Collections.Generic.List<(float xMin, float xMax, float yMin, float yMax)>();

        if (colliderBounds.min.x < mapMin.x)
            regions.Add((colliderBounds.min.x, mapMin.x, colliderBounds.min.y, colliderBounds.max.y));

        if (colliderBounds.max.x > mapMax.x)
            regions.Add((mapMax.x, colliderBounds.max.x, colliderBounds.min.y, colliderBounds.max.y));

        if (colliderBounds.min.y < mapMin.y)
            regions.Add((mapMin.x, mapMax.x, colliderBounds.min.y, mapMin.y));

        if (colliderBounds.max.y > mapMax.y)
            regions.Add((mapMin.x, mapMax.x, mapMax.y, colliderBounds.max.y));

        var selected = regions[Random.Range(0, regions.Count)];
        var x = Random.Range(selected.xMin, selected.xMax);
        var y = Random.Range(selected.yMin, selected.yMax);
        return new Vector3(x, y, 0);
    }

    public (float, float) GetOutsideMapWidth()
    {
        var colliderBounds = _collider.bounds;
        var mapMin = _tilemap.transform.TransformPoint(colliderBounds.min);
        var mapMax = _tilemap.transform.TransformPoint(colliderBounds.max);

        return (mapMin.x, mapMax.x);
    }
  
    public Vector3[] GetCorners()
    {
        var bounds = _collider.bounds;
        var min = bounds.min;
        var max = bounds.max;

        return new Vector3[]
        {
            new (min.x, min.y),
            new (max.x, min.y),
            new (max.x, max.y),
            new (min.x, max.y)
        };
    }
}