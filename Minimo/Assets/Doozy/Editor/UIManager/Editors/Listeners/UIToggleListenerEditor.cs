// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Input;
using Doozy.Runtime.UIManager.Listeners;
using Doozy.Runtime.UIManager.ScriptableObjects;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Editors.Listeners
{
    [CustomEditor(typeof(UIToggleListener), true)]
    public class UIToggleListenerEditor : UnityEditor.Editor
    {
        private static IEnumerable<Texture2D> componentIconTextures => EditorSpriteSheets.UIManager.Icons.UIToggleListener;

        private static Color accentColor => EditorColors.Default.ListenerComponent;
        private static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Default.ListenerComponent;

        private UIToggleListener castedTarget => (UIToggleListener)target;
        private IEnumerable<UIToggleListener> castedTargets => targets.Cast<UIToggleListener>();

        private VisualElement root { get; set; }

        private FluidComponentHeader componentHeader { get; set; }

        private PropertyField toggleIdPropertyField { get; set; }
        private EnumField commandEnumField { get; set; }
        private PropertyField callbackPropertyField { get; set; }
        private ObjectField multiplayerInfoObjectField { get; set; }
        
        private FluidField idField { get; set; }
        private FluidField callbackField { get; set; }
        private FluidField multiplayerInfoField { get; set; }
        
        private SerializedProperty propertyToggleId { get; set; }
        private SerializedProperty propertyCommand { get; set; }
        private SerializedProperty propertyCallback { get; set; }
        private SerializedProperty propertyMultiplayerInfo { get; set; }
        
        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            return root;
        }

        private void OnDestroy()
        {
            componentHeader?.Recycle();
            idField?.Recycle();
            callbackField?.Recycle();
            multiplayerInfoField?.Recycle();
        }

        private void FindProperties()
        {
            propertyToggleId = serializedObject.FindProperty("ToggleId");
            propertyCommand = serializedObject.FindProperty("Command");
            propertyCallback = serializedObject.FindProperty("Callback");
            propertyMultiplayerInfo = serializedObject.FindProperty("MultiplayerInfo");
        }

        private void InitializeEditor()
        {
            FindProperties();
            root = DesignUtils.GetEditorRoot();

            componentHeader =
                FluidComponentHeader.Get()
                    .SetElementSize(ElementSize.Large)
                    .SetAccentColor(accentColor)
                    .SetComponentNameText((ObjectNames.NicifyVariableName(nameof(UIToggleListener))))
                    .SetIcon(componentIconTextures.ToList())
                    .AddManualButton()
                    .AddApiButton("https://api.doozyui.com/api/Doozy.Runtime.UIManager.Listeners.UIToggleListener.html")
                    .AddYouTubeButton();

            toggleIdPropertyField =
                DesignUtils.NewPropertyField(propertyToggleId);

            commandEnumField =
                DesignUtils.NewEnumField(propertyCommand)
                    .SetStyleMarginTop(12)
                    .SetStyleWidth(60);

            callbackPropertyField =
                DesignUtils.NewPropertyField(propertyCallback);

            idField =
                FluidField.Get()
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .AddChild(toggleIdPropertyField)
                            .AddSpaceBlock()
                            .AddChild(commandEnumField)
                    );

            callbackField =
                FluidField.Get()
                    .AddFieldContent
                        (callbackPropertyField);

            root.schedule.Execute(() => callbackField.Q<FluidToggleSwitch>()?.Recycle());
            
            multiplayerInfoObjectField =
                DesignUtils.NewObjectField(propertyMultiplayerInfo, typeof(MultiplayerInfo))
                    .SetStyleFlexGrow(1);
            
            multiplayerInfoField =
                FluidField.Get()
                    .SetLabelText("Player Index")
                    .AddFieldContent(multiplayerInfoObjectField)
                    .SetStyleMarginTop(DesignUtils.k_Spacing2X)
                    .SetStyleDisplay(UIManagerInputSettings.instance.multiplayerMode ? DisplayStyle.Flex : DisplayStyle.None);
        }

        private void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddSpaceBlock()
                .AddChild(idField)
                .AddSpaceBlock()
                .AddChild(callbackField)
                .AddChild(multiplayerInfoField)
                ;
        }
    }
}
