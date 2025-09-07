using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingPanel : UIBase
{
    public override bool IsUseBlur => true;
    protected override bool IsUseGuide => true;
    
    [SerializeField] private TextMeshProUGUI _titleTMP;

    [SerializeField] private Button _openBtn;
    [SerializeField] private Button _closeBtn;
    [SerializeField] private Button _infoBtn;
    
    [SerializeField] private MenuToggleGroup _toggleGroup;

    private EditManager _editManager;
    
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);
        
        _editManager = App.GetManager<EditManager>();
        
        _openBtn.onClick.AddListener(OpenPanel);
        _closeBtn.onClick.AddListener(ClosePanel);
        _infoBtn.onClick.AddListener(ShowGuide);

        _titleTMP.text = App.GetData<TitleData>().GetString("STR_BUILDING_UI_NAME");
    }

    private void Start()
    {
        var slots = GetComponentsInChildren<BuildingSlot>(true);
        foreach (var slot in slots)
        {
            slot.OnItemSelected += OnItemSelected;
        }
    }
    
    public override void Show(bool isNew)
    {
        base.Show(isNew);
        
        _toggleGroup.Show(isNew);
    }
    
    private void OnItemSelected(InventorySlot<Building> slot)
    {
        if (!slot.Item.CanInstall)
        {
            App.Notification(NotifyType.GoldLack);
            return;
        }
        
        var cameraCenterPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, Camera.main.nearClipPlane));
        cameraCenterPosition.z = 0;

        _editManager.CreateAndEditAsync(slot.Item, cameraCenterPosition).Forget();
    }
}
