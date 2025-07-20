using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class BuildingObject : InteractObject
{
    public string BuildingId { get;  set; } // 파이어베이스에 저장된 건물 ID
    public Vector3 PreviousPosition { get; set; }
    public BuildingData BuildingData { get; set; }
    public BuildingPositionData PositionData { get; private set; }

    private bool _isPlaced;
    private bool _isFlipped = false;
    
    protected EditManager _editManager;
    private SpriteRenderer _spriteRenderer;

    protected virtual void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _editManager = App.GetManager<EditManager>();
    }
    
    public virtual async Task Initialize(BuildingData data)
    {
        try
        {
            BuildingData = data;
            await LoadPositionData();
            
            PreviousPosition = transform.position;
            
            _editManager.StartEdit(this, true);
        }
        catch (Exception e)
        {
            throw; // TODO 예외 처리
        }
    }
    
    public virtual void Initialize(int id)
    {
        _isPlaced = true;
        
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
                _spriteRenderer.transform.localPosition = new Vector3(PositionData.Offset.x, PositionData.Offset.y, 0);
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

    public async Task<bool> Install()
    {
        if (_isPlaced)
        {
            return await UpdateBuilding();
        }
        else
        {
            return await CreateBuilding();
        }
    }

    protected virtual async Task<bool> CreateBuilding()
    {
        // Firebase. 건물 설치 요청
        var firebaseManager = App.GetManager<FirebaseManager>();
        var targetCell = _editManager.GetCellPosition(transform.position);

        string? buildingId = await firebaseManager.InstallBuilding(BuildingData.ID, targetCell);
        if (buildingId == null)
        {
            Debug.LogError("Building installation failed.");
            return false;
        }

        this.BuildingId = buildingId;

        _isPlaced = true;
        PreviousPosition = transform.position;
        EndEdit();
        return true;
    }
    
    private async Task<bool> UpdateBuilding()
    {
        // Firebase. 건물 위치 업데이트 요청
        var firebaseManager = App.GetManager<FirebaseManager>();
        var targetCell = _editManager.GetCellPosition(transform.position);
        var success = await firebaseManager.MoveBuilding(BuildingId, targetCell);
        if (!success)
        {
            Debug.LogError("Building position update failed.");
            return false;
        }
        
        PreviousPosition = transform.position;
        EndEdit();
        return true;
    }
  
    public bool Cancel()
    {
        if (_isPlaced)
        {
            _editManager.MoveObject(PreviousPosition);
            EndEdit();
        }
        else
        {
            Destroy(gameObject);
        }

        return _isPlaced;
    }

    public void Rotate()
    {
        transform.Rotate(0, _isFlipped ? -180 : 180, 0);
        _isFlipped = !_isFlipped;
    }
    #endregion
}
