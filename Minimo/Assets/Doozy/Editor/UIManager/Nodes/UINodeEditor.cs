// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Nody.Nodes.Internal;
using Doozy.Editor.UIElements;
using Doozy.Editor.UIManager.Nodes.PortData;
using Doozy.Runtime.Colors;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Nodes;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Nodes
{
    [CustomEditor(typeof(UINode))]
    public class UINodeEditor : FlowNodeEditor
    {
        public override IEnumerable<Texture2D> nodeIconTextures => EditorSpriteSheets.Nody.Icons.UINode;

        private FluidComponentHeader enterNodeHeader { get; set; }
        private FluidComponentHeader exitNodeHeader { get; set; }
        private FluidToggleSwitch onEnterHideAllViewsSwitch { get; set; }
        private FluidToggleSwitch onExitHideAllViewsSwitch { get; set; }

        private SerializedProperty propertyOnEnterShowViews { get; set; }
        private SerializedProperty propertyOnEnterHideViews { get; set; }
        private SerializedProperty propertyOnExitShowViews { get; set; }
        private SerializedProperty propertyOnExitHideViews { get; set; }
        private SerializedProperty propertyOnEnterHideAllViews { get; set; }
        private SerializedProperty propertyOnExitHideAllViews { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            enterNodeHeader?.Recycle();
            exitNodeHeader?.Recycle();
            onEnterHideAllViewsSwitch?.Recycle();
            onExitHideAllViewsSwitch?.Recycle();
        }

        protected override void FindProperties()
        {
            base.FindProperties();

            propertyOnEnterShowViews = serializedObject.FindProperty("OnEnterShowViews");
            propertyOnEnterHideViews = serializedObject.FindProperty("OnEnterHideViews");
            propertyOnExitShowViews = serializedObject.FindProperty("OnExitShowViews");
            propertyOnExitHideViews = serializedObject.FindProperty("OnExitHideViews");
            propertyOnEnterHideAllViews = serializedObject.FindProperty("OnEnterHideAllViews");
            propertyOnExitHideAllViews = serializedObject.FindProperty("OnExitHideAllViews");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(UINode)))
                .AddManualButton()
                .AddApiButton("https://api.doozyui.com/api/Doozy.Runtime.UIManager.Nodes.UINode.html")
                .AddYouTubeButton();

            enterNodeHeader =
                GetHeader()
                    .SetComponentNameText("On Enter Node")
                    .SetIcon(EditorSpriteSheets.Nody.Icons.EnterNode)
                    .SetAccentColor(EditorColors.Nody.Input);

            onEnterHideAllViewsSwitch =
                FluidToggleSwitch.Get()
                    .SetToggleAccentColor(EditorSelectableColors.Nody.Input)
                    .SetLabelText("Hide All Views")
                    .BindToProperty(propertyOnEnterHideAllViews);

            exitNodeHeader =
                GetHeader()
                    .SetComponentNameText("On Exit Node")
                    .SetIcon(EditorSpriteSheets.Nody.Icons.ExitNode)
                    .SetAccentColor(EditorColors.Nody.Output);

            onExitHideAllViewsSwitch =
                FluidToggleSwitch.Get()
                    .SetToggleAccentColor(EditorSelectableColors.Nody.Output)
                    .SetLabelText("Hide All Views")
                    .BindToProperty(propertyOnExitHideAllViews);

            RefreshNodeEditor();
        }

        public override void RefreshNodeEditor()
        {
            base.RefreshNodeEditor();
            flowNode.outputPorts.ForEach(p => portsContainer.AddChild(new UIOutputPortDataEditor(p, nodeView)));
        }

        protected override void Compose()
        {
            base.Compose();

            root
                .AddSpaceBlock(2)
                .AddChild(portsContainer)
                .AddSpaceBlock(3)
                .AddChild(enterNodeHeader)
                .AddSpaceBlock()
                .AddChild
                (
                    showHideContainer
                        .SetStyleBorderColor(EditorColors.Nody.Input.WithAlpha(0.4f))
                        .AddChild(onEnterHideAllViewsSwitch)
                        .AddSpaceBlock(2)
                        .AddChild
                        (
                            new IdsPropertyList(propertyOnEnterShowViews)
                                .SetListTitle("Show Views")
                                .SetListDescription("Views that will be shown when the node is activated")
                                .SetListTitleColor(EditorColors.Nody.Input)
                        )
                        .AddSpaceBlock(2)
                        .AddChild
                        (
                            new IdsPropertyList(propertyOnEnterHideViews)
                                .SetListTitle("Hide Views")
                                .SetListDescription("Views that will be hidden when the node is activated")
                                .SetListTitleColor(EditorColors.Nody.Input)
                        )
                )
                .AddSpaceBlock(4)
                .AddChild(exitNodeHeader)
                .AddSpaceBlock()
                .AddChild
                (
                    showHideContainer
                        .SetStyleBorderColor(EditorColors.Nody.Output.WithAlpha(0.4f))
                        .AddChild(onExitHideAllViewsSwitch)
                        .AddSpaceBlock(2)
                        .AddChild
                        (
                            new IdsPropertyList(propertyOnExitShowViews)
                                .SetListTitle("Show Views")
                                .SetListDescription("Views that will be shown when the node is deactivated")
                                .SetListTitleColor(EditorColors.Nody.Output)
                        )
                        .AddSpaceBlock(2)
                        .AddChild
                        (
                            new IdsPropertyList(propertyOnExitHideViews)
                                .SetListTitle("Hide Views")
                                .SetListDescription("Views that will be hidden when the node is deactivated")
                                .SetListTitleColor(EditorColors.Nody.Output)
                        )
                )
                .AddSpaceBlock(2)
                ;
        }

        private static VisualElement showHideContainer =>
            DesignUtils.column
                .SetStyleBorderRadius(6)
                .SetStyleBorderWidth(1)
                .SetStylePadding(DesignUtils.k_Spacing2X);

        private static FluidComponentHeader GetHeader() => FluidComponentHeader.Get().SetElementSize(ElementSize.Small);

        private class IdsPropertyList : VisualElement
        {
            private SerializedProperty property { get; }

            //REFERENCES
            private VisualElement layoutContainer { get; }
            private Label titleLabel { get; }
            private Label descriptionLabel { get; }
            private VisualElement toolbarContainer { get; }
            private VisualElement listContainer { get; }
            private VisualElement addItemButtonContainer { get; }

            //ACTIONS
            public UnityAction AddNewItemButtonCallback;

            //SELECTABLE COLORS
            private static EditorSelectableColorInfo actionSelectableColor => EditorSelectableColors.Default.Action;
            private static EditorSelectableColorInfo addSelectableColor => EditorSelectableColors.Default.Add;
            private static EditorSelectableColorInfo removeSelectableColor => EditorSelectableColors.Default.Remove;

            //COLORS
            private static Color listNameTextColor => EditorColors.Default.TextTitle;
            private static Color listDescriptionTextColor => EditorColors.Default.TextDescription;
            private static Color backgroundColor => EditorColors.Default.Background;

            //FONTS
            private static Font listNameFont => EditorFonts.Ubuntu.Light;
            private static Font listDescriptionFont => EditorFonts.Inter.Light;

            private List<PropertyRow> rows { get; }

            public IdsPropertyList(SerializedProperty property)
            {
                if (property == null) return;
                if (!property.isArray) return;

                this.property = property;

                layoutContainer =
                    new VisualElement()
                        .SetStyleBackgroundColor(backgroundColor)
                        .SetStylePadding(DesignUtils.k_Spacing)
                        .SetStyleBorderRadius(DesignUtils.k_Spacing2X);

                titleLabel =
                    new Label()
                        .SetStyleColor(listNameTextColor)
                        .SetStyleUnityFont(listNameFont)
                        .SetStylePadding(DesignUtils.k_Spacing)
                        .SetStyleDisplay(DisplayStyle.None)
                        .SetStyleFontSize(14);

                descriptionLabel =
                    new Label()
                        .SetStyleColor(listDescriptionTextColor)
                        .SetStyleUnityFont(listDescriptionFont)
                        .SetStyleDisplay(DisplayStyle.None)
                        .SetStyleFontSize(10)
                        .SetStylePaddingLeft(DesignUtils.k_Spacing)
                        .SetWhiteSpace(WhiteSpace.Normal);

                toolbarContainer =
                    new VisualElement()
                        .SetStyleFlexDirection(FlexDirection.Row)
                        .SetStyleFlexGrow(1);

                addItemButtonContainer =
                    new VisualElement()
                        .SetStyleFlexDirection(FlexDirection.Row)
                        .SetStyleFlexGrow(0)
                        .SetStyleFlexShrink(0)
                        .SetStylePaddingLeft(DesignUtils.k_Spacing)
                        .SetStylePaddingRight(DesignUtils.k_Spacing2X)
                        .AddChild
                        (
                            DesignUtils.row
                                .AddChild(DesignUtils.dividerVertical)
                                .AddSpaceBlock()
                                .AddChild(Buttons.addButton.SetOnClick(AddNewItem).SetStyleAlignSelf(Align.FlexEnd))
                        );

                listContainer
                    = new VisualElement()
                        .SetStyleBackgroundColor(backgroundColor);

                this
                    .AddChild
                    (
                        layoutContainer
                            .AddChild(titleLabel)
                            .AddChild(descriptionLabel)
                            .AddSpaceBlock()
                            .AddChild
                            (
                                DesignUtils.row
                                    .AddChild(toolbarContainer)
                                    .AddChild(addItemButtonContainer)
                            )
                            .AddSpaceBlock()
                            .AddChild(listContainer)
                    );

                const int rowsCapacity = 5;
                rows = new List<PropertyRow>(rowsCapacity);

                RefreshRows();

                //every 50ms check if the array size has changed
                //this is needed to for Undo/Redo operations
                schedule.Execute
                    (
                        () =>
                        {
                            if (property.arraySize != rows.Count)
                                RefreshRows();
                        }
                    )
                    .Every(30);
            }


            public IdsPropertyList SetListTitle(string listTitle)
            {
                titleLabel.SetStyleDisplay(listTitle.IsNullOrEmpty() ? DisplayStyle.None : DisplayStyle.Flex);
                titleLabel.text = listTitle;
                return this;
            }

            public IdsPropertyList SetListDescription(string listDescription)
            {
                descriptionLabel.SetStyleDisplay(listDescription.IsNullOrEmpty() ? DisplayStyle.None : DisplayStyle.Flex);
                descriptionLabel.text = listDescription;
                return this;
            }

            public IdsPropertyList SetListTitleColor(Color color)
            {
                titleLabel.SetStyleColor(color);
                return this;
            }

            public IdsPropertyList SetListDescriptionColor(Color color)
            {
                descriptionLabel.SetStyleColor(color);
                return this;
            }

            private void RefreshRows()
            {
                property.serializedObject.UpdateIfRequiredOrScript();
                
                // make sure the row capacity is enough (in case the array size was increased to a crazy number)
                if (rows.Capacity < property.arraySize)
                    rows.Capacity = property.arraySize;

                for (int i = rows.Count - 1; i >= 0; i--)
                {
                    VisualElement row = rows[i];
                    if (row == null) continue;
                    rows[i].Recycle();
                    rows.RemoveAt(i);
                }

                if (rows.Count > 0)
                    rows.Clear();

                listContainer
                    .RecycleAndClear();

                for (int i = 0; i < property.arraySize; i++)
                {
                    PropertyRow row = GetRow(i);
                    rows.Add(row);
                    listContainer.AddChild(row);
                }

                listContainer.Bind(property.serializedObject);
            }

            private PropertyRow GetRow(int elementIndex) =>
                PropertyRow.Get()
                    .SetProperty(property.GetArrayElementAtIndex(elementIndex))
                    .SetRemoveAction(() => RemoveItem(elementIndex));

            private void RemoveItem(int elementIndex)
            {
                //check if the index is valid
                if (elementIndex < 0 || elementIndex >= property.arraySize)
                    return;

                //remove the property
                property.DeleteArrayElementAtIndex(elementIndex);
                property.serializedObject.ApplyModifiedProperties();
                //refresh the rows
                RefreshRows();
            }

            private void AddNewItem()
            {
                //add a new element to the array
                property.InsertArrayElementAtIndex(property.arraySize);
                property.serializedObject.ApplyModifiedProperties();
                //refresh the rows
                RefreshRows();
            }

            public class PropertyRow : PoolableElement<PropertyRow>
            {
                private SerializedProperty property { get; set; }
                private PropertyField propertyField { get; }
                private FluidButton removeButton { get; }

                public override void Reset()
                {
                    propertyField.Unbind();
                    property = null;
                    propertyField.SetBindingPath(null);
                    removeButton.ClearOnClick();
                    removeButton.SetSelectionState(SelectionState.Normal);
                }

                public PropertyRow()
                {
                    propertyField =
                        new PropertyField()
                            .ResetLayout()
                            .SetStyleFlexGrow(1);

                    removeButton =
                        Buttons.removeButton;

                    this
                        .SetStyleAlignItems(Align.Center)
                        .SetStyleBorderRadius(DesignUtils.k_FieldBorderRadius)
                        .SetStylePadding(DesignUtils.k_Spacing)
                        .SetStyleMargins(DesignUtils.k_Spacing)
                        .SetStyleBackgroundColor(EditorColors.Default.FieldBackground)
                        .SetStyleFlexDirection(FlexDirection.Row);

                    this
                        .AddChild(propertyField)
                        .AddSpaceBlock()
                        .AddChild(DesignUtils.dividerVertical)
                        .AddSpaceBlock()
                        .AddChild(removeButton);
                }

                public PropertyRow SetProperty(SerializedProperty newProperty)
                {
                    property = newProperty;
                    propertyField.SetBindingPath(property.propertyPath);
                    return this;
                }

                public PropertyRow SetRemoveAction(UnityAction action)
                {
                    removeButton.SetOnClick(action);
                    return this;
                }

            }

            private static class Buttons
            {
                private const ElementSize k_Size = ElementSize.Small;
                private const ButtonStyle k_ButtonStyle = ButtonStyle.Clear;
                private static EditorSelectableColorInfo accentColor => actionSelectableColor;

                // ReSharper disable once MemberCanBePrivate.Local
                public static FluidButton GetNewToolbarButton(IEnumerable<Texture2D> textures, string tooltip = "") =>
                    FluidButton.Get()
                        .SetIcon(textures)
                        .SetElementSize(k_Size)
                        .SetButtonStyle(k_ButtonStyle)
                        .SetAccentColor(accentColor)
                        .SetTooltip(tooltip);

                public static FluidButton addButton => GetNewToolbarButton(EditorSpriteSheets.EditorUI.Icons.Plus, "Add Item").SetAccentColor(addSelectableColor);
                public static FluidButton removeButton => GetNewToolbarButton(EditorSpriteSheets.EditorUI.Icons.Minus, "Remove Item").SetAccentColor(removeSelectableColor);
            }

        }
    }
}
