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
using Doozy.Runtime.Nody;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Input;
using Doozy.Runtime.UIManager.ScriptableObjects;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable UnusedMember.Local

namespace Doozy.Editor.Nody.Editors
{
    [CustomEditor(typeof(FlowController), true)]
    public class FlowControllerEditor : UnityEditor.Editor
    {
        private static Color accentColor => EditorColors.Nody.Color;
        private static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Nody.Color;

        private FlowController castedTarget => (FlowController)target;
        private IEnumerable<FlowController> castedTargets => targets.Cast<FlowController>();

        private bool hasOnStartCallback => castedTarget.onStart?.GetPersistentEventCount() > 0;
        private bool hasOnStopCallback => castedTarget.onStop?.GetPersistentEventCount() > 0;
        private bool hasOnPauseCallback => castedTarget.onPause?.GetPersistentEventCount() > 0;
        private bool hasOnResumeCallback => castedTarget.onResume?.GetPersistentEventCount() > 0;
        private bool hasOnBackFlowCallback => castedTarget.onBackFlow?.GetPersistentEventCount() > 0;
        private bool hasCallbacks => hasOnStartCallback | hasOnStopCallback | hasOnPauseCallback | hasOnResumeCallback | hasOnBackFlowCallback;

        private VisualElement root { get; set; }
        private FluidComponentHeader componentHeader { get; set; }
        private VisualElement toolbarContainer { get; set; }
        private VisualElement contentContainer { get; set; }

        private FluidToggleGroup tabsGroup { get; set; }
        private FluidTab settingsTab { get; set; }
        private FluidTab callbacksTab { get; set; }

        private FluidAnimatedContainer callbacksAnimatedContainer { get; set; }
        private FluidAnimatedContainer settingsAnimatedContainer { get; set; }

        private FluidField flowField { get; set; }
        private FluidField flowTypeFluidField { get; set; }
        private FluidToggleSwitch dontDestroyOnSceneChangeSwitch { get; set; }
        private FluidField multiplayerInfoField { get; set; }

        private FluidButton openNodyButton { get; set; }

        private SerializedProperty propertyDontDestroyOnSceneChange { get; set; }
        private SerializedProperty propertyFlow { get; set; }
        private SerializedProperty propertyFlowType { get; set; }
        private SerializedProperty propertyOnEnableBehaviour { get; set; }
        private SerializedProperty propertyOnDisableBehaviour { get; set; }
        private SerializedProperty propertyOnStart { get; set; }
        private SerializedProperty propertyOnStop { get; set; }
        private SerializedProperty propertyOnPause { get; set; }
        private SerializedProperty propertyOnResume { get; set; }
        private SerializedProperty propertyOnBackFlow { get; set; }
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

            tabsGroup?.Recycle();
            settingsTab?.Dispose();
            callbacksTab?.Dispose();

            callbacksAnimatedContainer?.Dispose();
            settingsAnimatedContainer?.Dispose();

            flowField?.Recycle();
            flowTypeFluidField?.Recycle();
            dontDestroyOnSceneChangeSwitch?.Recycle();
            multiplayerInfoField?.Recycle();

            openNodyButton?.Recycle();
        }

        private void FindProperties()
        {
            propertyDontDestroyOnSceneChange = serializedObject.FindProperty("DontDestroyOnSceneChange");
            propertyFlow = serializedObject.FindProperty("Flow");
            propertyFlowType = serializedObject.FindProperty("FlowType");
            propertyOnEnableBehaviour = serializedObject.FindProperty("OnEnableBehaviour");
            propertyOnDisableBehaviour = serializedObject.FindProperty("OnDisableBehaviour");
            propertyOnStart = serializedObject.FindProperty("OnStart");
            propertyOnStop = serializedObject.FindProperty("OnStop");
            propertyOnPause = serializedObject.FindProperty("OnPause");
            propertyOnResume = serializedObject.FindProperty("OnResume");
            propertyOnBackFlow = serializedObject.FindProperty("OnBackFlow");
            propertyMultiplayerInfo = serializedObject.FindProperty("MultiplayerInfo");
        }

        private void InitializeEditor()
        {
            FindProperties();
            root = DesignUtils.GetEditorRoot();
            componentHeader =
                DesignUtils.editorComponentHeader
                    .SetAccentColor(accentColor)
                    .SetComponentNameText((ObjectNames.NicifyVariableName(nameof(FlowController))))
                    .SetIcon(EditorSpriteSheets.Nody.Icons.GraphController)
                    .AddManualButton()
                    .AddApiButton("https://api.doozyui.com/api/Doozy.Runtime.Nody.FlowController.html")
                    .AddYouTubeButton();
            toolbarContainer = DesignUtils.editorToolbarContainer;
            tabsGroup = FluidToggleGroup.Get().SetControlMode(FluidToggleGroup.ControlMode.OneToggleOn);
            contentContainer = DesignUtils.editorContentContainer;

            InitializeSettings();
            InitializeCallbacks();

            root.schedule.Execute(() => settingsTab.ButtonSetIsOn(true, false));
        }

        private void InitializeSettings()
        {
            settingsAnimatedContainer = new FluidAnimatedContainer("Settings", true).Hide(false);
            settingsTab =
                new FluidTab()
                    .SetLabelText("Settings")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Settings)
                    .IndicatorSetEnabledColor(accentColor)
                    .ButtonSetAccentColor(selectableAccentColor)
                    .ButtonSetOnValueChanged(evt => settingsAnimatedContainer.Toggle(evt.newValue, evt.animateChange))
                    .AddToToggleGroup(tabsGroup);

            settingsAnimatedContainer.AddOnShowCallback(() =>
            {
                dontDestroyOnSceneChangeSwitch =
                    FluidToggleSwitch.Get()
                        .SetToggleAccentColor(selectableAccentColor)
                        .SetLabelText("Don't destroy controller on scene change")
                        .SetTooltip
                        (
                            "If enabled, the controller will not be destroyed when the scene changes." +
                            "\n\n" +
                            "This is useful if you want to keep the controller active between scenes." +
                            "\n\n" +
                            "For this to work, the controller must be a root object in the scene hierarchy (it must not have a parent)"
                        )
                        .BindToProperty(propertyDontDestroyOnSceneChange);

                dontDestroyOnSceneChangeSwitch.SetEnabled(castedTarget.transform.parent == null);
                if (castedTarget.transform.parent != null && propertyDontDestroyOnSceneChange.boolValue)
                {
                    propertyDontDestroyOnSceneChange.boolValue = false;
                    serializedObject.ApplyModifiedPropertiesWithoutUndo();
                }

                openNodyButton =
                    FluidButton.Get()
                        .SetIcon(EditorSpriteSheets.Nody.Icons.Nody)
                        .SetLabelText("Nody")
                        .SetTooltip("Open referenced graph in Nody")
                        .SetStyleFlexShrink(0)
                        .SetAccentColor(EditorSelectableColors.Nody.Color)
                        .SetButtonStyle(ButtonStyle.Contained)
                        .SetElementSize(ElementSize.Tiny)
                        .SetOnClick(() =>
                        {
                            NodyWindow.Open();
                            NodyWindow.OpenGraph(castedTarget.flow);
                        });

                flowField =
                    FluidField.Get()
                        .SetLabelText("Flow Graph")
                        .AddFieldContent
                        (
                            DesignUtils.row
                                .SetStyleFlexGrow(0)
                                .SetStyleFlexShrink(1)
                                .AddChild(DesignUtils.NewObjectField(propertyFlow, typeof(FlowGraph)).SetStyleFlexGrow(1))
                                .AddSpaceBlock(2)
                                .AddChild(openNodyButton)
                        );

                flowTypeFluidField =
                    FluidField.Get()
                        .SetStyleFlexGrow(0)
                        .SetStyleFlexShrink(0)
                        .SetLabelText("Flow Type")
                        .AddFieldContent(DesignUtils.NewEnumField(propertyFlowType).SetStyleWidth(60).SetStyleFlexShrink(0));

                multiplayerInfoField =
                    FluidField.Get()
                        .SetLabelText("Player Index")
                        .AddFieldContent(DesignUtils.NewObjectField(propertyMultiplayerInfo, typeof(MultiplayerInfo)).SetStyleFlexGrow(1))
                        .SetStyleMarginTop(DesignUtils.k_Spacing2X)
                        .SetStyleDisplay(UIManagerInputSettings.instance.multiplayerMode ? DisplayStyle.Flex : DisplayStyle.None);

                void SetBehaviourEnumFieldTooltip(string prefix, EnumField field, ControllerBehaviour behaviour)
                {
                    switch (behaviour)
                    {
                        case ControllerBehaviour.Disabled:
                            field.SetTooltip($"{prefix} - Disabled (does nothing)");
                            break;
                        case ControllerBehaviour.StartFlow:
                            field.SetTooltip($"{prefix} - Start or Restart or Resume (if paused) the flow graph");
                            break;
                        case ControllerBehaviour.RestartFlow:
                            field.SetTooltip($"{prefix} - Restart the flow graph (even if it's paused or running)");
                            break;
                        case ControllerBehaviour.StopFlow:
                            field.SetTooltip($"{prefix} - Stop the flow graph");
                            break;
                        case ControllerBehaviour.PauseFlow:
                            field.SetTooltip($"{prefix} - Pause the flow graph");
                            break;
                        case ControllerBehaviour.ResumeFlow:
                            field.SetTooltip($"{prefix} - Resume Flow");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(behaviour), behaviour, null);
                    }
                }

                EnumField onEnableBehaviourEnumField =
                    DesignUtils.NewEnumField(propertyOnEnableBehaviour)
                        .SetStyleFlexGrow(1);

                FluidField onEnableBehaviourFluidField =
                    FluidField.Get()
                        .SetLabelText("OnEnable Behaviour")
                        .AddFieldContent(onEnableBehaviourEnumField);

                SetBehaviourEnumFieldTooltip("OnEnable", onEnableBehaviourEnumField, (ControllerBehaviour)propertyOnEnableBehaviour.enumValueIndex);
                
                onEnableBehaviourEnumField.RegisterValueChangedCallback(evt =>
                {
                    if(evt?.newValue == null) return;
                    SetBehaviourEnumFieldTooltip("OnEnable", onEnableBehaviourEnumField, (ControllerBehaviour)evt.newValue);
                });
                
                EnumField onDisableBehaviourEnumField =
                    DesignUtils.NewEnumField(propertyOnDisableBehaviour)
                        .SetStyleFlexGrow(1);

                FluidField onDisableBehaviourFluidField =
                    FluidField.Get()
                        .SetLabelText("OnDisable Behaviour")
                        .AddFieldContent(onDisableBehaviourEnumField);

                SetBehaviourEnumFieldTooltip("OnDisable", onDisableBehaviourEnumField, (ControllerBehaviour)propertyOnDisableBehaviour.enumValueIndex);
                
                onDisableBehaviourEnumField.RegisterValueChangedCallback(evt =>
                {
                    if(evt?.newValue == null) return;
                    SetBehaviourEnumFieldTooltip("OnDisable", onDisableBehaviourEnumField, (ControllerBehaviour)evt.newValue);
                });

                settingsAnimatedContainer
                    .AddContent(dontDestroyOnSceneChangeSwitch)
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent
                    (
                        DesignUtils.row
                            .AddChild(flowField)
                            .AddSpaceBlock()
                            .AddChild(flowTypeFluidField)
                    )
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent
                    (
                        DesignUtils.row
                            .AddChild(onEnableBehaviourFluidField)
                            .AddSpaceBlock()
                            .AddChild(onDisableBehaviourFluidField)
                    )
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(multiplayerInfoField)
                    .Bind(serializedObject);
            });
        }

        private void InitializeCallbacks()
        {
            callbacksAnimatedContainer = new FluidAnimatedContainer("Callbacks", true).Hide(false);
            callbacksTab =
                new FluidTab()
                    .SetLabelText("Callbacks")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.UnityEvent)
                    .IndicatorSetEnabledColor(DesignUtils.callbacksColor)
                    .ButtonSetAccentColor(DesignUtils.callbackSelectableColor)
                    .ButtonSetOnValueChanged(evt => callbacksAnimatedContainer.Toggle(evt.newValue, evt.animateChange))
                    .AddToToggleGroup(tabsGroup);

            callbacksAnimatedContainer.AddOnShowCallback(() =>
            {
                callbacksAnimatedContainer
                    .AddContent(
                        FluidField.Get()
                            .AddFieldContent(DesignUtils.UnityEventField("Flow graph starts or restarts", propertyOnStart))
                    )
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(
                        FluidField.Get()
                            .AddFieldContent(DesignUtils.UnityEventField("Flow graph is stopped", propertyOnStop))
                    )
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(
                        FluidField.Get()
                            .AddFieldContent(DesignUtils.UnityEventField("Flow graph is paused", propertyOnPause))
                    )
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(
                        FluidField.Get()
                            .AddFieldContent(DesignUtils.UnityEventField("Flow graph is resumed", propertyOnResume))
                    )
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(
                        FluidField.Get()
                            .AddFieldContent(DesignUtils.UnityEventField("'Back' flow is triggered in the flow graph", propertyOnBackFlow))
                    )
                    .Bind(serializedObject);
            });

            //refresh tabs enabled indicator
            root.schedule.Execute(() =>
            {
                void UpdateIndicator(FluidTab tab, bool toggleOn, bool animateChange)
                {
                    if (tab == null) return;
                    if (tab.indicator.isOn != toggleOn)
                        tab.indicator.Toggle(toggleOn, animateChange);
                }

                bool HasOnStartCallbacks() => castedTarget.onStart != null && castedTarget.onStart.GetPersistentEventCount() > 0;

                bool HasOnStopCallbacks() => castedTarget.onStop != null && castedTarget.onStop.GetPersistentEventCount() > 0;

                bool HasCallbacks() => HasOnStartCallbacks() || HasOnStopCallbacks();

                //initial indicators state update (no animation)
                UpdateIndicator(callbacksTab, HasCallbacks(), false);

                //subsequent indicators state update (animated)
                callbacksTab.schedule.Execute(() => UpdateIndicator(callbacksTab, HasCallbacks(), true)).Every(200);
            });
        }

        private VisualElement Toolbar()
        {
            return
                toolbarContainer
                    .AddChild(settingsTab)
                    .AddSpaceBlock()
                    .AddChild(callbacksTab)
                    .AddSpaceBlock()
                    .AddChild(DesignUtils.flexibleSpace);
        }

        private VisualElement Content()
        {
            return
                contentContainer
                    .AddChild(settingsAnimatedContainer)
                    .AddChild(callbacksAnimatedContainer);
        }

        private void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddChild(Toolbar())
                .AddSpaceBlock(2)
                .AddChild(Content())
                .AddEndOfLineSpace();
        }
    }
}
