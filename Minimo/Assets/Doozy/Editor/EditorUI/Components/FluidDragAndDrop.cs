// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Colors;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.EditorUI.Components
{
    public class FluidDragAndDrop<T> : VisualElement where T : UnityEngine.Object
    {
        private static Color normalColor => EditorColors.Default.TextSubtitle;
        private static Color acceptColor => EditorColors.Default.Add;
        private static Color rejectColor => EditorColors.Default.Remove;

        private static Color backgroundColor => EditorColors.Default.BoxBackground;
        private static Color textColor => normalColor;
        private static Color infoTextHighlightColor => EditorColors.Default.TextTitle;
        private static Color borderColor => normalColor.WithAlpha(0.2f);
        private static Color borderAcceptColor => acceptColor.WithAlpha(0.2f);
        private static Color borderRejectColor => rejectColor.WithAlpha(0.2f);

        public Type dragAndDropType => typeof(T);
        public string dragAndDropTypeName => dragAndDropType.Name;
        public string dragAndDropTypeNamePretty => ObjectNames.NicifyVariableName(dragAndDropTypeName);
        public Label infoLabel { get; }
        public Label typeLabel { get; }
        public VisualElement overlay { get; }
        
        /// <summary> Contains all the references that were dragged and dropped on this container </summary>
        public List<T> references { get; private set; }
        
        /// <summary>
        /// Callback executed when a drag is performed over this container.
        /// This means that the drag was accepted and the references were added to the list of references.
        /// Use the references list to get the dragged references.
        /// </summary>
        public UnityAction onDragPerform { get; set; }
        
        /// <summary> Flag that tells if the drag is valid or not. </summary>
        private bool validDrag { get; set; }

        public FluidDragAndDrop(UnityAction onDragPerformCallback)
        {
            this
                .SetTooltip
                (
                    $"Drag and Drop {dragAndDropTypeName} references here." +
                    $"\n\n" +
                    $"You can also drag and drop folders, that contain {dragAndDropTypeName} references, and they too will be added to the list of references."
                );

            this
                .ResetLayout()
                .SetStyleFlexDirection(FlexDirection.Row)
                .SetStyleAlignItems(Align.Center)
                .SetStyleJustifyContent(Justify.Center)
                .SetStyleBorderRadius(DesignUtils.k_Spacing2X)
                .SetStyleBorderWidth(0.5f)
                .SetStyleBorderColor(borderColor)
                .SetStyleBackgroundColor(backgroundColor)
                .SetStylePaddingTop(DesignUtils.k_Spacing)
                .SetStylePaddingBottom(DesignUtils.k_Spacing)
                .SetStylePaddingLeft(DesignUtils.k_Spacing2X)
                .SetStylePaddingRight(DesignUtils.k_Spacing2X)
                .SetStyleMargins(DesignUtils.k_Spacing)
                .SetStyleOverflow(Overflow.Hidden);

            overlay =
                new VisualElement()
                    .ResetLayout()
                    .SetStylePosition(Position.Absolute)
                    .SetStyleTop(0)
                    .SetStyleLeft(0)
                    .SetStyleRight(0)
                    .SetStyleBottom(0)
                    .SetStyleBackgroundColor(normalColor)
                    .SetStyleOpacity(0.05f);

            typeLabel =
                DesignUtils.NewLabel($"({dragAndDropTypeNamePretty})", 9)
                    .SetStyleColor(textColor);

            infoLabel =
                DesignUtils.NewLabel($"Drag and Drop", 10)
                    .SetStyleColor(textColor);

            onDragPerform = onDragPerformCallback;

            RegisterCallback<AttachToPanelEvent>(_ =>
            {
                RegisterCallback<DragEnterEvent>(OnDragEnter);
                RegisterCallback<DragUpdatedEvent>(OnDragUpdated);
                RegisterCallback<DragPerformEvent>(OnDragPerform);
                RegisterCallback<DragExitedEvent>(OnDragExited);
                RegisterCallback<DragLeaveEvent>(OnDragLeave);
            });

            RegisterCallback<DetachFromPanelEvent>(_ =>
            {
                RegisterCallback<DragEnterEvent>(OnDragEnter);
                UnregisterCallback<DragUpdatedEvent>(OnDragUpdated);
                UnregisterCallback<DragPerformEvent>(OnDragPerform);
                UnregisterCallback<DragExitedEvent>(OnDragExited);
                UnregisterCallback<DragLeaveEvent>(OnDragLeave);
            });

            //COMPOSE
            this
                .AddChild(overlay)
                .AddChild(infoLabel)
                .AddSpaceBlock(2)
                .AddChild(typeLabel)
                ;
        }

        private void OnDragEnter(DragEnterEvent evt)
        {
            // Debug.Log("OnDragEnter");
        }

        private void OnDragUpdated(DragUpdatedEvent evt)
        {
            // Debug.Log("OnDragUpdated");
            validDrag = DragAndDrop.objectReferences.Length > 0 && DragAndDrop.objectReferences.Any(item => item.GetType() == dragAndDropType);
            if (!validDrag) //check if it's a folder
            {
                string assetPath = DragAndDrop.paths.FirstOrDefault();
                string[] paths = AssetDatabase.FindAssets($"t:{dragAndDropTypeName}", new[] { assetPath });
                validDrag = paths.Select(path => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(path))).Any(asset => asset != null);
            }
            if (!validDrag)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                SetRejectVisuals();
                return;
            }
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            SetAcceptVisuals();
        }

        private void OnDragPerform(DragPerformEvent evt)
        {
            // Debug.Log("OnDragPerform");
            references ??= new List<T>();
            references.Clear();

            // add all assets of type T from the drag and drop references to the references list
            references.AddRange
            (
                DragAndDrop.objectReferences
                    .Where(item => item.GetType() == dragAndDropType)
                    .Select(item => (T)item)
                    .OrderBy(item => item.name)
            );

            // check if we have folders and get all assets of type T from the folder and add them to the references list
            foreach (var asset in DragAndDrop.objectReferences)
            {
                string folderPath = AssetDatabase.GetAssetPath(asset);
                if (!AssetDatabase.IsValidFolder(folderPath)) continue;
                string[] guids = AssetDatabase.FindAssets($"t:{dragAndDropTypeName}", new[] { folderPath });
                foreach (string guid in guids) //get all assets of type T from the folder and add them to the references list
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    T reference = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                    if (asset == null) continue;
                    references.Add(reference);
                }
            }

            // we have no references of type T thus we have nothing to do here
            if (references.Count == 0)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.None;
                return;
            }
            DragAndDrop.AcceptDrag();
            onDragPerform?.Invoke();
        }

        private void OnDragExited(DragExitedEvent evt)
        {
            // Debug.Log("OnDragExited");
            ResetVisuals();
        }

        private void OnDragLeave(DragLeaveEvent evt)
        {
            // Debug.Log("OnDragLeave");
            ResetVisuals();
        }

        private void SetAcceptVisuals()
        {
            infoLabel.SetStyleColor(infoTextHighlightColor);
            typeLabel.SetStyleColor(acceptColor);
            this.SetStyleBorderColor(borderAcceptColor);
            overlay.SetStyleBackgroundColor(acceptColor);
        }

        private void SetRejectVisuals()
        {
            infoLabel.SetStyleColor(infoTextHighlightColor);
            typeLabel.SetStyleColor(rejectColor);
            this.SetStyleBorderColor(borderRejectColor);
            overlay.SetStyleBackgroundColor(rejectColor);
        }

        private void ResetVisuals()
        {
            infoLabel.SetStyleColor(textColor);
            typeLabel.SetStyleColor(textColor);
            this.SetStyleBorderColor(borderColor);
            overlay.SetStyleBackgroundColor(normalColor);
        }
    }
}
