using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

public class TilePanel : UIBase
{
    [SerializeField] private TileBase[] _tileBases;
    
    [SerializeField] private MenuToggle[] _menuTogs;
    [SerializeField] private Button _openBtn;
    [SerializeField] private Button _closeBtn;

    private Tilemap _tilemap;
    private int _selectedIndex;
    private bool _isPainting;
    private Vector3Int _lastPaintedCell = new(int.MinValue, int.MinValue, int.MinValue);
    
    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);
        
        _tilemap = GameObject.FindWithTag("VillageTilemap").GetComponent<Tilemap>();
        
        for (var i = 0; i < _menuTogs.Length; i++)
        {
            var index = i;
            _menuTogs[index].onValueChanged.AddListener((isOn) =>
            {
                if (isOn) _selectedIndex = index;
            });
        }
        
        _openBtn.onClick.AddListener(OpenPanel);
        _closeBtn.onClick.AddListener(ClosePanel);
    }

    public override void OpenPanel()
    {
        base.OpenPanel();
        
        _menuTogs[0].isOn = true;
        _isPainting = false;
        _lastPaintedCell = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);
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
        
        if (_tilemap.GetTile(cellPos) == _tileBases[_selectedIndex]) return;
        if (!UseGold()) return;
        
        _tilemap.SetTile(cellPos, _tileBases[_selectedIndex]);
    }

    private bool UseGold()
    {
        if (AccountInfo.Instance.Gold.Count < 10)
        {
            App.Notification(NotifyType.GoldLack);
            return false;
        }
        else
        {
            AccountInfo.Instance.Gold.AddCount(-10);
            return true;
        }
    }
}
