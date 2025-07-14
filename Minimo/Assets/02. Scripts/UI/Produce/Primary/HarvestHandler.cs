using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HarvestHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private RectTransform _rect;
    [SerializeField] private Image _image;
    
    private ProduceManager _produceManager;
    
    private Canvas _canvas;
    private LayerMask _targetLayerMask;
    
    private Vector3 _startPosition;

    private void Start()
    {
        _targetLayerMask = LayerMask.GetMask("InteractObject");
        _produceManager = App.GetManager<ProduceManager>();
        
        _canvas = GetComponentInParent<Canvas>();
        
        _startPosition = _rect.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rect.anchoredPosition += eventData.delta / _canvas.scaleFactor;

        var worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var hit = Physics2D.OverlapPoint(worldPosition, _targetLayerMask);
        if (hit != null && hit.TryGetComponent<ProducePrimary>(out var component))
        {
            _produceManager.Harvest(component);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _image.raycastTarget = true;
        _rect.anchoredPosition = _startPosition;
    }
}

