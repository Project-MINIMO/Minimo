using System.Collections.Generic;
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
    [SerializeField] private MenuToggleGroup _toggleGroup;
    [SerializeField] private GameObject _selectedObj;
    
    [SerializeField] private InventorySlideHandler _slideHandler;
    
    private EditManager _editManager;
    
    private Tilemap _tilemap;
    private Tilemap _glowMap;
    private Tilemap _installMap;
    private CustomTile _selectedTile;
    private TileBase _tile;
    
    private bool _isErase;
    private bool _isPainting;
    
    private Vector3Int _lastPaintedCell = new(int.MinValue, int.MinValue, int.MinValue);
    
    private Dictionary<Vector3Int, (TileBase origTile, TileBase origGlow)> _backupTiles = new();

    private List<TileChange> _pendingTileChanges = new();

    [SerializeField] private Button _confirmBtn;
    [SerializeField] private Button _cancelBtn;
    
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        _editManager = App.GetManager<EditManager>();
        _tilemap = GameObject.FindWithTag("VillageTilemap").GetComponent<Tilemap>();
        _glowMap = GameObject.FindWithTag("GlowTilemap").GetComponent<Tilemap>();
        _installMap = GameObject.FindWithTag("InstallTilemap").GetComponent<Tilemap>();
        
        _openBtn.onClick.AddListener(OpenPanel);
        _closeBtn.onClick.AddListener(ClosePanel);
        _confirmBtn.onClick.AddListener(Confirm);
        _cancelBtn.onClick.AddListener(Cancel);
        
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
                _selectedObj.SetActive(true);
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
    
    public override async void OpenPanel()
    {
        base.OpenPanel();
        
        _backupTiles.Clear();
        
        _isErase = false;
        _tile = null;
        _selectedTile = null;
        _selectedTileImg.sprite = null;
        _selectedTileImg.gameObject.SetActive(false);
        _toggleGroup.Show(true);
        _selectedObj.SetActive(false);
        
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

        if (_tilemap.GetTile(cellPos)?.name == "Water05")
        {
            App.Notification(NotifyType.CannotReplaceTile);
            return;
        }
        if (_isErase)
        {
            if (_installMap.GetTile(cellPos) != null)
            {
                App.Notification(NotifyType.CannotEraseTile);
                return;
            }
            
            if (!_backupTiles.ContainsKey(cellPos))
            {
                var origTile = _tilemap.GetTile(cellPos);
                var origGlow = _glowMap.GetTile(cellPos);
                _backupTiles[cellPos] = (origTile, origGlow);
            }
            
            if (!_backupTiles.ContainsKey(cellPos))
            {
                var origTile = _tilemap.GetTile(cellPos);
                var origGlow = _glowMap.GetTile(cellPos);
                _backupTiles[cellPos] = (origTile, origGlow);
            }
            
            // 변경사항 기록 (삭제)
            int prevTileId = _selectedTile != null ? _selectedTile.ID : -1;
            _pendingTileChanges.Add(new TileChange
            {
                ChangeType = TileChangeType.Remove,
                TileId = prevTileId,
                Position = cellPos
            });

            _tilemap.SetTile(cellPos, null);
            _glowMap.SetTile(cellPos, null);
            return;
        }
        
        if (_installMap.GetTile(cellPos) != null && _selectedTile.Type == TileType.Water)
        {
            App.Notification(NotifyType.CannotInstallWaterTile);
            return;
        }
        
        if (_tilemap.GetTile(cellPos) == _tile) return;
        if (!UseGold()) return;
        
        if (!_backupTiles.ContainsKey(cellPos))
        {
            var origTile = _tilemap.GetTile(cellPos);
            var origGlow = _glowMap.GetTile(cellPos);
            _backupTiles[cellPos] = (origTile, origGlow);
        }
        
        // 변경사항 기록 (설치)
        _pendingTileChanges.Add(new TileChange
        {
            ChangeType = TileChangeType.Install,
            TileId = _selectedTile != null ? _selectedTile.ID : -1,
            Position = cellPos
        });

        _tilemap.SetTile(cellPos, _tile);
        _glowMap.SetTile(cellPos, _tile);
    }
    
    private bool UseGold()
    {
        if (_selectedTile == null) return true;
        
        if (_selectedTile.CanInstall())
        {
            AccountInfo.Instance.Gold.AddCount(-_selectedTile.Cost);
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
        _selectedObj.SetActive(true);
    }
    
    private async void Confirm()
    {
        _backupTiles.Clear();
        // 일괄 저장
        if (_pendingTileChanges.Count > 0)
        {
            var firebaseManager = App.GetManager<FirebaseManager>();
            await firebaseManager.BatchUpdateTiles(_pendingTileChanges);
            _pendingTileChanges.Clear();
        }
        _slideHandler.Open();
        _selectedObj.SetActive(false);
    }

    private void Cancel()
    {
        foreach (var kv in _backupTiles)
        {
            var cell = kv.Key;
            var (origTile, origGlow) = kv.Value;
            _tilemap.SetTile(cell, origTile);
            _glowMap.SetTile(cell, origGlow);

            if (!_isErase)
            {
                AccountInfo.Instance.Gold.AddCount(_selectedTile.Cost);
            }
        }
        _backupTiles.Clear();
        _slideHandler.Open();
        _selectedObj.SetActive(false);
    }
}

public enum TileChangeType { Install, Remove }
public class TileChange
{
    public TileChangeType ChangeType;
    public int TileId;
    public Vector3Int Position;
}
