using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HarvestHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private GameObject _hint;
    [SerializeField] private RectTransform _rect;
    [SerializeField] private Image _image;
    
    private ProduceManager _produceManager;
    
    private Canvas _canvas;
    private LayerMask _targetLayerMask;
    private Vector3 _startPosition;
    
    private HashSet<ProducePrimary> _harvestedThisDrag;

    private void Start()
    {
        _targetLayerMask = LayerMask.GetMask("InteractObject");
        _produceManager = App.GetManager<ProduceManager>();
        
        _canvas = GetComponentInParent<Canvas>();
        
        _startPosition = _rect.anchoredPosition;
        
        _harvestedThisDrag = new HashSet<ProducePrimary>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _image.raycastTarget = false;
        _harvestedThisDrag.Clear();
        
        _hint.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rect.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        var worldPosition = Camera.main.ScreenToWorldPoint(eventData.position);
        worldPosition.z = 0;
        
        var hit = Physics2D.OverlapPoint(worldPosition, _targetLayerMask);
        if (hit == null) return;
        if (!hit.TryGetComponent<ProducePrimary>(out var component)) return;
        if (_harvestedThisDrag.Contains(component)) return;
        if (component.CurrentState is not ProduceState.Complete) return;
            
        _produceManager.Harvest(component);
        _harvestedThisDrag.Add(component);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _image.raycastTarget = true;
        _rect.anchoredPosition = _startPosition;
        _harvestedThisDrag.Clear();
        
        _hint.SetActive(true);
    }
}

