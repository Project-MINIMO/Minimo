using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StarTester))]
public class StarTesterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        var tester = (StarTester)target;
        if (GUILayout.Button("스프라이트 적용"))
        {
            tester.ApplySprites();
        }
        
        EditorGUILayout.Space();
        
        var newScale = EditorGUILayout.Slider("Star Scale", tester.GetScale(), 0.1f, 5f);
        if (!Mathf.Approximately(newScale, tester.GetScale()))
        {
            tester.SetScale(newScale);
            tester.ApplyScale(newScale);
        }

        EditorGUILayout.Space();
        
        if (GUI.changed)
        {
            EditorUtility.SetDirty(tester);
        }
    }
}