using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using TMPro;

public class TilePanel : UIBase
{
    [SerializeField] private TextMeshProUGUI _titleTMP;

    [SerializeField] private Button _openBtn;
    [SerializeField] private Button _closeBtn;
    [SerializeField] private Image _selectedTileImg;
    [SerializeField] private Sprite _eraseSprite;
    [SerializeField] private Toggle _eraseTog;
    
    private EditManager _editManager;
    
    private bool _isPainting;
    private Tilemap _tilemap;
    private CustomTile _selectedTile;
    private Tile _tile;
    
    private Vector3Int _lastPaintedCell = new(int.MinValue, int.MinValue, int.MinValue);
    
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        _editManager = App.GetManager<EditManager>();
        _tilemap = GameObject.FindWithTag("VillageTilemap").GetComponent<Tilemap>();
        
        _openBtn.onClick.AddListener(OpenPanel);
        _closeBtn.onClick.AddListener(ClosePanel);
        
        var slots = GetComponentsInChildren<TileSlot>(true);
        foreach (var slot in slots)
        {
            slot.OnItemSelected += OnItemSelected;
        }
        
        _eraseTog.onValueChanged.AddListener((isOn) =>
        {
            if (isOn)
            {
                _tile = null;
                _selectedTile = null;
                _selectedTileImg.sprite = _eraseSprite;
                _selectedTileImg.gameObject.SetActive(true);
            }
        });

        _titleTMP.text = App.GetData<TitleData>().GetString("STR_TILEEDIT_NAME");
    }
    
    public override void OpenPanel()
    {
        base.OpenPanel();
        
        _tile = null;
        _selectedTile = null;
        _selectedTileImg.sprite = null;
        _selectedTileImg.gameObject.SetActive(false);
        
        _isPainting = false;
        _lastPaintedCell = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);
        
        _editManager.SetTileEditing(true);
    }

    public override void ClosePanel()
    {
        base.ClosePanel();
        
        _editManager.SetTileEditing(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            _isPainting = true;
            TryPaint();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _isPainting = false;
            _lastPaintedCell = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);
        }
        else if (_isPainting && Input.GetMouseButton(0))
        {
            TryPaint();
        }
    }
    
    private void TryPaint()
    {
        var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0;
        var cellPos = _tilemap.WorldToCell(worldPos);
        
        if (cellPos == _lastPaintedCell) return;
        _lastPaintedCell = cellPos;
        
        if (_tilemap.GetTile(cellPos) == _tile) return;
        if (!UseGold()) return;
        
        _tilemap.SetTile(cellPos, _tile);
    }
    
    private bool UseGold()
    {
        if (_selectedTile == null) return true;
        
        if (_selectedTile.CanInstall())
        {
            AccountInfo.Instance.Gold.AddCount(-10);
            return true;
        }
        else
        {
            App.Notification(NotifyType.GoldLack);
            return false;
        }
    }
    
    private void OnItemSelected(InventorySlot<CustomTile> slot)
    {
        _tile = slot.Item.Tile;
        _selectedTile = slot.Item;
        _selectedTileImg.sprite = slot.Item.Icon;
        _selectedTileImg.gameObject.SetActive(true);
    }
}
