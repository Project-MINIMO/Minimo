using System;
using Cysharp.Threading.Tasks;
using MinimoShared;

using UnityEngine;
using UnityEngine.Serialization;

public class BuildingObject : InteractObject
{
    public BoundsInt Area;
    public BoundsInt PreviousArea { get; private set; }
    public BuildingData Data { get; private set; }

    protected int _id;

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
        Data = data;
        
        var size = new Vector3Int(data.SizeX, data.SizeY, 1);
        Area = new BoundsInt(_editManager.GetCellPosition(transform.position), size);
        PreviousArea = Area;
        
        transform.position = _editManager.GetWorldPosition(Area.position);
    }
    
    public virtual void Initialize(BuildingDTO buildingDto)
    {
        _id = buildingDto.Id;
        IsPlaced = true;

        var buildingString = buildingDto.BuildingType;
        buildingString = buildingString.Replace("Building", "");
        var buildingType = (EBuilding)Enum.Parse(typeof(EBuilding), buildingString);
        var buildingData = App.GetData<TitleData>().Building[buildingType];
        Initialize(buildingData);
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
        var buildingType = "Building_" + Data.ID;
        var newBuildingRequest = new BuildingDTO
        {
            BuildingType = buildingType,
            Position = new int[] {Area.position.x, Area.position.y, Area.position.z},
        };
        
        var newBuildingDto = await _buildingManager.CreateBuildingAsync(newBuildingRequest);
        if (newBuildingDto != null)
        {
            Debug.Log($"Building created: {newBuildingDto.BuildingType} (ID: {newBuildingDto.Id})");
            _id = newBuildingDto.Id;
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
            Id = _id,
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
