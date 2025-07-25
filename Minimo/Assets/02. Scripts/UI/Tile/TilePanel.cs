using System.Linq;

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
    
    [SerializeField] private InventorySlideHandler _slideHandler;
    
    private EditManager _editManager;
    
    private Tilemap _tilemap;
    private Tilemap _glowMap;
    private Tilemap _installMap;
    private CustomTile _selectedTile;
    private Tile _tile;
    
    private bool _isErase;
    private bool _isPainting;
    
    private Vector3Int _lastPaintedCell = new(int.MinValue, int.MinValue, int.MinValue);
    
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        _editManager = App.GetManager<EditManager>();
        _tilemap = GameObject.FindWithTag("VillageTilemap").GetComponent<Tilemap>();
        _glowMap = GameObject.FindWithTag("GlowTilemap").GetComponent<Tilemap>();
        _installMap = GameObject.FindWithTag("InstallTilemap").GetComponent<Tilemap>();
        
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
                _isErase = true;
                _selectedTileImg.sprite = _eraseSprite;
                _selectedTileImg.gameObject.SetActive(true);
                _slideHandler.Close();
            }
            else
            {
                _isErase = false;
                _selectedTileImg.sprite = null;
                _selectedTileImg.gameObject.SetActive(false);
            }
        });

        _titleTMP.text = App.GetData<TitleData>().GetString("STR_TILEEDIT_NAME");
    }
    
    public override void OpenPanel()
    {
        base.OpenPanel();
        
        _isErase = false;
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
        if (!_isErase && _selectedTile == null) return;
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
        
        if (_isErase)
        {
            if (_installMap.GetTile(cellPos) != null)
            {
                App.Notification(NotifyType.CannotEraseTile);
                return;
            }
            
            _tilemap.SetTile(cellPos, null);
            _glowMap.SetTile(cellPos, null);
            return;
        }

        if (_tilemap.GetTile(cellPos) == _tile) return;
        if (!UseGold()) return;
        
        _tilemap.SetTile(cellPos, _tile);
        _glowMap.SetTile(cellPos, _tile);
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
        _slideHandler.Close();
    }
}
