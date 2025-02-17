using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlantHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private LayerMask _targetLayerMask;
    
    private RectTransform _rect;
    private Image _image;
    private Canvas _canvas;
    
    private Vector3 _startPosition;

    private ProduceOption _currentOption;
    private ProduceManager _produceManager;
    
    private void Start()
    {
        _targetLayerMask = LayerMask.GetMask("InteractObject");
        _produceManager = App.GetManager<ProduceManager>();
        
        _rect = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
        _canvas = GetComponentInParent<Canvas>();
        
        _startPosition = _rect.anchoredPosition;
    }

    public void SetOption(ProduceOption option)
    {
        _currentOption = option;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        _image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rect.anchoredPosition += eventData.delta / _canvas.scaleFactor;

        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        Collider2D hit = Physics2D.OverlapPoint(worldPosition, _targetLayerMask);
        if (hit != null && hit.TryGetComponent<ProduceObject>(out var component))
        {
            if (component.IsPrimary)
            {
                component.StartPlant(_currentOption);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        List<RaycastResult> raycastResults = new();
        GraphicRaycaster raycaster = _canvas.GetComponent<GraphicRaycaster>();
        raycaster.Raycast(eventData, raycastResults);
        foreach (var result in raycastResults)
        {
            if (result.gameObject.CompareTag("ProduceTaskBtn"))
            {
                var currentObject = _produceManager.CurrentProduceObject;
                currentObject.StartPlant(_currentOption);
                break; 
            }
        }
        
        _image.raycastTarget = true;
        _rect.anchoredPosition = _startPosition;
    }
}
