// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Interfaces;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.ScriptableObjects;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
// ReSharper disable RedundantUsingDirective
using UnityEngine.UIElements; //leave this here to avoid errors in newer versions of Unity (THANKS UNITY)!!! - they moved IntegerField and FloatField to the UnityEngine.UIElements namespace
// ReSharper restore RedundantUsingDirective

namespace Doozy.Editor.UIManager.Layouts.Settings
{
    public sealed class UISettingsWindowLayout : FluidWindowLayout, IDashboardSettingsWindowLayout
    {
        public int order => 0;

        public override string layoutName => "UI Settings";
        public override List<Texture2D> animatedIconTextures => EditorSpriteSheets.EditorUI.Icons.DoozyUI;
        public override Color accentColor => EditorColors.Default.SettingsComponent;
        public override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Default.SettingsComponent;

        private SerializedObject serializedObject { get; set; }
        private SerializedProperty propertyUseOrientationDetection { get; set; }

        private FluidToggleSwitch useOrientationDetectionSwitch { get; set; }
        private FluidField useOrientationDetectionFluidField { get; set; }

        public override void OnDestroy()
        {
            base.OnDestroy();
            useOrientationDetectionSwitch?.Recycle();
            useOrientationDetectionFluidField?.Recycle();
        }

        public UISettingsWindowLayout()
        {
            AddHeader("UI Settings", "Global Settings", animatedIconTextures);
            sideMenu.Dispose(); //remove side menu
            FindProperties();
            Initialize();
            Compose();
        }

        private void FindProperties()
        {
            serializedObject = new SerializedObject(UIManagerSettings.instance);
            propertyUseOrientationDetection = serializedObject.FindProperty("UseOrientationDetection");
        }

        private void Initialize()
        {
            useOrientationDetectionSwitch =
                FluidToggleSwitch.Get()
                    .SetToggleAccentColor(selectableAccentColor)
                    .BindToProperty(propertyUseOrientationDetection);

            useOrientationDetectionFluidField =
                FluidField.Get()
                    .SetStyleFlexGrow(0)
                    .SetElementSize(ElementSize.Normal)
                    .SetLabelText("Use Orientation Detection")
                    .SetTooltip("Add orientation options to all relevant UI elements")
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .AddFlexibleSpace()
                            .AddChild(useOrientationDetectionSwitch)
                    );
            
            
        }

        private void Compose()
        {
            content
                .AddChild(useOrientationDetectionFluidField)
                .AddFlexibleSpace()
                ;
            
            content.Bind(serializedObject);
        }
    }
}
