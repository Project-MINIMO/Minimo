using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class BuildingObject : InteractObject
{
    public BoundsInt Area;
    public BoundsInt PreviousArea { get; private set; }
    
    public BuildingData BuildingData { get; private set; }
    public BuildingPositionData PositionData { get; private set; }

    public bool IsPlaced { get; private set; } 
    private bool _isFlipped = false;
    
    protected EditManager _editManager;
    private SpriteRenderer _spriteRenderer;

    protected virtual void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _editManager = App.GetManager<EditManager>();
    }
    
    public virtual async void Initialize(BuildingData data)
    {
        try
        {
            BuildingData = data;
            await LoadPositionData();
        
            var size = new Vector3Int(1, 1, 1/*data.SizeX, data.SizeY, 1*/);
            Area = new BoundsInt(_editManager.GetCellPosition(transform.position), size);
            PreviousArea = Area;
        
            transform.position = _editManager.GetWorldPosition(Area.position);
            _editManager.StartEdit(this);
        }
        catch (Exception e)
        {
            throw; // TODO 예외 처리
        }
    }
    
    public virtual void Initialize(int id)
    {
        IsPlaced = true;
        
        var buildingData = App.GetData<TitleData>().Building[id];
        Initialize(buildingData);
    }
    
    private async Task LoadPositionData()
    {
        try
        {
            var path = $"Assets/09. Scriptable Objects/Building/{BuildingData.Name}.asset";
            var handle = Addressables.LoadAssetAsync<BuildingPositionData>(path);
            await handle.Task;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                PositionData = handle.Result;
                SetPolygonCollider(GetComponent<PolygonCollider2D>());
                GetComponent<PolygonCollider2D>().offset = PositionData.ColliderOffset;
                _spriteRenderer.sprite = PositionData.Sprite;
                _spriteRenderer.transform.position = new Vector3(PositionData.Offset.x, PositionData.Offset.y, 0);
            }
            else
            {
                Debug.LogError("Failed to load BuildingData");
            }
        }
        catch (Exception e)
        {
            throw; // TODO 예외 처리
        }
    }
    
    private void SetPolygonCollider(PolygonCollider2D polyCollider)
    {
        var tileSet = new HashSet<Vector2Int>();
        foreach (var pos in PositionData.GroundTilePositions)
            tileSet.Add(pos);
        foreach (var pos in PositionData.WaterTilePositions)
            tileSet.Add(pos);
        
        var polygonPoints = ColliderGenerator.GenerateIsoPolygonCentered(tileSet);
        
        var simplifiedPolygon = ColliderGenerator.SimplifyPolygon(polygonPoints, 0.1f);
    
        if (simplifiedPolygon is { Count: > 0 })
        {
            polyCollider.pathCount = 1;
            polyCollider.SetPath(0, simplifiedPolygon.ToArray());
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

    public bool Install()
    {
        if (IsPlaced)
        {
            return UpdateBuilding();
        }
        else
        {
            return CreateBuilding();
        }
    }

    private bool CreateBuilding()
    {
        IsPlaced = true;
        PreviousArea = Area;
        EndEdit();
        return true;
    }
    
    private bool UpdateBuilding()
    {
        PreviousArea = Area;
        EndEdit();
        return true;
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
