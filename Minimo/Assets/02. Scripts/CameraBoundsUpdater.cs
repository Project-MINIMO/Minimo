using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(BoxCollider2D))]
public class CameraBoundsUpdater : MonoBehaviour
{
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private Vector3 _margin = Vector2.one * 0f;

    private BoxCollider2D _collider;
    private EditManager _editManager;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        _editManager = App.GetManager<EditManager>();
    }

    private void Start()
    {
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
        
        var localBounds = _tilemap.localBounds;

        var sizeWithMargin = localBounds.size + _margin * 2f;
        
        _collider.offset = localBounds.center;
        _collider.size = sizeWithMargin;
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