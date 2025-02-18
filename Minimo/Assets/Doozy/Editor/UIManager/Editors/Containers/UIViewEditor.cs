// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.UIManager.Editors.Containers.Internal;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Orientation;
using Doozy.Runtime.UIManager.ScriptableObjects;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UIView = Doozy.Runtime.UIManager.Containers.UIView;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Doozy.Editor.UIManager.Editors.Containers
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UIView), true)]
    public class UIViewEditor : BaseUIContainerEditor
    {
        public UIView castedTarget => (UIView)target;
        public IEnumerable<UIView> castedTargets => targets.Cast<UIView>();

        private FluidField idFluidField { get; set; }
        private FluidField targetOrientationFluidField { get; set; }

        protected SerializedProperty propertyId { get; set; }
        protected SerializedProperty propertyTargetOrientation { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            idFluidField?.Recycle();
            targetOrientationFluidField?.Recycle();
        }

        protected override void FindProperties()
        {
            base.FindProperties();
            propertyId = serializedObject.FindProperty(nameof(UIView.Id));
            propertyTargetOrientation = serializedObject.FindProperty("TargetOrientation");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText("UIView")
                .SetIcon(EditorSpriteSheets.UIManager.Icons.UIView)
                .AddManualButton()
                .AddApiButton("https://api.doozyui.com/api/Doozy.Runtime.UIManager.Containers.UIView.html")
                .AddYouTubeButton();

            idFluidField =
                FluidField.Get()
                    .AddFieldContent(DesignUtils.NewPropertyField(propertyId));

            EnumField targetOrientationEnumField =
                DesignUtils.NewEnumField(propertyTargetOrientation)
                    .SetStyleFlexGrow(1);

            targetOrientationFluidField =
                FluidField.Get()
                    .SetStyleMarginTop(DesignUtils.k_Spacing)
                    .SetLabelText("Target Orientation")
                    .SetTooltip("Target orientation for this view")
                    .AddFieldContent(targetOrientationEnumField);

            void UpdateOrientationIcon(TargetOrientation orientation)
            {
                switch (orientation)
                {
                    case TargetOrientation.Any:
                        targetOrientationFluidField.SetIcon(EditorSpriteSheets.EditorUI.Icons.Orientation);
                        break;
                    case TargetOrientation.Portrait:
                        targetOrientationFluidField.SetIcon(EditorSpriteSheets.EditorUI.Icons.Portrait);
                        break;
                    case TargetOrientation.Landscape:
                        targetOrientationFluidField.SetIcon(EditorSpriteSheets.EditorUI.Icons.Landscape);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                targetOrientationFluidField.iconReaction.Play();
            }

            UpdateOrientationIcon((TargetOrientation)propertyTargetOrientation.enumValueIndex);

            targetOrientationEnumField.RegisterValueChangedCallback(evt =>
            {
                if (evt?.newValue == null) return;
                UpdateOrientationIcon((TargetOrientation)evt.newValue);
            });
            
            targetOrientationFluidField.SetStyleDisplay(UIManagerSettings.instance.UseOrientationDetection ? DisplayStyle.Flex : DisplayStyle.None);
        }

        protected override VisualElement Toolbar()
        {
            toolbarContainer
                .AddChild(settingsTab)
                .AddSpaceBlock()
                .AddChild(callbacksTab)
                .AddSpaceBlock()
                .AddChild(progressorsTab)
                .AddSpaceBlock()
                .AddChild(DesignUtils.flexibleSpace)
                .AddSpaceBlock(2);

            if (castedTarget != null)
            {
                toolbarContainer
                    .AddChild(DesignUtils.SystemButton_RenameComponent
                        (
                            castedTarget.gameObject,
                            () => $"View - {castedTarget.Id.Name}"
                        )
                    )
                    .AddSpaceBlock();
            }

            toolbarContainer
                .AddChild
                (
                    DesignUtils.SystemButton_SortComponents
                    (
                        castedContainer.gameObject,
                        nameof(RectTransform),
                        nameof(UIView)
                    )
                );

            return toolbarContainer;
        }

        protected override void Compose()
        {
            root
                .AddChild(reactionControls)
                .AddChild(componentHeader)
                .AddChild(Toolbar())
                .AddSpaceBlock(2)
                .AddChild(Content())
                .AddSpaceBlock(2)
                .AddChild(idFluidField)
                .AddSpaceBlock()
                .AddChild(targetOrientationFluidField)
                .AddEndOfLineSpace();
        }
    }
}
