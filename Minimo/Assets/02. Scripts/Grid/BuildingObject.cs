using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MinimoShared;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class BuildingObject : InteractObject
{
    public BoundsInt Area;
    public BoundsInt PreviousArea { get; private set; }
    
    public BuildingData BuildingData { get; private set; }
    public BuildingPositionData PositionData { get; private set; }

    protected int ID;

    public bool IsPlaced { get; private set; } 
    private bool _isFlipped = false;
   
    protected BuildingManager _buildingManager;
    protected EditManager _editManager;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        
        _editManager = App.GetManager<EditManager>();
        _buildingManager = App.GetManager<BuildingManager>();
    }
    
    public virtual void Initialize(BuildingData data)
    {
        BuildingData = data;
        LoadPositionData();
        
        var size = new Vector3Int(1, 1, 1/*data.SizeX, data.SizeY, 1*/);
        Area = new BoundsInt(_editManager.GetCellPosition(transform.position), size);
        PreviousArea = Area;
        
        transform.position = _editManager.GetWorldPosition(Area.position);
    }
    
    public virtual void Initialize(BuildingDTO buildingDto)
    {
        ID = buildingDto.Id;
        IsPlaced = true;

        var buildingString = buildingDto.BuildingType;
        buildingString = buildingString.Replace("Building_", "");
        var buildingType = (EBuilding)Enum.Parse(typeof(EBuilding), buildingString);
        var buildingData = App.GetData<TitleData>().Building[buildingType];
        Initialize(buildingData);
    }
    
    private async void LoadPositionData()
    {
        var path = $"Assets/09. Scriptable Objects/Building/{BuildingData.ID}.asset";
        var handle = Addressables.LoadAssetAsync<BuildingPositionData>(path);
        await handle.Task;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            PositionData = handle.Result;
            SetPolygonCollider(GetComponent<PolygonCollider2D>());
            Debug.Log($"BuildingData loaded: {PositionData.Code}");
        }
        else
        {
            Debug.LogError("Failed to load BuildingData");
        }
    }
    
    private void SetPolygonCollider(PolygonCollider2D polyCollider)
    {
        // SO에 저장된 땅, 물 타일 상대 좌표들을 합친 집합 생성
        HashSet<Vector2Int> tileSet = new HashSet<Vector2Int>();
        foreach (var pos in PositionData.GroundTilePositions)
            tileSet.Add(pos);
        foreach (var pos in PositionData.WaterTilePositions)
            tileSet.Add(pos);
    
        // 건물 로컬 좌표계에 맞게 외곽선(볼록 껍질) 생성 후, 재중심화
        List<Vector2> polygonPoints = ColliderGenerator.GenerateIsoPolygonCentered(tileSet);
    
        // (필요하다면 추가로 단순화)
        List<Vector2> simplifiedPolygon = ColliderGenerator.SimplifyPolygon(polygonPoints, 0.1f);
    
        if (simplifiedPolygon != null && simplifiedPolygon.Count > 0)
        {
            polyCollider.pathCount = 1;
            polyCollider.SetPath(0, simplifiedPolygon.ToArray());
            Debug.Log("Collider vertices count: " + simplifiedPolygon.Count);
        }
    }

    public override void OnLongPress()
    {
        if (_editManager.IsEditing.Value) return;
        
        _editManager.StartEdit(this);
    }

    public override void OnClickUp()
    {
        if (_editManager.IsEditing.Value)
        {
            _editManager.StartEdit(this);
        }
    }
    
    #region Edit Functions
    public void StartEdit()
    {
        SetTransparency(0.5f);
    }

    private void EndEdit()
    {
        SetTransparency(1f);
    }
    
    private void SetTransparency(float alpha)
    {
        var color = _spriteRenderer.color;
        color.a = alpha;
        _spriteRenderer.color = color;
    }

    public async UniTask<bool> Install()
    {
        if (IsPlaced)
        {
            return await UpdateBuilding();
        }
        else
        {
            return await CreateBuilding();
        }
    }

    private async UniTask<bool> CreateBuilding()
    {
        var buildingType = "Building_" + BuildingData.ID;
        var newBuildingRequest = new BuildingDTO
        {
            BuildingType = buildingType,
            Position = new int[] {Area.position.x, Area.position.y, Area.position.z},
        };
        
        var newBuildingDto = await _buildingManager.CreateBuildingAsync(newBuildingRequest);
        if (newBuildingDto != null)
        {
            Debug.Log($"Building created: {newBuildingDto.BuildingType} (ID: {newBuildingDto.Id})");
            ID = newBuildingDto.Id;
            IsPlaced = true;
            PreviousArea = Area;

            EndEdit();
            return true;
        }
        else
        {
            Debug.LogError("Failed to create building");
            return false;
        }
    }
    
    private async UniTask<bool> UpdateBuilding()
    {
        var updateBuildingParameter = new UpdateBuildingParameter
        {
            Id = ID,
            Position = new int[] {Area.position.x, Area.position.y, Area.position.z},
        };

        var updatedBuilding = await _buildingManager.UpdateBuildingAsync(updateBuildingParameter);
        if (updatedBuilding != null)
        {
            Debug.Log($"Building updated: {updatedBuilding.BuildingType} (ID: {updatedBuilding.Id})");
            PreviousArea = Area;

            EndEdit();
            return true;
        }
        else
        {
            Debug.LogError("Failed to create building");
            return false;
        }
    }
  
    public void Cancel()
    {
        if (IsPlaced)
        {
            _editManager.MoveObject(PreviousArea);
            EndEdit();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Rotate()
    {
        transform.Rotate(0, _isFlipped ? -180 : 180, 0);
        _isFlipped = !_isFlipped;
    }
    #endregion
}
