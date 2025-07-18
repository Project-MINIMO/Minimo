using UniRx;
using UnityEngine;

public class EditManager : ManagerBase
{
    public ReactiveProperty<bool> IsEditing { get; } = new(false);
    public ReactiveProperty<Vector3> CurrentCellPosition { get; } = new();
    public BuildingObject CurrentEditObject { get; private set; }
    
    [SerializeField] private GridLayout _gridLayout;
    [SerializeField] private Transform _buildingParent;
    [SerializeField] private GameObject _objectPrefab;
    
    private InstallChecker _installChecker;
    private TileStateModifier _tileStateModifier;

    private void Start()
    {
        _installChecker = GetComponent<InstallChecker>();
        _tileStateModifier = GetComponent<TileStateModifier>();

        InstallExistBuildings();
    }
    
    private void InstallExistBuildings()
    {
        /*
        foreach (var building in buildings)
        {
            if (building.BuildingType == "TestBuilding")
            {
                continue;
            }

            var path = building.BuildingType;
            path = path.Replace("Building_", "");
            var prefabPath = $"Building/{path}";
            var objectPrefab = Resources.Load<GameObject>(prefabPath);
            var position = building.Position != null
                ? new Vector3Int(building.Position[0], building.Position[1], building.Position[2])
                : Vector3Int.zero;
            var cellPosition = _gridLayout.CellToWorld(position);
            var buildingObject = Instantiate(objectPrefab, cellPosition, Quaternion.identity).GetComponent<BuildingObject>();
            buildingObject.transform.SetParent(_buildingParent);
            buildingObject.Initialize(building);
            _tileStateModifier.ModifyTileState(buildingObject.Area, TileState.Installed);
        }
        */
    }
    
    public void StartEdit(BuildingObject gridObject, bool isNew = false)
    {
        if (CurrentEditObject && CurrentEditObject != gridObject)
        {
            CancelEdit();
        }

        if (!isNew)
        {
            _tileStateModifier.ModifyTileState(gridObject, TileState.Empty);
        }
        
        CurrentEditObject = gridObject;
        IsEditing.Value = true;
        
        CurrentEditObject.StartEdit();
        CurrentCellPosition.Value = CurrentEditObject.transform.position;
    }

    public void CancelEdit()
    {
        if (CurrentEditObject.Cancel())
        {
            _tileStateModifier.ModifyTileState(CurrentEditObject, TileState.Installed);
        }
        
        CurrentEditObject = null;
        IsEditing.Value = false;
    }
    
    public void ConfirmEdit()
    {
        if (!_installChecker.CheckCanInstall(CurrentEditObject))
        {
            return;
        }

        var isNew = !CurrentEditObject.IsPlaced;
        
        if (CurrentEditObject.Install())
        {
            _tileStateModifier.ModifyTileState(CurrentEditObject, TileState.Installed);
            
            if (isNew)
            {
                var currentCell = _gridLayout.WorldToCell(CurrentEditObject.transform.position);
                var diagonalOffset = new Vector3Int(0, -1, 0);
                var newCell = currentCell + diagonalOffset;
                var newWorldPos = _gridLayout.CellToWorld(newCell);

                var buildingData = CurrentEditObject.BuildingData;
                CurrentEditObject = null;
                CreateObject(buildingData, newWorldPos);
                return;
            }
            
            CurrentEditObject = null;
            IsEditing.Value = false;
        }
    }
    
    public void CreateObject(Building data, Vector3 position)
    {
        var gridObject = Instantiate(_objectPrefab, position, Quaternion.identity, _buildingParent);
        switch (data.Type)
        {
            case BuildingType.Tier1:
                gridObject.AddComponent<ProducePrimary>();
                break;
            
            case BuildingType.Tier2:
                gridObject.AddComponent<ProduceSecondary>();
                break;
            
            case BuildingType.Tier3:
                gridObject.AddComponent<ProduceTertiary>();
                break;
            
            default:
                gridObject.AddComponent<ProduceQuaternary>();
                break;
        }
       

        if (gridObject.TryGetComponent<ProduceObject>(out var produce))
        {
            produce.Initialize(data);
        }
        else
        {
            Debug.LogError("GridObject component not found in instantiated prefab.");
            Destroy(gridObject.gameObject);
        }
    }

    public void MoveObject(Vector3 touchPosition)
    {
        if (!IsEditing.Value) return;
        
        var cellPosition = _gridLayout.WorldToCell(touchPosition);
        SetCurrentPosition(cellPosition);
    }
    
    public void MoveObject(BoundsInt area)
    {
        SetCurrentPosition(area.position);
    }
    
    private void SetCurrentPosition(Vector3Int position)
    {
        CurrentEditObject.transform.position = _gridLayout.CellToWorld(position);
        CurrentCellPosition.Value = CurrentEditObject.transform.position;
    }
    
    public Vector3Int GetCellPosition(Vector3 position)
    {
        return _gridLayout.WorldToCell(position);
    }
    
    public Vector3 GetWorldPosition(Vector3Int position)
    {
        return _gridLayout.CellToWorld(position);
    }
}
