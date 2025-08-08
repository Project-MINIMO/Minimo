using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EditManager : ManagerBase
{
    public ReactiveProperty<bool> IsBuildingEditing { get; } = new(false);
    public ReactiveProperty<bool> IsTileEditing { get; } = new(false);
    public ReactiveProperty<Vector3> CurrentCellPosition { get; } = new();
    public BuildingObject CurrentEditObject { get; private set; }
    public ReactiveCollection<ProduceObject> ActiveProduces { get; } = new();
    
    [SerializeField] private GridLayout _gridLayout;
    [SerializeField] private Transform _buildingParent;
    [SerializeField] private GameObject _objectPrefab;
    
    private InstallChecker _installChecker;
    private TileStateModifier _tileStateModifier;

    private void Start()
    {
        _installChecker = GetComponent<InstallChecker>();
        _tileStateModifier = GetComponent<TileStateModifier>();

        LoadExistingBuildingsAsync().Forget();
    }
    
    private async UniTaskVoid LoadExistingBuildingsAsync()
    {
        var firebaseManager = App.GetManager<FirebaseManager>();
        var titleData = App.GetData<TitleData>();

        // --- 타일 서버에서 불러와 설치 ---
        var tiles = await firebaseManager.LoadUserTiles();
        var tilemap = GameObject.FindWithTag("VillageTilemap").GetComponent<Tilemap>();
        var glowMap = GameObject.FindWithTag("GlowTilemap").GetComponent<Tilemap>();
        foreach (var tile in tiles)
        {
            var customTile = titleData.CustomTile[tile.TileId];
            tilemap.SetTile(tile.Position, customTile.Tile);
            glowMap.SetTile(tile.Position, customTile.Tile);
        }

        var buildings = await firebaseManager.LoadUserBuildings();
        foreach (var building in buildings)
        {
            // 임시 필터링: 필요 없으면 제거 가능
            if (building.BuildingDataId < 0) continue;
            
            // 셀 위치를 월드 위치로 변환
            var cellPosition = _gridLayout.CellToWorld(building.Position);
            var buildingData = titleData.Building[building.BuildingDataId];
            var produce = await SpawnBuildingAsync(buildingData, cellPosition);

            if (produce == null) continue;
            
            produce.BuildingId = building.BuildingId;
            produce.PreviousPosition = cellPosition;
            produce.transform.position = cellPosition;
            produce.IsPlaced = true;
            _tileStateModifier.ModifyTileState(produce, TileState.Installed);
            ActiveProduces.Add(produce);
        }
    }

    public async UniTask CreateAndEditAsync(Building data, Vector3 position)
    {
        var obj = await SpawnBuildingAsync(data, AlignToCell(position));
        StartEdit(obj, isNew: true);
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
        IsBuildingEditing.Value = true;
        
        CurrentCellPosition.Value = CurrentEditObject.transform.position;
    }

    public void CancelEdit()
    {
        if (CurrentEditObject.Cancel())
        {
            _tileStateModifier.ModifyTileState(CurrentEditObject, TileState.Installed);
        }
        
        CurrentEditObject = null;
        IsBuildingEditing.Value = false;
    }
    
    public async UniTask ConfirmEdit()
    {
        if (!_installChecker.CheckCanInstall(CurrentEditObject)) return;

        var isNew = !CurrentEditObject.IsPlaced;
        if (isNew)
        {
            if (!CurrentEditObject.BuildingData.CanInstall)
            {
                App.Notification(NotifyType.GoldLack);
                return;
            }
        }
        
        if (!await CurrentEditObject.Install())
        {
            Debug.LogError("Installation failed");
            return;
        }
       
        _tileStateModifier.ModifyTileState(CurrentEditObject, TileState.Installed);
            
        if (isNew)
        {
            CurrentEditObject.BuildingData.Install();
            ActiveProduces.Add(CurrentEditObject as ProduceObject);
            HandlePostInstall();
        }
        else
        {
            CurrentEditObject = null;
            IsBuildingEditing.Value = false;
        }
    }
    
    private void HandlePostInstall()
    {
        var nextCell = _gridLayout.WorldToCell(CurrentEditObject.transform.position) + Vector3Int.down;
        var buildingData = CurrentEditObject.BuildingData;
        CurrentEditObject = null;
        IsBuildingEditing.Value = false;
        CreateAndEditAsync(buildingData, _gridLayout.CellToWorld(nextCell)).Forget();
    }
    
    public void RotateObject()
    {
        if (!IsBuildingEditing.Value) return;
        
        CurrentEditObject.Rotate();
    }

    public void DeleteObject()
    {
        if (!IsBuildingEditing.Value) return;
        
        var result = CurrentEditObject.Destroy();
        if (result)
        {
            CurrentEditObject.BuildingData.Uninstall();
            ActiveProduces.Remove(CurrentEditObject as ProduceObject);
            CurrentEditObject = null;
            IsBuildingEditing.Value = false;
        }
    }

    public void MoveObject(Vector3 worldPosition)
    {
        if (!IsBuildingEditing.Value) return;
        
        var aligned = AlignToCell(worldPosition);
        CurrentEditObject.transform.position = aligned;
        if (CurrentCellPosition.Value != aligned)
        {
            CurrentCellPosition.Value = aligned;
        }
    }
    
    public Vector3 AlignToCell(Vector3 worldPos)
    {
        var cell = _gridLayout.WorldToCell(worldPos);
        return _gridLayout.CellToWorld(cell);
    }
    
    public void SetTileEditing(bool isEditing)
    {
        IsTileEditing.Value = isEditing;
    }
    
    public Vector3Int GetCellPosition(Vector3 position)
    {
        return _gridLayout.WorldToCell(position);
    }
    
    private ProduceObject AddComponentByBuildingType(GameObject gridObject, BuildingType type)
    {
        return type switch
        {
            BuildingType.Tier1 => gridObject.AddComponent<ProducePrimary>(),
            BuildingType.Tier2 => gridObject.AddComponent<ProduceSecondary>(),
            BuildingType.Tier3 => gridObject.AddComponent<ProduceTertiary>(),
            _ => gridObject.AddComponent<ProduceQuaternary>(),
        };
    }
    
    public async Task<ProduceObject> SpawnBuildingAsync(Building data, Vector3 position)
    {
        var gridObject = Instantiate(_objectPrefab, position, Quaternion.identity, _buildingParent);
        var compenet = AddComponentByBuildingType(gridObject, data.Type);
        if (compenet == null)
        {
            Destroy(gridObject);
            Debug.LogError("Missing ProduceObject component");
            return null;
        }
        await compenet.Initialize(data);
        return compenet;
    }
    
    public async UniTask Install(BuildingObject buildingObject)
    {
        if (!await buildingObject.Install())
        {
            Debug.LogError("Installation failed");
            return;
        }
        
        _tileStateModifier.ModifyTileState(buildingObject, TileState.Installed);
        ActiveProduces.Add(CurrentEditObject as ProduceObject);
    }
}
