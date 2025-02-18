// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.UIManager.Editors.Components.Internal;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager;
using Doozy.Runtime.UIManager.Animators;
using Doozy.Runtime.UIManager.Components;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.Editors.Components
{
    [CustomEditor(typeof(UIToggleGroup), true)]
    [CanEditMultipleObjects]
    public class UIToggleGroupEditor : UISelectableBaseEditor
    {
        public override Color accentColor => EditorColors.Default.UIComponent;
        public override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Default.UIComponent;

        public UIToggleGroup castedTarget => (UIToggleGroup)target;
        public IEnumerable<UIToggleGroup> castedTargets => targets.Cast<UIToggleGroup>();

        private FluidTab callbacksTab { get; set; }
        private FluidAnimatedContainer callbacksAnimatedContainer { get; set; }
        private FluidField idField { get; set; }

        private SerializedProperty propertyAutoSort { get; set; }
        private SerializedProperty propertyBehaviours { get; set; }
        private SerializedProperty propertyCooldown { get; set; }
        private SerializedProperty propertyDisableWhenInCooldown { get; set; }
        private SerializedProperty propertyFirstToggle { get; set; }
        private SerializedProperty propertyHasMixedValues { get; set; }
        private SerializedProperty propertyId { get; set; }
        private SerializedProperty propertyIsOn { get; set; }
        private SerializedProperty propertyIsLocked { get; set; }
        private SerializedProperty propertyMode { get; set; }
        private SerializedProperty propertyOnInstantToggleOffCallback { get; set; }
        private SerializedProperty propertyOnInstantToggleOnCallback { get; set; }
        private SerializedProperty propertyOnToggleAddedCallback { get; set; }
        private SerializedProperty propertyOnToggleGroupMixedValuesCallback { get; set; }
        private SerializedProperty propertyOnToggleOffCallback { get; set; }
        private SerializedProperty propertyOnToggleOnCallback { get; set; }
        private SerializedProperty propertyOnToggleRemovedCallback { get; set; }
        private SerializedProperty propertyOnToggleTriggeredCallback { get; set; }
        private SerializedProperty propertyOnValueChangedCallback { get; set; }
        private SerializedProperty propertyOverrideInteractabilityForToggles { get; set; }
        private SerializedProperty propertyToggleGroup { get; set; }
        private SerializedProperty propertyToggleGroupValue { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            callbacksTab?.Dispose();
            callbacksAnimatedContainer?.Dispose();
            idField?.Recycle();
        }

        protected override void SearchForAnimators()
        {
            selectableAnimators ??= new List<BaseUISelectableAnimator>();
            selectableAnimators.Clear();

            //check if prefab was selected -> get the animators from it alone and return
            if (castedTargets.Any(s => s.gameObject.scene.name == null))
            {
                selectableAnimators.AddRange(castedSelectable.GetComponentsInChildren<BaseUISelectableAnimator>());
                return;
            }

            //a prefab was not selected -> search for all the animators in the scene (not efficient, but there is no other way)
            #if UNITY_2023_1_OR_NEWER
            selectableAnimators.AddRange(FindObjectsByType<BaseUISelectableAnimator>(FindObjectsSortMode.None));
            #else
            selectableAnimators.AddRange(FindObjectsOfType<BaseUISelectableAnimator>());
            #endif
        }

        protected override void FindProperties()
        {
            base.FindProperties();

            propertyAutoSort = serializedObject.FindProperty("AutoSort");
            propertyBehaviours = serializedObject.FindProperty("Behaviours");
            propertyCooldown = serializedObject.FindProperty(nameof(UIToggleGroup.Cooldown));
            propertyDisableWhenInCooldown = serializedObject.FindProperty(nameof(UIToggleGroup.DisableWhenInCooldown));
            propertyFirstToggle = serializedObject.FindProperty(nameof(UIToggleGroup.FirstToggle));
            propertyHasMixedValues = serializedObject.FindProperty("HasMixedValues");
            propertyId = serializedObject.FindProperty(nameof(UIToggleGroup.Id));
            propertyIsOn = serializedObject.FindProperty("IsOn");
            propertyIsLocked = serializedObject.FindProperty("IsLocked");
            propertyMode = serializedObject.FindProperty("Mode");
            propertyOnInstantToggleOffCallback = serializedObject.FindProperty(nameof(UIToggleGroup.OnInstantToggleOffCallback));
            propertyOnInstantToggleOnCallback = serializedObject.FindProperty(nameof(UIToggleGroup.OnInstantToggleOnCallback));
            propertyOnToggleAddedCallback = serializedObject.FindProperty(nameof(UIToggleGroup.OnToggleAddedCallback));
            propertyOnToggleGroupMixedValuesCallback = serializedObject.FindProperty(nameof(UIToggleGroup.OnToggleGroupMixedValuesCallback));
            propertyOnToggleOffCallback = serializedObject.FindProperty(nameof(UIToggleGroup.OnToggleOffCallback));
            propertyOnToggleOnCallback = serializedObject.FindProperty(nameof(UIToggleGroup.OnToggleOnCallback));
            propertyOnToggleRemovedCallback = serializedObject.FindProperty(nameof(UIToggleGroup.OnToggleRemovedCallback));
            propertyOnToggleTriggeredCallback = serializedObject.FindProperty(nameof(UIToggleGroup.OnToggleTriggeredCallback));
            propertyOnValueChangedCallback = serializedObject.FindProperty(nameof(UIToggleGroup.OnValueChangedCallback));
            propertyOverrideInteractabilityForToggles = serializedObject.FindProperty("OverrideInteractabilityForToggles");
            propertyToggleGroup = serializedObject.FindProperty("ToggleGroup");
            propertyToggleGroupValue = serializedObject.FindProperty("ToggleGroupValue");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetAccentColor(accentColor)
                .SetComponentNameText("UIToggle Group")
                .SetIcon(EditorSpriteSheets.UIManager.Icons.UIToggleGroup)
                .AddManualButton()
                .AddApiButton("https://api.doozyui.com/api/Doozy.Runtime.UIManager.Components.UIToggleGroup.html")
                .AddYouTubeButton();

            idField = FluidField.Get().AddFieldContent(DesignUtils.NewPropertyField(propertyId));

            InitializeCallbacks();

            //refresh tabs enabled indicator
            root.schedule.Execute(() =>
            {
                void UpdateIndicator(FluidTab tab, bool toggleOn, bool animateChange)
                {
                    if (tab == null) return;
                    if (tab.indicator.isOn != toggleOn)
                        tab.indicator.Toggle(toggleOn, animateChange);
                }

                bool HasCallbacks()
                {
                    if (castedTarget == null)
                        return false;

                    return castedTarget.OnToggleOnCallback.hasCallbacks ||
                        castedTarget.OnInstantToggleOnCallback.hasCallbacks ||
                        castedTarget.OnToggleOffCallback.hasCallbacks ||
                        castedTarget.OnInstantToggleOffCallback.hasCallbacks ||
                        castedTarget.OnValueChangedCallback?.GetPersistentEventCount() > 0 ||
                        castedTarget.OnToggleGroupMixedValuesCallback.hasCallbacks ||
                        castedTarget.OnToggleAddedCallback?.GetPersistentEventCount() > 0 ||
                        castedTarget.OnToggleRemovedCallback?.GetPersistentEventCount() > 0 ||
                        castedTarget.OnToggleTriggeredCallback?.GetPersistentEventCount() > 0;
                }

                //initial indicators state update (no animation)
                UpdateIndicator(callbacksTab, HasCallbacks(), false);

                //subsequent indicators state update (animated)
                root.schedule.Execute(() =>
                {
                    UpdateIndicator(callbacksTab, HasCallbacks(), true);

                }).Every(200);
            });
        }

        protected override void InitializeSettings()
        {
            base.InitializeSettings();

            settingsAnimatedContainer.SetOnShowCallback(() =>
            {
                #region Interactable

                FluidToggleCheckbox interactableCheckbox = FluidToggleCheckbox.Get()
                    .SetLabelText("Interactable")
                    .SetTooltip("Can the Selectable be interacted with?")
                    .BindToProperty(propertyInteractable);

                #endregion

                #region Deselect after Press

                FluidToggleCheckbox deselectAfterPressCheckbox = FluidToggleCheckbox.Get()
                    .SetLabelText("Deselect after Press")
                    .BindToProperty(propertyDeselectAfterPress);

                #endregion

                #region Is On

                FluidToggleCheckbox isOnCheckbox =
                    FluidToggleCheckbox.Get()
                        .SetLabelText("Is On")
                        .BindToProperty(propertyIsOn)
                        .SetOnClick(() =>
                        {
                            if (Application.isPlaying)
                            {
                                foreach (var t in castedTargets)
                                    t.isOn = !castedTarget.isOn;
                                return;
                            }
                            HeartbeatCheck();
                            foreach (var a in selectableAnimators.RemoveNulls())
                            {
                                switch (a.ToggleCommand)
                                {
                                    case CommandToggle.On when !castedTarget.isOn:
                                    case CommandToggle.Off when castedTarget.isOn:
                                        continue;
                                    default:
                                        a.Play(castedSelectable.currentUISelectionState);
                                        break;
                                }
                            }
                        });

                #endregion

                #region Is Locked

                FluidToggleCheckbox isLockedCheckbox =
                    FluidToggleCheckbox.Get()
                        .SetLabelText("Is Locked")
                        .SetTooltip("Locks the toggle group so its isOn value cannot be changed.")
                        .BindToProperty(propertyIsLocked)
                        .SetOnClick(() =>
                        {
                            foreach (var t in castedTargets)
                                t.isLocked = !castedTarget.isLocked;

                            isOnCheckbox.SetEnabled(!castedTarget.isLocked);

                            if (Application.isPlaying) return;
                            HeartbeatCheck();
                            foreach (var a in selectableAnimators.RemoveNulls())
                            {
                                switch (a.ToggleCommand)
                                {
                                    case CommandToggle.On when !castedTarget.isOn:
                                    case CommandToggle.Off when castedTarget.isOn:
                                        continue;
                                    default:
                                        a.Play(castedSelectable.currentUISelectionState);
                                        break;
                                }
                            }
                        });

                isOnCheckbox.SetEnabled(!castedTarget.isLocked);

                #endregion

                #region Has Mixed Values

                FluidToggleCheckbox hasMixedValuesToggle =
                    FluidToggleCheckbox.Get()
                        .SetLabelText("Has Mixed Values")
                        .BindToProperty(propertyHasMixedValues);

                hasMixedValuesToggle.SetEnabled(false);

                EnumField toggleGroupValueEnumField =
                    DesignUtils.NewEnumField(propertyToggleGroupValue)
                        .SetStyleWidth(120);

                toggleGroupValueEnumField.SetEnabled(false);

                #endregion

                #region Override interactability for connected UIToggles

                FluidToggleCheckbox overrideInteractabilityForTogglesToggle =
                    FluidToggleCheckbox.Get()
                        .SetLabelText("Override interactability for connected UIToggles")
                        .BindToProperty(propertyOverrideInteractabilityForToggles)
                        .SetTooltip("Override and control the interactable state for all the connected UIToggles");

                #endregion

                #region Cooldown

                FluidToggleCheckbox disableWhenInCooldownCheckbox =
                    FluidToggleCheckbox.Get()
                        .SetLabelText("Disable when in Cooldown")
                        .SetTooltip("Set the toggle's interactable state to false during the cooldown time.")
                        .SetToggleAccentColor(selectableAccentColor)
                        .BindToProperty(propertyDisableWhenInCooldown)
                        .SetStyleAlignSelf(Align.FlexEnd);

                FluidField cooldownFluidField =
                    FluidField.Get("Cooldown")
                        .SetTooltip
                        (
                            "Cooldown time in seconds before the toggle can be clicked again." +
                            "\n\n" +
                            "This is useful when you want to prevent the toggle from being clicked multiple times in a short period of time." +
                            "\n\n" +
                            "Set to 0 to disable the cooldown and make the toggle clickable every time."
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

                #region Control Mode

                EnumField modeEnumField =
                    DesignUtils.NewEnumField(propertyMode)
                        .SetStyleFlexGrow(1);

                void UpdateToggleModeTooltip(UIToggleGroup.ControlMode value)
                {
                    switch (value)
                    {
                        case UIToggleGroup.ControlMode.Passive:
                            modeEnumField.SetTooltip("Toggle values are not enforced in any way (allows for all toggles to be OFF)");
                            break;
                        case UIToggleGroup.ControlMode.OneToggleOn:
                            modeEnumField.SetTooltip("Only one Toggle can be ON at any given time (allows for all toggles to be OFF)");
                            break;
                        case UIToggleGroup.ControlMode.OneToggleOnEnforced:
                            modeEnumField.SetTooltip("Only one Toggle can be ON at any given time (one Toggle will be forced ON at all times)");
                            break;
                        case UIToggleGroup.ControlMode.AnyToggleOnEnforced:
                            modeEnumField.SetTooltip("At least one Toggle needs to be ON at any given time (one Toggle will be forced ON at all times)");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(value), value, null);
                    }
                }

                UpdateToggleModeTooltip((UIToggleGroup.ControlMode)propertyMode.enumValueIndex);
                modeEnumField.RegisterValueChangedCallback(evt =>
                {
                    if (evt?.newValue == null) return;
                    UpdateToggleModeTooltip((UIToggleGroup.ControlMode)evt.newValue);
                });

                FluidField toggleModeField =
                    FluidField.Get()
                        .SetLabelText("Control Mode")
                        .SetIcon(EditorSpriteSheets.UIManager.Icons.UIToggleGroup)
                        .AddFieldContent(modeEnumField);

                #endregion

                #region Auto Sort

                EnumField autoSortEnumField =
                    DesignUtils.NewEnumField(propertyAutoSort)
                        .SetStyleFlexGrow(1);

                void UpdateAutoSortTooltip(UIToggleGroup.SortMode value)
                {
                    switch (value)
                    {
                        case UIToggleGroup.SortMode.Disabled:
                            autoSortEnumField.SetTooltip("Auto sort is disabled");
                            break;
                        case UIToggleGroup.SortMode.Hierarchy:
                            autoSortEnumField.SetTooltip("Auto sort by sibling index (the order toggles appear in the Hierarchy)");
                            break;
                        case UIToggleGroup.SortMode.GameObjectName:
                            autoSortEnumField.SetTooltip("Auto sort by Toggle's GameObject name");
                            break;
                        case UIToggleGroup.SortMode.ToggleName:
                            autoSortEnumField.SetTooltip("Auto sort by Toggle Id Name (ignores category)");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(value), value, null);
                    }
                }

                UpdateAutoSortTooltip((UIToggleGroup.SortMode)propertyAutoSort.enumValueIndex);
                autoSortEnumField.RegisterValueChangedCallback(evt =>
                {
                    if (evt?.newValue == null) return;
                    UpdateAutoSortTooltip((UIToggleGroup.SortMode)evt.newValue);
                });

                FluidField autoSortField =
                    FluidField.Get()
                        .SetLabelText("Auto Sort")
                        .SetIcon(EditorSpriteSheets.EditorUI.Icons.SortAz)
                        .AddFieldContent(autoSortEnumField);

                #endregion

                #region First Toggle

                ObjectField firstToggleObjectField =
                    DesignUtils.NewObjectField(propertyFirstToggle, typeof(UIToggle))
                        .SetTooltip("The first toggle in the group")
                        .SetStyleFlexGrow(1);

                FluidField firstToggleField =
                    FluidField.Get()
                        .SetLabelText("First Toggle")
                        .SetIcon(EditorSpriteSheets.UIManager.Icons.UIToggle)
                        .AddFieldContent(firstToggleObjectField);

                #endregion

                #region Toggle Group

                ObjectField toggleGroupObjectField =
                    DesignUtils.NewObjectField(propertyToggleGroup, typeof(UIToggleGroup))
                        .SetTooltip("The toggle group this toggle belongs to")
                        .SetStyleFlexGrow(1);

                FluidField toggleGroupField =
                    FluidField.Get()
                        .SetLabelText("Toggle Group")
                        .SetIcon(EditorSpriteSheets.UIManager.Icons.UIToggleGroup)
                        .AddFieldContent(toggleGroupObjectField);

                #endregion

                settingsAnimatedContainer
                    .AddContent
                    (
                        DesignUtils.row
                            .AddChild(interactableCheckbox)
                            .AddSpaceBlock()
                            .AddChild(deselectAfterPressCheckbox)
                    )
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent
                    (
                        DesignUtils.row
                            .AddChild(isOnCheckbox)
                            .AddSpaceBlock()
                            .AddChild(isLockedCheckbox)
                            .AddSpaceBlock()
                            .AddChild(hasMixedValuesToggle)
                            .AddSpaceBlock()
                            .AddChild(toggleGroupValueEnumField)
                    )
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent(overrideInteractabilityForTogglesToggle)
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent(cooldownFluidField)
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent
                    (
                        DesignUtils.row
                            .AddChild(toggleModeField)
                            .AddSpaceBlock()
                            .AddChild(autoSortField)
                    )
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent(firstToggleField)
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent(toggleGroupField)
                    .Bind(serializedObject);
            });
        }

        private void InitializeCallbacks()
        {
            callbacksAnimatedContainer = new FluidAnimatedContainer("Callbacks", true).Hide(false);
            callbacksTab =
                GetTab("Callbacks")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.UnityEvent)
                    .IndicatorSetEnabledColor(DesignUtils.callbacksColor)
                    .ButtonSetAccentColor(DesignUtils.callbackSelectableColor)
                    .ButtonSetOnValueChanged(evt => callbacksAnimatedContainer.Toggle(evt.newValue, evt.animateChange));

            callbacksAnimatedContainer.SetOnShowCallback(() =>
            {
                FluidField GetField(SerializedProperty property, string fieldLabelText) =>
                    FluidField.Get()
                        .SetLabelText(fieldLabelText)
                        .SetElementSize(ElementSize.Small)
                        .AddFieldContent(DesignUtils.NewPropertyField(property));

                callbacksAnimatedContainer
                    .AddContent(GetField(propertyOnToggleOnCallback, "Toggle ON - toggle value changed from OFF to ON"))
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent(GetField(propertyOnInstantToggleOnCallback, "Instant Toggle ON - toggle value changed from OFF to ON (without animations)"))
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(GetField(propertyOnToggleOffCallback, "Toggle OFF - toggle value changed from ON to OFF"))
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent(GetField(propertyOnInstantToggleOffCallback, "Instant Toggle OFF - toggle value changed from ON to OFF (without animations)"))
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(GetField(propertyOnValueChangedCallback, "Toggle Value Changed - toggle value changed"))
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(GetField(propertyOnToggleGroupMixedValuesCallback, "Toggle Group has Mixed Values - toggle group hasMixedValues becomes TRUE"))
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(GetField(propertyOnToggleAddedCallback, "Toggle Added - toggle added to the toggle group"))
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(GetField(propertyOnToggleRemovedCallback, "Toggle Removed - toggle removed from the toggle group"))
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(GetField(propertyOnToggleTriggeredCallback, "Toggle Triggered - toggle in the group is triggered (this includes the toggle group itself)"))
                    .AddContent(DesignUtils.endOfLineBlock)
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
                .AddChild(callbacksTab)
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
                            () => $"Toggle Group - {castedTarget.Id.Name}"
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
                        nameof(UIToggle),
                        nameof(UIToggleGroup)
                    )
                );

            return toolbarContainer;
        }

        protected override VisualElement Content()
        {
            return
                contentContainer
                    .AddChild(settingsAnimatedContainer)
                    .AddChild(statesAnimatedContainer)
                    .AddChild(behavioursAnimatedContainer)
                    .AddChild(callbacksAnimatedContainer)
                    .AddChild(navigationAnimatedContainer);
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
