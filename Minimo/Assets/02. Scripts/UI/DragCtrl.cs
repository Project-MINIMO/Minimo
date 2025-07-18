using UnityEngine;
using UnityEngine.EventSystems;

public class DragCtrl : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private float edgeThreshold = 0.1f;
    [SerializeField] private float panSpeed = 5f;
    
    private Camera _camera;
    private bool _isDragging;
    private Vector2 _pointerPos;

    private void Awake()
    {
        _camera = Camera.main;
    }

    public void OnBeginDrag(PointerEventData eventData) => _isDragging = true;
    public void OnEndDrag(PointerEventData eventData)   => _isDragging = false;

    private void Update()
    {
        if (!_isDragging) return;
        
        if (!Input.GetMouseButton(0))
        {
            _isDragging = false;
            return;
        }
        
#if UNITY_EDITOR
        _pointerPos = Input.mousePosition;
#else
        if (Input.touchCount > 0)_pointerPos = Input.GetTouch(0).position;
        else return;
#endif
        
        var w = Screen.width;
        var h = Screen.height;
        
        var thrX = w * edgeThreshold;
        var thrY = h * edgeThreshold;
        
        var nearEdge =
            _pointerPos.x <= thrX || _pointerPos.x >= w - thrX ||
            _pointerPos.y <= thrY || _pointerPos.y >= h - thrY;

        if (!nearEdge) return;

        var screenCenter = new Vector2(w * 0.5f, h * 0.5f);
        var dir2D = (_pointerPos - screenCenter).normalized;

        var worldDir =
            _camera.transform.right * dir2D.x +
            _camera.transform.up    * dir2D.y;

        _camera.transform.Translate(worldDir * panSpeed * Time.deltaTime, Space.World);
    }
}