using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.Common.ScriptableObjects;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.ScriptableObjects.Fonts;
using Doozy.Editor.EditorUI.ScriptableObjects.Layouts;
using Doozy.Editor.EditorUI.ScriptableObjects.MicroAnimations;
using Doozy.Editor.EditorUI.ScriptableObjects.SpriteSheets;
using Doozy.Editor.EditorUI.ScriptableObjects.Styles;
using Doozy.Editor.EditorUI.ScriptableObjects.Textures;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBeMadeStatic.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.EditorUI.ScriptableObjects
{
    public class EditorUISettings : SingletonEditorScriptableObject<EditorUISettings>
    {
        public bool AutoRefresh;

        [RestoreData(nameof(EditorUISettings))]
        public static EditorUISettings RestoreData() =>
            instance;

        [MenuItem("Tools/Doozy/Refresh/EditorUI/Refresh All", priority = -450)]
        public static void Refresh()
        {
            if (EditorUtility.DisplayDialog
                (
                    $"Refresh the all the Editor UI databases?",
                    "This will regenerate all the databases with the latest registered items, from the source files." +
                    "\n\n" +
                    "Takes a few minutes, depending on the number of source files and your computer's performance." +
                    "\n\n" +
                    "This operation cannot be undone!",
                    "Yes",
                    "No"
                )
               )
            {
               ExecuteRefresh();
            }
        }

        [RefreshData(nameof(EditorUISettings))]
        public static void RefreshData() =>
            ExecuteRefresh();
        
        private static void ExecuteRefresh()
        {
            EditorDataColorDatabase.instance.RefreshDatabase();
            EditorDataFontDatabase.instance.RefreshDatabase();
            EditorDataLayoutDatabase.instance.RefreshDatabase();
            EditorDataMicroAnimationDatabase.instance.RefreshDatabase();
            EditorDataSelectableColorDatabase.instance.RefreshDatabase();
            EditorDataSpriteSheetDatabase.instance.RefreshDatabase();
            EditorDataStyleDatabase.instance.RefreshDatabase();
            EditorDataTextureDatabase.instance.RefreshDatabase();
        }

    }

    [CustomEditor(typeof(EditorUISettings))]
    public class EditorUISettingsEditor : UnityEditor.Editor
    {
        private static IEnumerable<Texture2D> iconTextures => EditorSpriteSheets.EditorUI.Icons.EditorSettings;
        private static Color accentColor => EditorColors.EditorUI.Amber;
        private static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.EditorUI.Amber;

        private VisualElement root { get; set; }
        private FluidComponentHeader componentHeader { get; set; }

        private FluidToggleCheckbox autoRefreshCheckbox { get; set; }
        private FluidButton saveButton { get; set; }

        private SerializedProperty propertyAutoRefresh { get; set; }

        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            return root;
        }

        private void OnDestroy()
        {
            componentHeader?.Recycle();
            autoRefreshCheckbox?.Recycle();
            saveButton?.Recycle();
        }

        private void FindProperties()
        {
            propertyAutoRefresh = serializedObject.FindProperty("AutoRefresh");
        }

        private void InitializeEditor()
        {
            FindProperties();

            root = DesignUtils.GetEditorRoot();

            componentHeader =
                FluidComponentHeader.Get()
                    .SetElementSize(ElementSize.Large)
                    .SetAccentColor(accentColor)
                    .SetComponentNameText("EditorUI Settings")
                    .SetIcon(iconTextures.ToList());

            autoRefreshCheckbox =
                FluidToggleCheckbox.Get()
                    .SetLabelText("Auto Refresh")
                    .BindToProperty(propertyAutoRefresh);

            saveButton =
                FluidButton.Get()
                    .SetLabelText("Save")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Save)
                    .SetButtonStyle(ButtonStyle.Contained)
                    .SetElementSize(ElementSize.Small)
                    .SetOnClick(() =>
                    {
                        EditorUtility.SetDirty(serializedObject.targetObject);
                        AssetDatabase.SaveAssets();
                    });
        }

        private void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddSpaceBlock()
                .AddChild
                (
                    DesignUtils.row
                        .SetStyleAlignItems(Align.Center)
                        .AddChild(autoRefreshCheckbox)
                        .AddChild(DesignUtils.flexibleSpace)
                        .AddChild(saveButton)
                        .AddSpaceBlock()
                );
        }
    }
}
