// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Reactor.Components;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Components
{
    public class ToggleReactionControls : ReactionControls
    {
        public ToggleReactionControls AddToggleControls
        (
            UnityAction resetCallback,
            UnityAction onCallback,
            UnityAction offCallback,
            UnityAction searchForAnimatorsCallback = null
        )
        {
            VisualElement resetButtonContainer =
                DesignUtils.row
                    .SetName("Reset Button Container")
                    .SetStyleDisplay(EditorApplication.isPlayingOrWillChangePlaymode ? DisplayStyle.None : DisplayStyle.Flex)
                    .SetStyleFlexGrow(0)
                    .SetStyleAlignItems(Align.Center)
                    .AddChild(GetResetButton(resetCallback))
                    .AddSpaceBlock()
                    .AddChild(DesignUtils.dividerVertical)
                    .AddSpaceBlock(2);

            VisualElement editorOnlyContainer =
                DesignUtils.row
                    .SetName("Editor Only Container")
                    .SetStyleDisplay(EditorApplication.isPlayingOrWillChangePlaymode ? DisplayStyle.None : DisplayStyle.Flex)
                    .SetStyleFlexGrow(0)
                    .SetStyleAlignItems(Align.Center)
                    .AddSpaceBlock(2)
                    .AddChild(DesignUtils.dividerVertical)
                    .AddSpaceBlock()
                    ;
            
            if (searchForAnimatorsCallback != null)
            {
                editorOnlyContainer
                    .AddChild
                    (
                        //Search for Animators button
                        GetNewButton(EditorSpriteSheets.EditorUI.Icons.Refresh, "Search for Animators\nUse this after you've added a new animator")
                            .SetOnClick(searchForAnimatorsCallback)
                    )
                    .AddSpaceBlock();
            }
            
            return this
                .AddItem(resetButtonContainer)
                .AddIsOnButton(onCallback)
                .AddSpaceBlock()
                .AddIsOffButton(offCallback)
                .AddSpaceBlock2X()
                .AddItem(editorOnlyContainer)
                .AddFlexibleSpace()
                .AddItem(icon);
        }

        public ToggleReactionControls AddIsOnButton(UnityAction callback) =>
            this.AddItem
            (
                FluidButton.Get()
                    .SetLabelText("ON")
                    .SetTooltip("Is On")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.ToggleON)
                    .ClearOnClick()
                    .SetOnClick(callback)
            );

        public ToggleReactionControls AddIsOffButton(UnityAction callback) =>
            this.AddItem
            (
                FluidButton.Get()
                    .SetLabelText("OFF")
                    .SetTooltip("Is Off")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.ToggleOFF)
                    .ClearOnClick()
                    .SetOnClick(callback)
            );
    }
}
