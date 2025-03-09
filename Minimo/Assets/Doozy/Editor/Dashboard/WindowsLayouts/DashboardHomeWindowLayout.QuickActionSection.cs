// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBeProtected.Global

namespace Doozy.Editor.Dashboard.WindowsLayouts
{
    public partial class DashboardHomeWindowLayout
    {
        public class QuickActionSection : VisualElement
        {
            public virtual int sectionOrder => 0;
            public virtual string sectionName => "Unnamed Quick Action Section";

            public QuickActionSection()
            {
                this
                    .SetStyleFlexDirection(FlexDirection.Row)
                    .SetStyleFlexShrink(0)
                    .SetStyleFlexGrow(0)
                    .SetStyleAlignItems(Align.Center)
                    .SetStylePadding(DesignUtils.k_Spacing2X)
                    .SetStyleMargins(DesignUtils.k_Spacing / 2f)
                    .SetStyleBackgroundColor(EditorColors.Default.WindowHeaderIcon)
                    .SetStyleBorderRadius(DesignUtils.k_Spacing2X)
                    ;
            }

            public static IEnumerable<Texture2D> refreshIcon => EditorSpriteSheets.EditorUI.Icons.Refresh;

            public static FluidButton NormalButton
            (
                string tooltipText,
                IEnumerable<Texture2D> icon,
                EditorSelectableColorInfo accentColor,
                UnityAction onClick
            ) =>
                FluidButton.Get()
                    .SetTooltip(tooltipText)
                    .SetIcon(icon)
                    .SetAccentColor(accentColor)
                    .SetOnClick(onClick)
                    .SetStyleFlexShrink(0)
                    .SetButtonStyle(ButtonStyle.Contained);

            public static FluidButton SmallButton
            (
                string tooltipText,
                IEnumerable<Texture2D> icon,
                EditorSelectableColorInfo accentColor,
                UnityAction onClick
            ) =>
                NormalButton(tooltipText, icon, accentColor, onClick)
                    .SetElementSize(ElementSize.Small);

            public static FluidButton TinyButton
            (
                string tooltipText,
                IEnumerable<Texture2D> icon,
                EditorSelectableColorInfo accentColor,
                UnityAction onClick
            ) =>
                NormalButton(tooltipText, icon, accentColor, onClick)
                    .SetElementSize(ElementSize.Tiny);

        }
    }
}
