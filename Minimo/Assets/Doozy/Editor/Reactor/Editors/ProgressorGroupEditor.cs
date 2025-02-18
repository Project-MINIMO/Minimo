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
using Doozy.Runtime.Reactor;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.Reactor.Editors
{
    [CustomEditor(typeof(ProgressorGroup), true)]
    [CanEditMultipleObjects]
    public class ProgressorGroupEditor : UnityEditor.Editor
    {
        public ProgressorGroup castedTarget => (ProgressorGroup)target;
        public List<ProgressorGroup> castedTargets => targets.Cast<ProgressorGroup>().ToList();

        public static Color accentColor => EditorColors.Reactor.Red;
        public static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Reactor.Red;

        private VisualElement root { get; set; }
        private FluidComponentHeader componentHeader { get; set; }
        private VisualElement toolbarContainer { get; set; }
        private VisualElement contentContainer { get; set; }

        private FluidToggleGroup tabsGroup { get; set; }
        private FluidTab settingsTab { get; set; }
        private FluidTab callbacksTab { get; set; }
        private FluidTab progressTargetsTab { get; set; }
        private FluidTab progressorsTab { get; set; }

        private FluidProgressBar progressBar { get; set; }

        private FluidAnimatedContainer settingsAnimatedContainer { get; set; }
        private FluidAnimatedContainer callbacksAnimatedContainer { get; set; }
        private FluidAnimatedContainer progressTargetsAnimatedContainer { get; set; }
        private FluidAnimatedContainer progressorsAnimatedContainer { get; set; }

        private SerializedProperty propertyProgress { get; set; }
        private SerializedProperty propertyProgressors { get; set; }
        private SerializedProperty propertyProgressTargets { get; set; }
        private SerializedProperty propertyOnProgressChanged { get; set; }
        private SerializedProperty propertyOnProgressIncremented { get; set; }
        private SerializedProperty propertyOnProgressDecremented { get; set; }
        private SerializedProperty propertyOnProgressReachedOne { get; set; }
        private SerializedProperty propertyOnProgressReachedZero { get; set; }

        private void OnDestroy()
        {
            componentHeader?.Recycle();

            tabsGroup?.Recycle();
            settingsTab?.Recycle();
            callbacksTab?.Recycle();
            progressTargetsTab?.Recycle();
            progressorsTab?.Recycle();

            settingsAnimatedContainer?.Dispose();
            callbacksAnimatedContainer?.Dispose();
            progressTargetsAnimatedContainer?.Dispose();
            progressorsAnimatedContainer?.Dispose();
        }

        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            return root;
        }

        private void FindProperties()
        {
            propertyProgress = serializedObject.FindProperty("Progress");
            propertyProgressors = serializedObject.FindProperty("Progressors");
            propertyProgressTargets = serializedObject.FindProperty("ProgressTargets");
            propertyOnProgressChanged = serializedObject.FindProperty("OnProgressChanged");
            propertyOnProgressIncremented = serializedObject.FindProperty("OnProgressIncremented");
            propertyOnProgressDecremented = serializedObject.FindProperty("OnProgressDecremented");
            propertyOnProgressReachedOne = serializedObject.FindProperty("OnProgressReachedOne");
            propertyOnProgressReachedZero = serializedObject.FindProperty("OnProgressReachedZero");
        }

        private void InitializeEditor()
        {
            FindProperties();
            root = DesignUtils.GetEditorRoot();
            componentHeader =
                DesignUtils.editorComponentHeader
                    .SetAccentColor(accentColor)
                    .SetComponentNameText("Progressor Group")
                    .SetIcon(EditorSpriteSheets.Reactor.Icons.ProgressorGroup)
                    .AddManualButton()
                    .AddApiButton()
                    .AddYouTubeButton();
            toolbarContainer = DesignUtils.editorToolbarContainer;
            tabsGroup = FluidToggleGroup.Get().SetControlMode(FluidToggleGroup.ControlMode.OneToggleOn);
            contentContainer = DesignUtils.editorContentContainer;

            InitializeSettings();
            InitializeCallbacks();
            InitializeProgressTargets();
            InitializeProgressorTargets();

            root.schedule.Execute(() => settingsTab.ButtonSetIsOn(true, false));

            //refresh tabs enabled indicator
            root.schedule.Execute(() =>
            {
                void UpdateIndicator(FluidTab tab, bool toggleOn, bool animateChange)
                {
                    if (tab == null) return;
                    if (tab.indicator.isOn != toggleOn)
                        tab.indicator.Toggle(toggleOn, animateChange);
                }

                bool HasCallbacks() =>
                    castedTarget != null &&
                    castedTarget.onProgressChanged.GetPersistentEventCount() > 0 |
                    castedTarget.onProgressIncremented.GetPersistentEventCount() > 0 |
                    castedTarget.onProgressDecremented.GetPersistentEventCount() > 0 |
                    castedTarget.onProgressReachedOne.GetPersistentEventCount() > 0 |
                    castedTarget.onProgressReachedZero.GetPersistentEventCount() > 0;

                bool HasProgressTargets() =>
                    castedTarget != null &&
                    castedTarget.progressTargets?.Count > 0;

                bool HasProgressors() =>
                    castedTarget != null &&
                    castedTarget.progressors?.Count > 0;

                //initial indicators state update (no animation)
                UpdateIndicator(callbacksTab, HasCallbacks(), false);
                UpdateIndicator(progressTargetsTab, HasProgressTargets(), false);
                UpdateIndicator(progressorsTab, HasProgressors(), false);

                //subsequent indicators state update (animated)
                root.schedule.Execute(() =>
                {
                    UpdateIndicator(callbacksTab, HasCallbacks(), true);
                    UpdateIndicator(progressTargetsTab, HasProgressTargets(), true);
                    UpdateIndicator(progressorsTab, HasProgressors(), true);

                }).Every(200);
            });
        }

        private void InitializeSettings()
        {
            settingsAnimatedContainer = new FluidAnimatedContainer("Settings", true).Hide(false);
            settingsTab =
                FluidTab.Get()
                    .SetLabelText("Settings")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Settings)
                    .IndicatorSetEnabledColor(accentColor)
                    .ButtonSetAccentColor(selectableAccentColor)
                    .ButtonSetOnValueChanged(evt => settingsAnimatedContainer.Toggle(evt.newValue, evt.animateChange))
                    .AddToToggleGroup(tabsGroup);

            settingsAnimatedContainer.AddOnShowCallback(() =>
            {
                progressBar =
                    FluidProgressBar.Get()
                        .SetIndicatorColor(accentColor)
                        .SetStyleHeight(2);

                Label progressLabel =
                    DesignUtils.fieldLabel.SetStyleFontSize(12)
                        .SetStyleColor(accentColor);


                progressBar.schedule.Execute(() =>
                {
                    if (castedTarget == null) return;
                    progressBar.reaction.SetProgressAt(castedTarget.progress);
                    progressLabel.SetText(castedTarget.progress.ToString("P0"));
                }).Every(50);


                var progressFluidField =
                    FluidField.Get("Progress")
                        .SetTooltip("The current progress value of the ProgressorGroup.")
                        .AddFieldContent
                        (
                            DesignUtils.row
                                .AddFlexibleSpace()
                                .AddChild(progressLabel)
                                .AddFlexibleSpace()
                        )
                        .AddFieldContent
                        (
                            DesignUtils.row
                                .SetStyleAlignItems(Align.Center)
                                .AddSpaceBlock(2)
                                .AddChild(DesignUtils.fieldLabel.SetStyleFontSize(10).SetText("0"))
                                .AddSpaceBlock()
                                .AddChild(progressBar)
                                .AddSpaceBlock()
                                .AddChild(DesignUtils.fieldLabel.SetStyleFontSize(10).SetText("1"))
                                .AddSpaceBlock(2)
                        );

                settingsAnimatedContainer
                    .AddContent(progressFluidField)
                    .Bind(serializedObject);
            });
        }

        private void InitializeCallbacks()
        {
            callbacksAnimatedContainer = new FluidAnimatedContainer("Callbacks", true).Hide(false);
            callbacksTab =
                FluidTab.Get()
                    .SetLabelText("Callbacks")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.UnityEvent)
                    .IndicatorSetEnabledColor(DesignUtils.callbacksColor)
                    .ButtonSetAccentColor(DesignUtils.callbackSelectableColor)
                    .ButtonSetOnValueChanged(evt => callbacksAnimatedContainer.Toggle(evt.newValue, evt.animateChange))
                    .AddToToggleGroup(tabsGroup);

            callbacksAnimatedContainer.SetOnShowCallback(() =>
            {
                callbacksAnimatedContainer
                    .AddContent(FluidField.Get().AddFieldContent(DesignUtils.UnityEventField("Progress Changed - [0, 1]", propertyOnProgressChanged)))
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(FluidField.Get().AddFieldContent(DesignUtils.UnityEventField("On Progress Incremented", propertyOnProgressIncremented)))
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(FluidField.Get().AddFieldContent(DesignUtils.UnityEventField("On Progress Decremented", propertyOnProgressDecremented)))
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(FluidField.Get().AddFieldContent(DesignUtils.UnityEventField("On Progress Reached One", propertyOnProgressReachedOne)))
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(FluidField.Get().AddFieldContent(DesignUtils.UnityEventField("On Progress Reached Zero", propertyOnProgressReachedZero)))
                    .Bind(serializedObject);
            });
        }

        private void InitializeProgressTargets()
        {
            progressTargetsAnimatedContainer = new FluidAnimatedContainer("Targets", true).Hide(false);
            progressTargetsTab =
                FluidTab.Get()
                    .SetLabelText("Targets")
                    .SetIcon(EditorSpriteSheets.Reactor.Icons.ProgressTarget)
                    .IndicatorSetEnabledColor(EditorColors.Reactor.Red)
                    .ButtonSetAccentColor(EditorSelectableColors.Reactor.Red)
                    .ButtonSetOnValueChanged(evt => progressTargetsAnimatedContainer.Toggle(evt.newValue, evt.animateChange))
                    .AddToToggleGroup(tabsGroup);

            progressTargetsAnimatedContainer.SetOnShowCallback(() =>
            {
                progressTargetsAnimatedContainer
                    .AddContent
                    (
                        DesignUtils.NewObjectListView
                        (
                            propertyProgressTargets,
                            "Progress Targets",
                            "Progress targets that get updated by this ProgressorGroup when activated",
                            typeof(ProgressTarget)
                        )
                    )
                    .Bind(serializedObject);
            });
        }

        private void InitializeProgressorTargets()
        {
            progressorsAnimatedContainer = new FluidAnimatedContainer("Progressors", true).Hide(false);
            progressorsTab =
                FluidTab.Get()
                    .SetLabelText("Progressors")
                    .SetIcon(EditorSpriteSheets.Reactor.Icons.Progressor)
                    .IndicatorSetEnabledColor(EditorColors.Reactor.Red)
                    .ButtonSetAccentColor(EditorSelectableColors.Reactor.Red)
                    .ButtonSetOnValueChanged(evt => progressorsAnimatedContainer.Toggle(evt.newValue, evt.animateChange))
                    .AddToToggleGroup(tabsGroup);

            progressorsAnimatedContainer.SetOnShowCallback(() =>
            {
                progressorsAnimatedContainer
                    .AddContent
                    (
                        DesignUtils.NewObjectListView
                        (
                            propertyProgressors,
                            "Progressors",
                            "Progressors sources that are used to calculate the mean progress value for this ProgressorGroup",
                            typeof(Progressor)
                        )
                    )
                    .Bind(serializedObject);
            });
        }

        private VisualElement Toolbar()
        {
            return
                toolbarContainer
                    .AddChild(settingsTab)
                    .AddSpaceBlock(2)
                    .AddChild(callbacksTab)
                    .AddSpaceBlock(2)
                    .AddChild(progressTargetsTab)
                    .AddSpaceBlock(2)
                    .AddChild(progressorsTab)
                    .AddSpaceBlock()
                    .AddChild(DesignUtils.flexibleSpace);
        }

        private VisualElement Content()
        {
            return
                contentContainer
                    .AddChild(settingsAnimatedContainer)
                    .AddChild(callbacksAnimatedContainer)
                    .AddChild(progressTargetsAnimatedContainer)
                    .AddChild(progressorsAnimatedContainer);
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
