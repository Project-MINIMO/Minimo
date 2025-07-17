using UnityEngine;
using UnityEngine.UI;

public class StorageInfoCtrl : MonoBehaviour
{
    [SerializeField] private Button _closeBtn;
    [SerializeField] private Button _sellBtn;
    
    [SerializeField] private ItemInfoUpdater _infoUpdater;
    [SerializeField] private ItemSellHandler _sellHandler;
    [SerializeField] private Canvas _canvas;
    
    private RectTransform _rect;
    
    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        
        _closeBtn.onClick.AddListener(() => gameObject.SetActive(false));
        _sellBtn.onClick.AddListener(() => gameObject.SetActive(false));
    }

    public void Show(InventorySlot slot)
    {
        _infoUpdater.UpdateItem(slot.Item);
        _sellHandler.SetItem(slot.Item);
        
        PositionNear(slot.GetComponent<RectTransform>());
    }

    private void PositionNear(RectTransform slotRect)
    {
        var worldPos = slotRect.TransformPoint(slotRect.rect.center);
        var screenPoint = RectTransformUtility.WorldToScreenPoint(null, worldPos);
        
        var canvasRect = _canvas.transform as RectTransform;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, 
                _canvas.worldCamera, 
                out var localPoint))
            return;
        
        var pivot = new Vector2(
            screenPoint.x / Screen.width  < 0.5f ? 0f : 1f,
            screenPoint.y / Screen.height < 0.5f ? 0f : 1f
        );
        _rect.pivot = pivot;
        
        _rect.anchoredPosition = localPoint;
    }
}
