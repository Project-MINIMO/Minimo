using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BuildingPositionData))]
public class BuildingDataEditor : Editor
{
    private void OnSceneGUI()
    {
        //BuildingPositionData data = (BuildingPositionData)target;

        //if (data == null) return;

        //Handles.color = Color.green;
        //Vector3 position = Vector3.zero; // 건물의 실제 배치 위치
        //Vector3 size = new Vector3(data.size.x, data.size.y, 0);

        //Handles.DrawWireCube(position, size);
        //SceneView.RepaintAll();
    }
}