using UnityEngine;
using UnityEngine.UI;

public class StorageInfoCtrl : MonoBehaviour
{
    [SerializeField] private Button _closeBtn;
    
    [SerializeField] private ItemInfoUpdater _infoUpdater;
    [SerializeField] private ItemSellHandler _sellHandler;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private RectTransform _rect;

    private RectTransform _parentRect;
    
    private void Awake()
    {
        _parentRect = _rect.parent.GetComponent<RectTransform>();
        
        _closeBtn.onClick.AddListener(() => gameObject.SetActive(false));
    }

    public void Show(InventorySlot slot)
    {
        gameObject.SetActive(true);
        
        _infoUpdater.UpdateItem(slot.Item);
        _sellHandler.Initialize(slot.Item);
        
        PositionNear(slot.GetComponent<RectTransform>());
    }

    private void PositionNear(RectTransform slotRect)
    {
        var worldCenter = slotRect.TransformPoint(slotRect.rect.center);
        var corners = new Vector3[4];
        slotRect.GetWorldCorners(corners);
        var screenPoint = RectTransformUtility.WorldToScreenPoint(null, worldCenter);
        var placeOnRight = worldCenter.x < (Screen.width * 0.5f);
    
        var worldX = placeOnRight
            ? corners[2].x   // top‑right.x
            : corners[0].x; // bottom‑left,  top‑left
        var worldPos = new Vector3(worldX, worldCenter.y, worldCenter.z);

        var screenPos = RectTransformUtility.WorldToScreenPoint(null, worldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _parentRect, screenPos, null, out Vector2 localPoint);
        var pivot = new Vector2(placeOnRight ? 0f : 1f, 0.5f);
        _rect.pivot = pivot;
        _rect.anchoredPosition = localPoint;
        
        Debug.Log($"screenPos: {screenPoint}, localPoint: {localPoint}, pivot: {pivot}");
    }
}
