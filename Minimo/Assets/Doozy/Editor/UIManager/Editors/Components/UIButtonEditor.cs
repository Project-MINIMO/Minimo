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
using Doozy.Editor.UIManager.Editors.Components.Internal;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Animators;
using Doozy.Runtime.UIManager.Components;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.Editors.Components
{
    [CustomEditor(typeof(UIButton), true)]
    [CanEditMultipleObjects]
    public sealed class UIButtonEditor : UISelectableBaseEditor
    {
        public override Color accentColor => EditorColors.Default.UIComponent;
        public override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Default.UIComponent;

        public UIButton castedTarget => (UIButton)target;
        public IEnumerable<UIButton> castedTargets => targets.Cast<UIButton>();

        private FluidField idField { get; set; }

        private SerializedProperty propertyId { get; set; }
        private SerializedProperty propertyCooldown { get; set; }
        private SerializedProperty propertyDisableWhenInCooldown { get; set; }

        protected override void SearchForAnimators()
        {
            selectableAnimators ??= new List<BaseUISelectableAnimator>();
            selectableAnimators.Clear();

            //check if prefab was selected
            if (castedTargets.Any(s => s.gameObject.scene.name == null))
            {
                selectableAnimators.AddRange(castedSelectable.GetComponentsInChildren<BaseUISelectableAnimator>());
                return;
            }

            //not prefab
            #if UNITY_2023_1_OR_NEWER
            selectableAnimators.AddRange(FindObjectsByType<BaseUISelectableAnimator>(FindObjectsSortMode.None));
            #else
            selectableAnimators.AddRange(FindObjectsOfType<BaseUISelectableAnimator>());
            #endif
        }

        protected override void FindProperties()
        {
            base.FindProperties();

            propertyId = serializedObject.FindProperty(nameof(UIButton.Id));
            propertyCooldown = serializedObject.FindProperty(nameof(UIButton.Cooldown));
            propertyDisableWhenInCooldown = serializedObject.FindProperty(nameof(UIButton.DisableWhenInCooldown));
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetAccentColor(accentColor)
                .SetComponentNameText("UIButton")
                .SetIcon(EditorSpriteSheets.UIManager.Icons.UIButton)
                .AddManualButton()
                .AddApiButton("https://api.doozyui.com/api/Doozy.Runtime.UIManager.Components.UIButton.html")
                .AddYouTubeButton();

            idField =
                FluidField.Get()
                    .AddFieldContent(DesignUtils.NewPropertyField(propertyId));
        }

        protected override void InitializeSettings()
        {
            settingsAnimatedContainer = new FluidAnimatedContainer("Settings", true).Hide(false);
            settingsTab =
                GetTab("Settings")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Settings)
                    .ButtonSetOnValueChanged(evt => settingsAnimatedContainer.Toggle(evt.newValue, evt.animateChange));

            settingsAnimatedContainer.SetOnShowCallback(() =>
            {
                #region Interactable

                FluidToggleCheckbox interactableCheckbox = FluidToggleCheckbox.Get()
                    .SetLabelText("Interactable")
                    .SetTooltip("Can the Selectable be interacted with?")
                    .BindToProperty(propertyInteractable);

                #endregion

                #region Deselect after press

                FluidToggleCheckbox deselectAfterPressCheckbox = FluidToggleCheckbox.Get()
                    .SetLabelText("Deselect after Press")
                    .BindToProperty(propertyDeselectAfterPress);

                #endregion

                #region Cooldown

                FluidToggleCheckbox disableWhenInCooldownCheckbox =
                    FluidToggleCheckbox.Get()
                        .SetLabelText("Disable when in Cooldown")
                        .SetTooltip("Set the button's interactable state to false during the cooldown time.")
                        .SetToggleAccentColor(selectableAccentColor)
                        .BindToProperty(propertyDisableWhenInCooldown)
                        .SetStyleAlignSelf(Align.FlexEnd);

                FluidField cooldownFluidField =
                    FluidField.Get("Cooldown")
                        .SetTooltip
                        (
                            "Cooldown time in seconds before the button can be clicked again." +
                            "\n\n" +
                            "This is useful when you want to prevent the button from being clicked multiple times in a short period of time." +
                            "\n\n" +
                            "Set to 0 to disable the cooldown and make the button clickable every time."
                        )
                        .AddFieldContent
                        (
                            DesignUtils.row
                                .SetStyleAlignItems(Align.Center)
                                .AddChild(DesignUtils.NewFloatField(propertyCooldown).SetStyleWidth(40))
                                .AddSpaceBlock()
                                .AddChild(DesignUtils.fieldLabel.SetText("seconds"))
                                .AddSpaceBlock(2)
                                .AddChild(disableWhenInCooldownCheckbox)
                        );

                #endregion

                settingsAnimatedContainer
                    .AddContent
                    (
                        DesignUtils.row
                            .AddChild(interactableCheckbox)
                            .AddSpaceBlock()
                            .AddChild(deselectAfterPressCheckbox)
                    )
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(cooldownFluidField)
                    .Bind(serializedObject);
            });

        }

        protected override VisualElement Toolbar()
        {
            toolbarContainer
                .AddChild(settingsTab)
                .AddSpaceBlock(2)
                .AddChild(statesTab)
                .AddSpaceBlock(2)
                .AddChild(behavioursTab)
                .AddSpaceBlock(2)
                .AddChild(navigationTab)
                .AddSpaceBlock()
                .AddChild(DesignUtils.flexibleSpace)
                .AddSpaceBlock(2);

            if (castedTarget != null)
            {
                toolbarContainer
                    .AddChild
                    (
                        DesignUtils.SystemButton_RenameComponent
                        (
                            castedTarget.gameObject,
                            () => $"Button - {castedTarget.Id.Name}"
                        )
                    )
                    .AddSpaceBlock();
            }

            toolbarContainer
                .AddChild
                (
                    DesignUtils.SystemButton_SortComponents
                    (
                        ((UISelectable)target).gameObject,
                        nameof(RectTransform),
                        nameof(UIButton)
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
                .AddChild(idField)
                .AddEndOfLineSpace();
        }
    }
}
