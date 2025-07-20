using System.Threading.Tasks;
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
    [SerializeField] private GameObject _objectPrefab;
    
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
        var firebaseManager = App.GetManager<FirebaseManager>();
        var buildings = await firebaseManager.LoadUserBuildings();

        foreach (var building in buildings)
        {
            // 임시 필터링: 필요 없으면 제거 가능
            if (building.BuildingDataId == -1) continue;

            // 프리팹 경로 추정 예시 (ID -> 리소스 경로 변환 필요 시 매핑 테이블화)
            var prefabPath = $"Building/GridObject";
            var prefab = Resources.Load<GameObject>(prefabPath);
            if (prefab == null)
            {
                Debug.LogWarning($"Prefab not found: {prefabPath}");
                continue;
            }

            // 셀 위치를 월드 위치로 변환
            var cellPosition = _gridLayout.CellToWorld(building.Position);
            
            var buildingData = App.GetData<TitleData>().Building[building.BuildingDataId];
            var produce = await CreateObject(buildingData, cellPosition);
            if (produce != null)
            {
                produce.BuildingId = building.BuildingId;
                produce.PreviousPosition = cellPosition;
                produce.transform.position = cellPosition;
                _tileStateModifier.ModifyTileState(produce, TileState.Installed);
                CurrentEditObject = null;
                IsEditing.Value = false;
            }
        }
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
    
    public async void ConfirmEdit()
    {
        if (!_installChecker.CheckCanInstall(CurrentEditObject))
        {
            return;
        }

        var success = await CurrentEditObject.Install();
        var isNew = !CurrentEditObject.IsPlaced;

        if (success)
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
        else
        {
            Debug.LogError("Failed to confirm edit. Installation check failed or installation process failed.");
        }
    }
    
    public async Task<ProduceObject> CreateObject(Building data, Vector3 position)
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
            await produce.Initialize(data);
            return produce;
        }
        else
        {
            Debug.LogError("GridObject component not found in instantiated prefab.");
            Destroy(gridObject.gameObject);
            return null;
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
