using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingPanel : UIBase
{
    [SerializeField] private TextMeshProUGUI _titleTMP;

    [SerializeField] private Button _openBtn;
    [SerializeField] private Button _closeBtn;

    private EditManager _editManager;
    
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);
        
        _editManager = App.GetManager<EditManager>();
        
        _openBtn.onClick.AddListener(OpenPanel);
        _closeBtn.onClick.AddListener(ClosePanel);
        
        var slots = GetComponentsInChildren<BuildingSlot>(true);
        foreach (var slot in slots)
        {
            slot.OnItemSelected += OnItemSelected;
        }

        _titleTMP.text = App.GetData<TitleData>().GetString("STR_BUILDING_UI_TITLE");
    }
    
    private void OnItemSelected(InventorySlot<Building> slot)
    {
        var cameraCenterPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, Camera.main.nearClipPlane));
        cameraCenterPosition.z = 0;

        _editManager.CreateObject(slot.Item, cameraCenterPosition);
    }
}
