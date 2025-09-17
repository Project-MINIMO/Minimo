using System;
using System.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlantHandler : MonoBehaviour
    , IPointerDownHandler, IPointerUpHandler
    , IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private enum PlantType { Object, UI }

    public event Action<bool> OnDragChanged;
    
    [SerializeField] private PlantType _plantType;
    
    [SerializeField] private ItemInfoUpdater _infoUpdater;
    [SerializeField] private GameObject _amountObj;
    
    [SerializeField] private GameObject _infoObj;
    [SerializeField] private TextMeshProUGUI _nameTMP;
    [SerializeField] private TextMeshProUGUI _timeTMP;
    [SerializeField] private TextMeshProUGUI _amountTMP;

    [SerializeField] private GameObject _lockObj;
    [SerializeField] private TextMeshProUGUI _lockTMP;
    
    private LayerMask _targetLayerMask;
    
    private RectTransform _rect;
    private Image _image;
    private Canvas _canvas;
    private Vector3 _startPosition;

    private ProduceData _currentOption;
    private ProduceManager _produceManager;
    
    private HashSet<ProduceObject> _plantedThisDrag;

    private bool _isLocked;
    private string _lockString;
    
    private void Awake()
    {
        _lockString = App.GetData<TitleData>().GetString("STR_BUILDING_UI_LOCK");
        
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
        
        _infoObj.SetActive(false);
        
        _amountObj.SetActive(true);
        _plantedThisDrag.Clear();
    }

    public void SetOption(ProduceData option)
    {
        _currentOption = option;

        _isLocked = AccountInfo.Instance.Level.Count < option.UnlockLevel;
        _lockObj.SetActive(_isLocked);
        _lockTMP.text = string.Format(_lockString, _currentOption.UnlockLevel);

        var result = option.ResultItems[0];
        var item = AccountInfo.Instance.Items[result.ID];
        
        _infoUpdater.UpdateItem(item.Icon, $"X {result.Amount}");
        
        _nameTMP.text = $"{item.Name} X {result.Amount}";
        _timeTMP.text = FormatTime(option.Time);
        _amountTMP.text = $"보유량 : {item.Count}";
    }
    
    private string FormatTime(float time)
    {
        var timeSpan = TimeSpan.FromSeconds(time);
        var minutes = (int)timeSpan.TotalMinutes;
        var seconds = timeSpan.Seconds;

        return minutes > 0 
            ? $"{minutes}분 {seconds:D2}초" 
            : $"{seconds}초";
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        _infoObj.SetActive(true);
        _infoObj.transform.SetParent(transform.parent.parent);
        _infoObj.transform.SetAsLastSibling();
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        _infoObj.SetActive(false);
        _infoObj.transform.SetParent(transform);
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_isLocked) return;
        
        _image.raycastTarget = false;
        
        _amountObj.SetActive(false);
        _infoObj.SetActive(false);
        _infoObj.transform.SetParent(transform);
        _plantedThisDrag.Clear();
        
        OnDragChanged?.Invoke(true);
    }

    public void OnDrag(PointerEventData eventData)
    {        
        if (_isLocked) return;

        _rect.anchoredPosition += eventData.delta / _canvas.scaleFactor;

        if (_plantType == PlantType.Object)
        {
            var worldPosition = Camera.main.ScreenToWorldPoint(eventData.position);
            worldPosition.z = 0;
            
            var hit = Physics2D.OverlapPoint(worldPosition, _targetLayerMask);
            if (hit == null) return;
            if (!hit.TryGetComponent<ProduceObject>(out var component)) return;
            if (_plantedThisDrag.Contains(component)) return;
            if (component.CurrentState is not ProduceState.Idle) return;
            
            _produceManager.RequestPlant(component, _currentOption,
                onSuccess: task => _plantedThisDrag.Add(component),
                onFailed: result =>
                {
                    if (result == NotifyType.MissMinimo) App.Notification(result);
                });
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_isLocked) return;
        
        _image.raycastTarget = true;
        _rect.anchoredPosition = _startPosition;
        
        if (_plantType == PlantType.UI)
        {
            List<RaycastResult> raycastResults = new();
            var raycaster = _canvas.GetComponent<GraphicRaycaster>();
            raycaster.Raycast(eventData, raycastResults);
            if (raycastResults.Any(result => result.gameObject.CompareTag("ProduceTaskBtn")))
            {
                _produceManager.RequestPlant(_currentOption,
                    onSuccess: null,
                    onFailed: App.Notification);
            }
        }
        
        _amountObj.SetActive(true);
        _plantedThisDrag.Clear();
        
        OnDragChanged?.Invoke(false);
    }
}
