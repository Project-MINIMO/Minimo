using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StarTester))]
public class StarTesterEditor : Editor
{
    private static bool _starFoldout = true;
    private static bool _lineFoldout = true;
    private static bool _rotationFoldout = true;
    private static bool _layerFoldout = true;
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        var tester = (StarTester)target;
        EditorGUILayout.BeginVertical(GUI.skin.box);
        _starFoldout = EditorGUILayout.Foldout(_starFoldout, "별", true, EditorStyles.foldoutHeader);
        if (_starFoldout)
        {
            if (GUILayout.Button("스프라이트 적용"))
            {
                tester.ApplySprites();
            }
        
            var newScale = EditorGUILayout.Slider("별 크기", tester.GetScale(), 0.1f, 5f);
            if (!Mathf.Approximately(newScale, tester.GetScale()))
            {
                tester.SetScale(newScale);
                tester.ApplyScale(newScale);
            }
            
            var newAmount = EditorGUILayout.IntSlider("별 최대 개수", StarManager.MaxCount, 0, 400);
            if (!Mathf.Approximately(newAmount, StarManager.MaxCount))
            {
                StarManager.MaxCount = newAmount;
            }
        }
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.BeginVertical(GUI.skin.box);
        _lineFoldout = EditorGUILayout.Foldout(_lineFoldout, "선", true, EditorStyles.foldoutHeader);
        if (_lineFoldout)
        {
            EditorGUI.BeginChangeCheck();
            var newColor = EditorGUILayout.ColorField("선 색상", tester.GetLineColor());
            if (EditorGUI.EndChangeCheck())
            {
                tester.SetLineColor(newColor);
                tester.ApplyLineColor();
                EditorUtility.SetDirty(tester);
            }
        
            EditorGUI.BeginChangeCheck();
            var newWidth = EditorGUILayout.Slider("선 굵기", tester.GetLineWidth(), 0.1f, 5f);
            if (EditorGUI.EndChangeCheck())
            {
                tester.SetLineWidth(newWidth);
                tester.ApplyLineWidth();
                EditorUtility.SetDirty(tester);
            }
        
            EditorGUI.BeginChangeCheck();
            var useMat = EditorGUILayout.Toggle("선 모양(스프라이트) 사용", tester.GetUseCustomMaterial());
            if (EditorGUI.EndChangeCheck())
            {
                tester.SetUseCustomMaterial(useMat);
                tester.ApplyLineMaterialMode();
                EditorUtility.SetDirty(tester);
            }

            if (tester.GetUseCustomMaterial())
            {
                EditorGUI.BeginChangeCheck();
                var newSprite = (Sprite)EditorGUILayout.ObjectField("스프라이트", tester.GetLineSprite(), typeof(Sprite), false);
                if (EditorGUI.EndChangeCheck())
                {
                    tester.SetLineSprite(newSprite);
                    tester.ApplyLineSpriteToMaterial();
                    EditorUtility.SetDirty(tester);
                }
            }
        }
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.BeginVertical(GUI.skin.box);
        _rotationFoldout = EditorGUILayout.Foldout(_rotationFoldout, "회전", true, EditorStyles.foldoutHeader);
        if (_rotationFoldout)
        {
            EditorGUI.BeginChangeCheck();
            var cw = EditorGUILayout.Toggle("회전 방향", tester.GetRotateClockwise());
            if (EditorGUI.EndChangeCheck())
            {
                tester.SetRotateClockwise(cw);
                EditorUtility.SetDirty(tester);
            }
        
            EditorGUI.BeginChangeCheck();
            var speed = EditorGUILayout.Slider("회전 속도", tester.GetRotateSpeed(), 0f, 5f);
            if (EditorGUI.EndChangeCheck())
            {
                tester.SetRotateSpeed(speed);
                EditorUtility.SetDirty(tester);
            }
        }
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.BeginVertical(GUI.skin.box);
        _layerFoldout = EditorGUILayout.Foldout(_layerFoldout, "레이어 설정", true, EditorStyles.foldoutHeader);
        if (_layerFoldout)
        {
            EditorGUI.BeginChangeCheck();
            var fg = EditorGUILayout.Toggle("타일 아래로", tester.GetIsForeground());
            if (EditorGUI.EndChangeCheck())
            {
                tester.SetIsForeground(fg);
                EditorUtility.SetDirty(tester);
            }
        }
        EditorGUILayout.EndVertical();
        
        if (GUI.changed)
        {
            EditorUtility.SetDirty(tester);
        }
    }
}