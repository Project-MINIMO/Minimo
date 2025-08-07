using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using TMPro;

public class TilePanel : UIBase
{
    public override bool IsUseInput => _currentBrush != null;
    
    [SerializeField] private TextMeshProUGUI _titleTMP;

    [SerializeField] private Button _openBtn;
    [SerializeField] private Button _closeBtn; 
    [SerializeField] private Button _confirmBtn;
    [SerializeField] private Button _cancelBtn;  
    
    [SerializeField] private Image _selectedTileImg;
    [SerializeField] private Sprite _eraseSprite;
    [SerializeField] private Toggle _eraseTog;
    [SerializeField] private MenuToggleGroup _toggleGroup;
    [SerializeField] private GameObject _selectedObj;
    
    [SerializeField] private InventorySlideHandler _slideHandler;
    
    private EditManager _editManager;
    private InputManager _inputManager;
    
    private Tilemap _tilemap;
    private Tilemap _glowMap;
    private Tilemap _installMap;
    
    private Dictionary<Vector3Int, (TileBase origTile, TileBase origGlow)> _backupTiles = new();
    private List<TileChange> _pendingTileChanges = new();
    
    private Vector3Int _lastPaintedCell = new(int.MinValue, int.MinValue, int.MinValue);
    
    private PaintBrush _paintBrush;
    private EraseBrush _eraseBrush;
    private ITileBrush _currentBrush;

    private CameraBoundsUpdater _mapBounds;

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);

        _editManager = App.GetManager<EditManager>();
        _inputManager = App.GetManager<InputManager>();
        
        _tilemap = GameObject.FindWithTag("VillageTilemap").GetComponent<Tilemap>();
        _glowMap = GameObject.FindWithTag("GlowTilemap").GetComponent<Tilemap>();
        _installMap = GameObject.FindWithTag("InstallTilemap").GetComponent<Tilemap>();
        _mapBounds = GameObject.FindWithTag("MapBounds").GetComponent<CameraBoundsUpdater>();
        
        _paintBrush = new PaintBrush(_tilemap, _glowMap, _installMap);
        _eraseBrush = new EraseBrush(_tilemap, _glowMap, _installMap);
        
        _openBtn.onClick.AddListener(OpenPanel);
        _closeBtn.onClick.AddListener(ClosePanel);
        _confirmBtn.onClick.AddListener(Confirm);
        _cancelBtn.onClick.AddListener(Cancel);
        
        var slots = GetComponentsInChildren<TileSlot>(true);
        foreach (var slot in slots)
        {
            slot.OnItemSelected += OnItemSelected;
        }
        
        var toggles = GetComponentsInChildren<Toggle>(true);
        foreach (var toggle in toggles)
        {
            toggle.onValueChanged.AddListener((isOn) =>
            {
                if (!isOn) _currentBrush = null;
            });
        }
        
        _eraseTog.onValueChanged.AddListener((isOn) =>
        {
            if (!isOn) return;
            _selectedTileImg.sprite = _eraseSprite;
            SetBrush(_eraseBrush);
        });

        _titleTMP.text = App.GetData<TitleData>().GetString("STR_TILEEDIT_NAME");
    }
    
    private void Update()
    {
        if (!gameObject.activeInHierarchy) return;
        if (_inputManager.InputTarget != InputTargetType.Camera) return;

        if (_inputManager.CurrentState is InputState.ClickDown or InputState.Drag)
        {
            var screenPos = GetCurrentScreenPosition();
            var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
            worldPos.z = 0;
            var cellPos = _tilemap.WorldToCell(worldPos);
            
            TryPaint(cellPos);
        }
    }
    
    private Vector3 GetCurrentScreenPosition()
    {
#if UNITY_EDITOR
        return Input.mousePosition;
#else
        return Input.touchCount > 0 ? Input.GetTouch(0).position : Vector3.zero;
#endif
    }
    
    public override async void OpenPanel()
    {
        base.OpenPanel();
        
        _backupTiles.Clear();
        
        _toggleGroup.Show(true);
        _selectedObj.SetActive(false);
        
        _lastPaintedCell = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);
        
        _editManager.SetTileEditing(true);
    }

    public override void ClosePanel()
    {
        base.ClosePanel();

        Cancel();
        _editManager.SetTileEditing(false);
    }
    
    private void TryPaint(Vector3Int cellPos)
    {
        if (cellPos == _lastPaintedCell) return;
        _lastPaintedCell = cellPos;

        if (_tilemap.GetTile(cellPos)?.name == "Water05")
        {
            App.Notification(NotifyType.CannotReplaceTile);
            return;
        }
        
        if (_currentBrush == null) return;
        
        if (!_backupTiles.ContainsKey(cellPos))
        {
            var origTile = _tilemap.GetTile(cellPos);
            var origGlow = _glowMap.GetTile(cellPos);
            _backupTiles[cellPos] = (origTile, origGlow);
        }
        
        var result = _currentBrush.Apply(cellPos);
        if (result.Success)
        {
            _pendingTileChanges.Add(result.Change);
        }
    }

    private void OnItemSelected(InventorySlot<CustomTile> slot)
    {
        _paintBrush.SetSelectedTile(slot.Item);
        _selectedTileImg.sprite = slot.Item.Icon;
        SetBrush(_paintBrush);
    }

    private void SetBrush(ITileBrush brush)
    {
        _currentBrush = brush;
        _slideHandler.Close();
        _selectedObj.SetActive(true);
    }
    
    private async void Confirm()
    {
        _backupTiles.Clear();

        if (_pendingTileChanges.Count > 0)
        {
            var firebaseManager = App.GetManager<FirebaseManager>();
            await firebaseManager.BatchUpdateTiles(_pendingTileChanges);
            _pendingTileChanges.Clear();
        }

        _mapBounds.CalculateBounds();
        
        _currentBrush = null;
        _slideHandler.Open();
        _selectedObj.SetActive(false);
    }

    private void Cancel()
    {
        var returnCost = 0;
        
        foreach (var tile in _backupTiles)
        {
            var cell = tile.Key;
            var currentTile = _tilemap.GetTile(cell);
            var (origTile, origGlow) = tile.Value;
            _tilemap.SetTile(cell, origTile);
            _glowMap.SetTile(cell, origGlow);

            if (origTile != currentTile)
            {
                returnCost += _currentBrush.SelectedTile.Cost;
            }
        }

        if (returnCost != 0)
        {
            AccountInfo.Instance.Gold.AddCount(returnCost);
        }

        _currentBrush = null;
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
