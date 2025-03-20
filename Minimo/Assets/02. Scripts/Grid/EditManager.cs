using UniRx;
using Unity.VisualScripting;
using UnityEngine;

public class EditManager : ManagerBase
{
    public ReactiveProperty<bool> IsEditing { get; } = new(false);
    public ReactiveProperty<Vector3> CurrentCellPosition { get; } = new();
    public BuildingObject CurrentEditObject { get; private set; }
    
    [SerializeField] private GridLayout _gridLayout;
    [SerializeField] private Transform _buildingParent;
    
    private InstallChecker _installChecker;
    private TileStateModifier _tileStateModifier;

    private void Start()
    {
        _installChecker = GetComponent<InstallChecker>();
        _tileStateModifier = GetComponent<TileStateModifier>();

        InstallExistBuildings();
    }
    
    private async void InstallExistBuildings()
    {
        var buildingManager = App.GetManager<BuildingManager>();
        var buildings = await buildingManager.GetBuildingsAsync();
        
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
    }
    
    public void StartEdit(BuildingObject gridObject)
    {
        if (CurrentEditObject && CurrentEditObject != gridObject)
        {
            CancelEdit();
        }
        
        _tileStateModifier.ModifyTileState(gridObject.Area, TileState.Empty);
        
        CurrentEditObject = gridObject;
        IsEditing.Value = true;
        
        CurrentEditObject.StartEdit();
        CurrentCellPosition.Value = CurrentEditObject.transform.position;
    }

    public void CancelEdit()
    {
        CurrentEditObject.Cancel();
        
        _tileStateModifier.ModifyTileState(CurrentEditObject.Area, TileState.Installed);
        
        CurrentEditObject = null;
        IsEditing.Value = false;
    }
    
    public async void ConfirmEdit()
    {
        if (!_installChecker.CheckCanInstall(CurrentEditObject))
        {
            return;
        }

        if (await CurrentEditObject.Install())
        {
            _tileStateModifier.ModifyTileState(CurrentEditObject.Area, TileState.Installed);
            
            CurrentEditObject = null;
            IsEditing.Value = false;
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
        CurrentEditObject.Area.position =  position;
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
