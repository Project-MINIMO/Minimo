using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class PlantHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private enum PlantType
    {
        Object,
        UI
    }
    
    [SerializeField] private PlantType _plantType;
    
    [SerializeField] private Image _itemImg;
    [SerializeField] private TextMeshProUGUI _amountTMP;
    
    private LayerMask _targetLayerMask;
    
    private RectTransform _rect;
    private Image _image;
    private Canvas _canvas;
    
    private Vector3 _startPosition;

    private TitleData _titleData;
    private ProduceData _currentOption;
    private ProduceManager _produceManager;
    
    private void Awake()
    {
        _titleData = App.GetData<TitleData>();
        
        _targetLayerMask = LayerMask.GetMask("InteractObject");
        _produceManager = App.GetManager<ProduceManager>();
        
        _rect = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
        _canvas = GetComponentInParent<Canvas>();
        
        _startPosition = _rect.anchoredPosition;
    }

    public void SetOption(ProduceData option)
    {
        _currentOption = option;
        
        SetResultInfo(option.ResultItems[0]);
    }
    
    private void SetResultInfo(ProduceResult result)
    {
        if (!_titleData.Item.TryGetValue(result.ID, out var itemData))
        {
            Debug.LogError($"Cannot find item data with code : {result.ID}");
            return;
        }
        
        _itemImg.sprite = Resources.Load<Sprite>($"Item/{itemData.Name}");
        _amountTMP.text = result.Amount.ToString();
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        _image.raycastTarget = false;
        
        _amountTMP.gameObject.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rect.anchoredPosition += eventData.delta / _canvas.scaleFactor;

        if (_plantType == PlantType.Object)
        {
            var worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
            var hit = Physics2D.OverlapPoint(worldPosition, _targetLayerMask);
            if (hit != null && hit.TryGetComponent<ProduceObject>(out var component))
            {
                component.StartPlant(_currentOption);
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
                var currentObject = _produceManager.CurrentProduceObject;
                currentObject.StartPlant(_currentOption);
            }
        }
        
        _amountTMP.gameObject.SetActive(true);
    }
}
