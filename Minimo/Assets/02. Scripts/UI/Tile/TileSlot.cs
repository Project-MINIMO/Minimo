using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TileSlot : InventorySlot<CustomTile>
{
    public override bool CanShow() => Item.IsLocked;
    
    [SerializeField] private Image _tileImg;
    [SerializeField] private TextMeshProUGUI _costTMP;
    [SerializeField] private GameObject _lockBack;
    [SerializeField] private TextMeshProUGUI _lockTMP;
    
    private void OnEnable()
    {
        SetState();
        if (Item != null && Item.ID is 14 or 16 or 17)
        {
            gameObject.SetActive(false);
        }
    }
        
    public override void Initialize(CustomTile item)
    {
        base.Initialize(item);

        Item = item;
        _tileImg.sprite = item.Icon;
        _costTMP.SetText(item.Cost.ToString());

        _lockTMP.SetText(App.GetData<TitleData>().GetFormatString("STR_BUILDING_UI_LOCK", item.UnlockLevel.ToString()));
    }

    private void SetState()
    {
        if (Item == null) return;
        
        _lockBack.SetActive(CanShow());
    }
    
}
