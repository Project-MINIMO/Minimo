// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.UIElements;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Input;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Editors.Input
{
    [CustomEditor(typeof(MultiplayerInfo), true)]
    public class MultiplayerInfoEditor : UnityEditor.Editor
    {
        private static IEnumerable<Texture2D> multiplayerInfoIconTextures => EditorSpriteSheets.UIManager.Icons.MultiplayerInfo;

        private static Color accentColor => EditorColors.Default.InputComponent;
        private static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Default.InputComponent;

        private MultiplayerInfo castedTarget => (MultiplayerInfo)target;
        private IEnumerable<MultiplayerInfo> castedTargets => targets.Cast<MultiplayerInfo>();

        private VisualElement root { get; set; }

        #if INPUT_SYSTEM_PACKAGE
        private PropertyField playerInputPropertyField { get; set; }
        private FluidField playerInputField { get; set; }
        private SerializedProperty propertyPlayerInput { get; set; }
        #endif

        private IntegerField customPlayerIndexIntegerField { get; set; }

        private FluidComponentHeader componentHeader { get; set; }
        private FluidToggleSwitch autoUpdateSwitch { get; set; }

        private FluidField customPlayerIndexField { get; set; }
        private FluidToggleSwitch useCustomPlayerIndexSwitch { get; set; }

        private SerializedProperty propertyAutoUpdate { get; set; }
        private SerializedProperty propertyCustomPlayerIndex { get; set; }
        private SerializedProperty propertyUseCustomPlayerIndex { get; set; }

        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            return root;
        }

        private void OnDestroy()
        {
            #if INPUT_SYSTEM_PACKAGE
            playerInputField?.Recycle();
            #endif
            autoUpdateSwitch?.Recycle();
            componentHeader?.Recycle();
            customPlayerIndexField?.Recycle();
            useCustomPlayerIndexSwitch?.Recycle();
        }

        private void FindProperties()
        {
            #if INPUT_SYSTEM_PACKAGE
            propertyPlayerInput = serializedObject.FindProperty("PlayerInput");
            #endif
            propertyAutoUpdate = serializedObject.FindProperty("AutoUpdate");
            propertyCustomPlayerIndex = serializedObject.FindProperty("CustomPlayerIndex");
            propertyUseCustomPlayerIndex = serializedObject.FindProperty("UseCustomPlayerIndex");
        }

        private void InitializeEditor()
        {
            FindProperties();
            root = DesignUtils.GetEditorRoot();

            componentHeader =
                FluidComponentHeader.Get()
                    .SetElementSize(ElementSize.Large)
                    .SetAccentColor(accentColor)
                    .SetComponentNameText((ObjectNames.NicifyVariableName(nameof(MultiplayerInfo))))
                    .SetIcon(multiplayerInfoIconTextures.ToList())
                    .AddManualButton()
                    .AddApiButton("https://api.doozyui.com/api/Doozy.Runtime.UIManager.Input.MultiplayerInfo.html")
                    .AddYouTubeButton();

            autoUpdateSwitch =
                FluidToggleSwitch.Get()
                    .SetLabelText("AutoUpdate")
                    .SetStyleMarginLeft(40)
                    .BindToProperty(propertyAutoUpdate);

            #if INPUT_SYSTEM_PACKAGE
            playerInputPropertyField =
                DesignUtils.NewPropertyField(propertyPlayerInput)
                    .TryToHideLabel()
                    .SetStyleFlexGrow(1);
            #endif

            customPlayerIndexIntegerField =
                DesignUtils.NewIntegerField(propertyCustomPlayerIndex)
                    .SetStyleWidth(80)
                    .SetStyleFlexShrink(1);

            customPlayerIndexIntegerField.SetEnabled(propertyUseCustomPlayerIndex.boolValue);

            useCustomPlayerIndexSwitch =
                FluidToggleSwitch.Get()
                    .BindToProperty(propertyUseCustomPlayerIndex);

            useCustomPlayerIndexSwitch.SetOnValueChanged(evt => customPlayerIndexIntegerField.SetEnabled(evt.newValue));

            #if INPUT_SYSTEM_PACKAGE
            playerInputField =
                FluidField.Get()
                    .SetLabelText("Player Input")
                    .AddFieldContent(playerInputPropertyField);
            #endif

            customPlayerIndexField =
                FluidField.Get()
                    .SetStyleFlexGrow(0)
                    .SetStyleFlexShrink(1)
                    .SetLabelText("Custom Player Index")
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .AddChild(useCustomPlayerIndexSwitch)
                            .AddSpaceBlock()
                            .AddChild(customPlayerIndexIntegerField)
                    );
        }

        private void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddChild(autoUpdateSwitch)
                .AddSpaceBlock()
                .AddChild
                (
                    DesignUtils.row.SetStyleMarginLeft(40)
                #if INPUT_SYSTEM_PACKAGE
                        .AddChild(playerInputField)
                        .AddSpaceBlock()
                #endif
                        .AddChild(customPlayerIndexField)
                )
                ;
        }
    }
}
