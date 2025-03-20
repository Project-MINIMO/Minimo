using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewBuildingData", menuName = "Building Position Data")]
public class BuildingPositionData : ScriptableObject
{
    public string Code;  
    public List<Vector2Int> GroundTilePositions = new(); 
    public List<Vector2Int> WaterTilePositions = new();  
    public Vector2 Offset;
    public Sprite Sprite;
}