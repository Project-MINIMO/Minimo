using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlantHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private enum PlantType { Object, UI }
    
    [SerializeField] private PlantType _plantType;
    
    [SerializeField] private ItemInfoUpdater _infoUpdater;
    [SerializeField] private GameObject _amountObj;
    
    private LayerMask _targetLayerMask;
    
    private RectTransform _rect;
    private Image _image;
    private Canvas _canvas;
    private Vector3 _startPosition;

    private ProduceData _currentOption;
    private ProduceManager _produceManager;
    
    private HashSet<ProduceObject> _plantedThisDrag;
    
    private void Awake()
    {
        _targetLayerMask = LayerMask.GetMask("InteractObject");
        _produceManager = App.GetManager<ProduceManager>();
        
        _rect = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
        _canvas = GetComponentInParent<Canvas>();
        
        _startPosition = _rect.anchoredPosition;
        
        _plantedThisDrag = new HashSet<ProduceObject>();
    }

    private void OnEnable()
    {
        _image.raycastTarget = true;
        _rect.anchoredPosition = _startPosition;
        
        _amountObj.gameObject.SetActive(true);
        _plantedThisDrag.Clear();
    }

    public void SetOption(ProduceData option)
    {
        _currentOption = option;

        var result = option.ResultItems[0];
        _infoUpdater.UpdateItem(result.ID, result.Amount);
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        _image.raycastTarget = false;
        
        _amountObj.gameObject.SetActive(false);
        _plantedThisDrag.Clear();
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rect.anchoredPosition += eventData.delta / _canvas.scaleFactor;

        if (_plantType == PlantType.Object)
        {
            var worldPosition = Camera.main.ScreenToWorldPoint(eventData.position);
            worldPosition.z = 0;
            
            var hit = Physics2D.OverlapPoint(worldPosition, _targetLayerMask);
            if (hit != null
                && hit.TryGetComponent<ProduceObject>(out var component)
                && !_plantedThisDrag.Contains(component))
            {
                _produceManager.Plant(component, _currentOption);
                _plantedThisDrag.Add(component);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _image.raycastTarget = true;
        _rect.anchoredPosition = _startPosition;
        
        if (_plantType == PlantType.UI)
        {
            List<RaycastResult> raycastResults = new();
            var raycaster = _canvas.GetComponent<GraphicRaycaster>();
            raycaster.Raycast(eventData, raycastResults);
            if (raycastResults.Any(result => result.gameObject.CompareTag("ProduceTaskBtn")))
            {
                _produceManager.Plant(_currentOption);
            }
        }
        
        _amountObj.gameObject.SetActive(true);
        _plantedThisDrag.Clear();
    }
}
