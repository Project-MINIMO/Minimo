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
        
        var newScale = EditorGUILayout.Slider("Star Scale", tester.GetScale(), 0.1f, 5f);
        if (!Mathf.Approximately(newScale, tester.GetScale()))
        {
            tester.SetScale(newScale);
            tester.ApplyScale(newScale);
        }
        
        EditorGUI.BeginChangeCheck();
        var newColor = EditorGUILayout.ColorField("Line Color (Multiply)", tester.GetLineColor());
        if (EditorGUI.EndChangeCheck())
        {
            tester.SetLineColor(newColor);
            tester.ApplyLineColor();
            EditorUtility.SetDirty(tester);
        }
        
        EditorGUI.BeginChangeCheck();
        var newWidth = EditorGUILayout.Slider("Line Width Multiplier", tester.GetLineWidth(), 0.1f, 5f);
        if (EditorGUI.EndChangeCheck())
        {
            tester.SetLineWidth(newWidth);
            tester.ApplyLineWidth();
            EditorUtility.SetDirty(tester);
        }
        
        EditorGUI.BeginChangeCheck();
        bool useMat = EditorGUILayout.Toggle("Use Custom Material", tester.GetUseCustomMaterial());
        if (EditorGUI.EndChangeCheck())
        {
            tester.SetUseCustomMaterial(useMat);
            tester.ApplyLineMaterialMode();
            EditorUtility.SetDirty(tester);
        }

        if (tester.GetUseCustomMaterial())
        {
            EditorGUI.BeginChangeCheck();
            var newSprite = (Sprite)EditorGUILayout.ObjectField("Line Sprite (BaseMap)", tester.GetLineSprite(), typeof(Sprite), false);
            if (EditorGUI.EndChangeCheck())
            {
                tester.SetLineSprite(newSprite);
                tester.ApplyLineSpriteToMaterial();
                EditorUtility.SetDirty(tester);
            }
        }
        
        EditorGUI.BeginChangeCheck();
        var cw = EditorGUILayout.Toggle("Rotate Clockwise", tester.GetRotateClockwise());
        if (EditorGUI.EndChangeCheck())
        {
            tester.SetRotateClockwise(cw);
            EditorUtility.SetDirty(tester);
        }
        
        EditorGUI.BeginChangeCheck();
        var speed = EditorGUILayout.Slider("Rotate Speed", tester.GetRotateSpeed(), 0f, 5f);
        if (EditorGUI.EndChangeCheck())
        {
            tester.SetRotateSpeed(speed);
            EditorUtility.SetDirty(tester);
        }
        
        if (GUI.changed)
        {
            EditorUtility.SetDirty(tester);
        }
    }
}