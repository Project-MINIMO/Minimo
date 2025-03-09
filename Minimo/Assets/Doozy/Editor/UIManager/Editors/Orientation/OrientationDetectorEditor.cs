// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Orientation;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Editors.Orientation
{
    [CustomEditor(typeof(OrientationDetector), true)]
    public class OrientationDetectorEditor : UnityEditor.Editor
    {
        private OrientationDetector castedTarget => (OrientationDetector) target;
        
        private VisualElement root { get; set; }
        private FluidComponentHeader componentHeader { get; set; }

        private VisualElement onOrientationChangedElement { get; set; }
        private FluidField currentOrientationFluidField { get; set; }
        
        private SerializedProperty propertyOnOrientationChanged { get; set; }
        private SerializedProperty propertyOnAnyOrientation { get; set; }
        private SerializedProperty propertyOnPortraitOrientation { get; set; }
        private SerializedProperty propertyOnLandscapeOrientation { get; set; }
        private SerializedProperty propertyCurrentOrientation { get; set; }
        
        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            return root;
        }

        private void OnDestroy()
        {
            componentHeader?.Recycle();
            currentOrientationFluidField?.Recycle();
        }

        private void FindProperties()
        {
            propertyOnOrientationChanged = serializedObject.FindProperty("OnOrientationChanged");
            propertyOnAnyOrientation = serializedObject.FindProperty("OnAnyOrientation");
            propertyOnPortraitOrientation = serializedObject.FindProperty("OnPortraitOrientation");
            propertyOnLandscapeOrientation = serializedObject.FindProperty("OnLandscapeOrientation");
            propertyCurrentOrientation = serializedObject.FindProperty("CurrentOrientation");
        }

        private void InitializeEditor()
        {
            FindProperties();

            root = DesignUtils.GetEditorRoot();

            componentHeader =
                FluidComponentHeader.Get()
                    .SetElementSize(ElementSize.Large)
                    .SetAccentColor(EditorColors.Default.ListenerComponent)
                    .SetIcon(EditorSpriteSheets.UIManager.Icons.OrientationDetector)
                    .SetComponentNameText("Orientation Detector")
                    .AddManualButton()
                    .AddApiButton()
                    .AddYouTubeButton();

            EnumField currentOrientationEnum = 
                DesignUtils.NewEnumField(propertyCurrentOrientation)
                    .SetStyleFlexGrow(1)
                    .DisableElement();

            currentOrientationFluidField =
                FluidField.Get()
                    .SetElementSize(ElementSize.Small)
                    .SetLabelText("Current Orientation")
                    .AddFieldContent(currentOrientationEnum);

            onOrientationChangedElement = 
                DesignUtils.UnityEventField("Callback triggered when the device orientation changed", propertyOnOrientationChanged);
        }

        private void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddSpaceBlock()
                .AddChild(currentOrientationFluidField)
                .AddSpaceBlock(2)
                .AddChild(onOrientationChangedElement)
                .AddSpaceBlock(2)
                .AddChild(DesignUtils.NewPropertyField(propertyOnAnyOrientation))
                .AddSpaceBlock(2)
                .AddChild(DesignUtils.NewPropertyField(propertyOnPortraitOrientation))
                .AddSpaceBlock(2)
                .AddChild(DesignUtils.NewPropertyField(propertyOnLandscapeOrientation))
                .AddEndOfLineSpace()
                ;
        }

    }
}
