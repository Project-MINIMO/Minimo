using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UniRx;

public class BuildingObject : InteractObject
{
    public string BuildingId { get;  set; } // 파이어베이스에 저장된 건물 ID
    public Vector3 PreviousPosition { get; set; }
    public Building BuildingData { get; set; }
    public BuildingPositionData PositionData { get; private set; }

    public bool IsPlaced {get; set;}
    
    protected EditManager EditManager;
    protected SpriteRenderer SpriteRenderer;

    protected virtual void Awake()
    {
        SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        EditManager = App.GetManager<EditManager>();
        EditManager.IsBuildingEditing
            .Subscribe(isEditing => SetTransparency(isEditing ? 0.5f : 1)).AddTo(gameObject);
        EditManager.IsTileEditing
            .Subscribe(isEditing => SetTransparency(isEditing ? 0.5f : 1)).AddTo(gameObject);
        SetTransparency(EditManager.IsBuildingEditing.Value ? 0.5f : 1);
    }
    
    public virtual async Task Initialize(Building data)
    {
        BuildingData = data;
        PositionData = BuildingData.Position;
        
        SetPolygonCollider(GetComponent<PolygonCollider2D>());
        GetComponent<PolygonCollider2D>().offset = PositionData.ColliderOffset;
        SpriteRenderer.sprite = PositionData.Sprite;
        SpriteRenderer.transform.localPosition = new Vector3(PositionData.Offset.x, PositionData.Offset.y, 0);
            
        PreviousPosition = transform.position;
        
        BuildingData.AddCount(1);
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

    #region InteractObject
    public override void OnLongPress()
    {
        if (EditManager.IsBuildingEditing.Value) return;
        if (EditManager.IsTileEditing.Value) return;
        
        EditManager.StartEdit(this);
    }

    public override void OnClickUp()
    {
        if (EditManager.IsTileEditing.Value) return;
        
        if (EditManager.IsBuildingEditing.Value)
        {
            EditManager.StartEdit(this);
        }
    }
    #endregion
    
    #region Edit Functions
    private void SetTransparency(float alpha)
    {
        var color = SpriteRenderer.color;
        color.a = alpha;
        SpriteRenderer.color = color;
    }

    public async Task<bool> Install()
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

    protected async Task<bool> CreateBuilding()
    {
        // Firebase. 건물 설치 요청
        var firebaseManager = App.GetManager<FirebaseManager>();
        var targetCell = EditManager.GetCellPosition(transform.position);

        string? buildingId = await firebaseManager.InstallBuilding(BuildingData.ID, targetCell);
        if (buildingId == null)
        {
            Debug.LogError("Building installation failed.");
            return false;
        }

        this.BuildingId = buildingId;

        IsPlaced = true;
        PreviousPosition = transform.position;
        return true;
    }
    
    private async Task<bool> UpdateBuilding()
    {
        // Firebase. 건물 위치 업데이트 요청
        var firebaseManager = App.GetManager<FirebaseManager>();
        var targetCell = EditManager.GetCellPosition(transform.position);
        var success = await firebaseManager.MoveBuilding(BuildingId, targetCell);
        if (!success)
        {
            Debug.LogError("Building position update failed.");
            return false;
        }
        
        PreviousPosition = transform.position;
        return true;
    }
  
    public bool Cancel()
    {
        if (IsPlaced)
        {
            EditManager.MoveObject(PreviousPosition);
        }
        else
        {
            Destroy();
        }

        return IsPlaced;
    }

    public void Rotate()
    {
        SpriteRenderer.flipX = !SpriteRenderer.flipX;
    }

    public virtual void Destroy()
    {
        BuildingData.AddCount(-1);
        Destroy(gameObject);
    }
    #endregion
}
