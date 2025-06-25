using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class BuildingEditorWindow : EditorWindow
{
    #region Fields
    // Building data related
    private BuildingPositionData _data;
    private string _name = "New Building";
    private Vector2 _offset = Vector2.zero;
    private Vector2 _colliderOffset = Vector2.zero;
    private Sprite _sprite;
    private bool _isFlipped;

    // Tile range list (land/water)
    private List<Vector2Int> _groundTilePositions = new();
    private List<Vector2Int> _waterTilePositions = new();

    // Settings related to tilemap and tile palette
    private Tilemap _targetTilemap;
    private TileBase _groundTile; 
    private TileBase _waterTile;

    // Currently selected tile type (ground/water) - selected in GUI
    private TileType _selectedTileType = TileType.Ground;
    
    // Constants for isometric tile size
    private const float TileWidth = 1f;
    private const float TileHeight = 0.5f;
    private const int TileZ = 0;
    #endregion
    
    #region Menu & Initialization
    [MenuItem("Tools/Building Editor")]
    public static void ShowWindow()
    {
        GetWindow<BuildingEditorWindow>("Building Editor");
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
        
        if (_targetTilemap == null)
        {
            _targetTilemap = FindObjectOfType<Tilemap>();
        }
        
        if (_groundTile == null)
        {
            _groundTile = AssetDatabase.LoadAssetAtPath<TileBase>("Assets/03. Images/Tile/Check/grass.asset");
        }
        if (_waterTile == null)
        {
            _waterTile = AssetDatabase.LoadAssetAtPath<TileBase>("Assets/03. Images/Tile/Check/water.asset");
        }
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        
        ClearTilemap();
    }

    private void OnGUI()
    {
        GUILayout.Label("수정할 빌딩 데이터", EditorStyles.boldLabel);
        var newData = (BuildingPositionData)EditorGUILayout.ObjectField("빌딩 데이터", _data, typeof(BuildingPositionData), false);
        if (newData != _data)
        {
            _data = newData;
            if (_data != null)
            {
                LoadBuildingData(_data);
            }
            else
            {
                ClearTilemap();
            }
        }

        GUILayout.Space(10);
        if (_data != null)
        {
            if (GUILayout.Button("새로운 데이터 생성하기"))
            {
                ResetBuildingData();
            }
        }

        GUILayout.Space(10);
        GUILayout.Label("건물 데이터 설정", EditorStyles.boldLabel);
        _name = EditorGUILayout.TextField("건물 이름", _name);
        _offset = EditorGUILayout.Vector2Field("이미지 오프셋", _offset);
        _colliderOffset = EditorGUILayout.Vector2Field("콜라이더 오프셋", _colliderOffset);
        _sprite = (Sprite)EditorGUILayout.ObjectField("건물 스프라이트", _sprite, typeof(Sprite), false);

        GUILayout.BeginHorizontal();
        GUILayout.Label("회전 상태: " + (_isFlipped ? "플립됨" : "정상"));
        if (GUILayout.Button("회전하기"))
        {
            FlipBuilding();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        if (GUILayout.Button("저장하기"))
        {
            SaveBuildingData();
        }

        GUILayout.Space(10);
        GUILayout.Label("타일 종류 선택", EditorStyles.boldLabel);
        _selectedTileType = (TileType)EditorGUILayout.EnumPopup("타일 타입", _selectedTileType);
    }
    #endregion

    #region Data Loading & Saving
    private void LoadBuildingData(BuildingPositionData data)
    {
        _name = data.Code;
        _offset = data.Offset;
        _colliderOffset = data.ColliderOffset;
        _sprite = data.Sprite;
        _groundTilePositions = new List<Vector2Int>(data.GroundTilePositions);
        _waterTilePositions = new List<Vector2Int>(data.WaterTilePositions);
        _isFlipped = false;
        RefreshTilemapFromData();
    }

    private void ResetBuildingData()
    {
        _data = null;
        _name = string.Empty;
        _offset = Vector2.zero;
        _colliderOffset = Vector2.zero;
        _sprite = null;
        _groundTilePositions = new List<Vector2Int>();
        _waterTilePositions = new List<Vector2Int>();
        _isFlipped = false;
        ClearTilemap();
    }

    private void SaveBuildingData()
    {
        if (_data == null)
        {
            _data = CreateInstance<BuildingPositionData>();
            var assetPath = $"Assets/09. Scriptable Objects/Building/{_name}.asset";
            AssetDatabase.CreateAsset(_data, assetPath);
            Debug.Log($"{_name} 생성 완료! 경로: {assetPath}");
        }
        else
        {
            Debug.Log($"{_name} 업데이트 완료!");
        }

        _data.Code = _name;
        _data.Offset = _offset;
        _data.ColliderOffset = _colliderOffset;
        _data.Sprite = _sprite;
        _data.GroundTilePositions = new List<Vector2Int>(_groundTilePositions);
        _data.WaterTilePositions = new List<Vector2Int>(_waterTilePositions);

        EditorUtility.SetDirty(_data);
        AssetDatabase.SaveAssets();
    }
    #endregion

    #region Tilemap Helper Methods
    private void ClearTilemap()
    {
        if (_targetTilemap != null)
        {
            _targetTilemap.ClearAllTiles();
        }
    }

    private void RefreshTilemapFromData()
    {
        if (_targetTilemap == null) return;
        
        ClearTilemap();
        ApplyTiles(_groundTilePositions, _groundTile);
        ApplyTiles(_waterTilePositions, _waterTile);
    }

    private void ApplyTiles(List<Vector2Int> positions, TileBase tile)
    {
        foreach (var pos in positions)
        {
            SetTileInTilemap(pos, tile);
        }
    }

    private void SetTileInTilemap(Vector2Int pos, TileBase tile)
    {
        if (_targetTilemap != null)
        {
            _targetTilemap.SetTile(new Vector3Int(pos.x, pos.y, TileZ), tile);
        }
    }

    private void ClearTileAt(Vector2Int pos)
    {
        SetTileInTilemap(pos, null);
    }
    #endregion

    #region Mouse Input & Tile Operations
    private void OnSceneGUI(SceneView sceneView)
    {
        var e = Event.current;
        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        HandleUtility.AddDefaultControl(controlID);

        var basePosition = new Vector3(_offset.x, _offset.y, 0);

        if (e.type is EventType.MouseDown or EventType.MouseDrag && !e.alt)
        {
            var worldPos = HandleUtility.GUIPointToWorldRay(e.mousePosition).origin;
            var localPos = worldPos - basePosition;
            var clickedTile = GetTileCoordinateFromLocalPos(localPos);

            switch (e.button)
            {
                // Left click: Add tile
                case 0:
                {
                    switch (_selectedTileType)
                    {
                        case TileType.Ground:
                            AddGroundTile(clickedTile);
                            break;
                        
                        case TileType.Water:
                            AddWaterTile(clickedTile);
                            break;
                    }
                    e.Use();
                    break;
                }
                
                // Right click: Delete tile
                case 1:
                    RemoveTile(clickedTile);
                    e.Use();
                    break;
            }
        }

        DrawBuildingSpritePreview();
        SceneView.RepaintAll();
    }

    private Vector2Int GetTileCoordinateFromLocalPos(Vector3 localPos)
    {
        var halfTileWidth = TileWidth / 2f;
        var halfTileHeight = TileHeight / 2f;
        var gridX = ((localPos.y / halfTileHeight) + (localPos.x / halfTileWidth)) / 2f;
        var gridY = ((localPos.y / halfTileHeight) - (localPos.x / halfTileWidth)) / 2f;
        return new Vector2Int(Mathf.FloorToInt(gridX), Mathf.FloorToInt(gridY));
    }

    private void AddGroundTile(Vector2Int tile)
    {
        if (_waterTilePositions.Contains(tile))
        {
            _waterTilePositions.Remove(tile);
            ClearTileAt(tile);
        }

        if (!_groundTilePositions.Contains(tile))
        {
            _groundTilePositions.Add(tile);
            SetTileInTilemap(tile, _groundTile);
        }
    }

    private void AddWaterTile(Vector2Int tile)
    {
        if (_groundTilePositions.Contains(tile))
        {
            _groundTilePositions.Remove(tile);
            ClearTileAt(tile);
        }
        
        if (!_waterTilePositions.Contains(tile))
        {
            _waterTilePositions.Add(tile);
            SetTileInTilemap(tile, _waterTile);
        }
    }

    private void RemoveTile(Vector2Int tile)
    {
        var removed = false;
        
        if (_groundTilePositions.Contains(tile))
        {
            _groundTilePositions.Remove(tile);
            removed = true;
        }
        
        if (_waterTilePositions.Contains(tile))
        {
            _waterTilePositions.Remove(tile);
            removed = true;
        }

        if (removed)
        {
            ClearTileAt(tile);
        }
    }

    private void DrawBuildingSpritePreview()
    {
        if (_sprite == null) return;

        Handles.BeginGUI();
        var worldPos = new Vector3(_offset.x, _offset.y, 0);
        var screenPos = HandleUtility.WorldToGUIPoint(worldPos);

        var worldWidth = _sprite.rect.width / _sprite.pixelsPerUnit;
        var worldHeight = _sprite.rect.height / _sprite.pixelsPerUnit;
        var screenSize = HandleUtility.WorldToGUIPoint(worldPos + new Vector3(worldWidth, worldHeight, 0)) - screenPos;
        var spriteRect = new Rect(screenPos.x - screenSize.x / 2, screenPos.y - screenSize.y / 2, screenSize.x, screenSize.y);

        var prevColor = GUI.color;
        GUI.color = new Color(1, 1, 1, 0.5f);

        var texCoords = _isFlipped ? new Rect(1f, 1f, -1f, -1f) : new Rect(0f, 1f, 1f, -1f);
        GUI.DrawTextureWithTexCoords(spriteRect, _sprite.texture, texCoords);

        GUI.color = prevColor;
        Handles.EndGUI();
    }
    #endregion

    #region Flip Building
    private void FlipBuilding()
    {
        _isFlipped = !_isFlipped;
        
        ClearTilemap();
        
        var allPositions = new List<Vector2Int>();
        allPositions.AddRange(_groundTilePositions);
        allPositions.AddRange(_waterTilePositions);
        if (allPositions.Count == 0) return;

        // Find the world center of a building
        var sumWorld = Vector2.zero;
        foreach (var pos in allPositions)
        {
            var wp = GetIsometricPosition(pos, Vector3.zero) + (Vector3)_offset;
            sumWorld += new Vector2(wp.x, wp.y);
        }
        var centerWorld = sumWorld / allPositions.Count;

        // Apply flip
        _groundTilePositions = FlipTilePositions(_groundTilePositions, centerWorld);
        _waterTilePositions = FlipTilePositions(_waterTilePositions, centerWorld);

        // Reapply flipped tiles
        ApplyTiles(_groundTilePositions, _groundTile);
        ApplyTiles(_waterTilePositions, _waterTile);
    }

    private List<Vector2Int> FlipTilePositions(List<Vector2Int> positions, Vector2 centerWorld)
    {
        var flipped = new List<Vector2Int>();
        foreach (var pos in positions)
        {
            var origWorld = GetIsometricPosition(pos, Vector3.zero) + (Vector3)_offset;
            var newWorld = new Vector2(2 * centerWorld.x - origWorld.x, origWorld.y);
            var newGrid = WorldToGrid(newWorld);
            
            flipped.Add(newGrid);
        }
        
        return flipped;
    }
    #endregion

    #region Coordinate Conversion
    private Vector3 GetIsometricPosition(Vector2Int tile, Vector3 basePosition)
    {
        var isoX = (tile.x - tile.y) * (TileWidth / 2f);
        var isoY = (tile.x + tile.y) * (TileHeight / 2f);
        return new Vector3(isoX, isoY, 0) + basePosition;
    }

    private Vector2Int WorldToGrid(Vector2 worldPos)
    {
        var rel = worldPos - _offset;
        var gridX = rel.x / TileWidth + rel.y / TileHeight;
        var gridY = rel.y / TileHeight - rel.x / TileWidth;
        return new Vector2Int(Mathf.RoundToInt(gridX), Mathf.RoundToInt(gridY));
    }
    #endregion
}