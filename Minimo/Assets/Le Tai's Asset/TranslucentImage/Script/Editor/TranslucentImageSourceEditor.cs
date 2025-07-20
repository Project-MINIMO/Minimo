using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LeTai.Asset.TranslucentImage.Editor
{
[CustomEditor(typeof(TranslucentImageSource))]
[CanEditMultipleObjects]
public class TranslucentImageSourceEditor : UnityEditor.Editor
{
    ScalableBlurConfigEditor configEditor;

    EditorProperty blurConfig;
    EditorProperty downsample;
    EditorProperty blurRegion;
    EditorProperty maxUpdateRate;
    EditorProperty cullPadding;
    EditorProperty backgroundFill;
    EditorProperty preview;
    EditorProperty skipCulling;

    void OnEnable()
    {
        blurConfig     = new EditorProperty(serializedObject, nameof(TranslucentImageSource.BlurConfig));
        downsample     = new EditorProperty(serializedObject, nameof(TranslucentImageSource.Downsample));
        blurRegion     = new EditorProperty(serializedObject, nameof(TranslucentImageSource.BlurRegion));
        maxUpdateRate  = new EditorProperty(serializedObject, nameof(TranslucentImageSource.MaxUpdateRate));
        cullPadding    = new EditorProperty(serializedObject, nameof(TranslucentImageSource.CullPadding));
        backgroundFill = new EditorProperty(serializedObject, nameof(TranslucentImageSource.BackgroundFill));
        preview        = new EditorProperty(serializedObject, nameof(TranslucentImageSource.Preview));
        skipCulling    = new EditorProperty(serializedObject, nameof(TranslucentImageSource.SkipCulling));
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();

        using (var configChange = new EditorGUI.ChangeCheckScope())
        {
            blurConfig.Draw();

            var curConfig = (ScalableBlurConfig)blurConfig.serializedProperty.objectReferenceValue;
            if (!curConfig)
            {
                EditorGUILayout.HelpBox("Missing Blur Config", MessageType.Warning);
                if (GUILayout.Button("New Blur Config File"))
                {
                    ScalableBlurConfig newConfig = CreateInstance<ScalableBlurConfig>();

                    var path = AssetDatabase.GenerateUniqueAssetPath(
                        $"Assets/{SceneManager.GetActiveScene().name} {serializedObject.targetObject.name} Blur Config.asset");
                    AssetDatabase.CreateAsset(newConfig, path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    EditorGUIUtility.PingObject(newConfig);
                    blurConfig.serializedProperty.objectReferenceValue = newConfig;
                }
            }
            else
            {
                if (!configEditor || configChange.changed)
                    configEditor = (ScalableBlurConfigEditor)CreateEditor(curConfig);

                configEditor.Draw();
            }
        }

        EditorGUILayout.Space();

        downsample.Draw();
        using (var blurRegionChange = new EditorGUI.ChangeCheckScope())
        {
            blurRegion.Draw();
            if (blurRegionChange.changed)
            {
                foreach (var source in targets.Cast<TranslucentImageSource>())
                {
                    source.OnBlurRegionChanged();
                }
            }
        }
        maxUpdateRate.Draw();
        if (maxUpdateRate.serializedProperty.floatValue < float.PositiveInfinity
         && skipCulling.serializedProperty.boolValue == false)
        {
            cullPadding.Draw();
            if (cullPadding.serializedProperty.floatValue <= 0)
                EditorGUILayout.HelpBox("When using a limited Max Update Rate with Culling enabled," +
                                        " add some padding to prevent culled area being shown during UI movement.", MessageType.Warning);
        }
        backgroundFill.Draw();
        preview.Draw();
        skipCulling.Draw();

        if (GUI.changed) serializedObject.ApplyModifiedProperties();
    }
}
}
