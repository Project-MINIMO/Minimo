// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Interfaces;
using Doozy.Runtime.Colors;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable UnusedType.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
// ReSharper disable UnusedMember.Local

namespace Doozy.Editor.Dashboard.WindowsLayouts
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public partial class DashboardHomeWindowLayout : FluidWindowLayout, IDashboardWindowLayout
    {
        public bool isValid => homeSections.Count > 0;
        
        public int order => 0;

        public override string layoutName => "General";
        public sealed override List<Texture2D> animatedIconTextures => EditorSpriteSheets.EditorUI.Icons.Boards;

        public override Color accentColor => EditorColors.Default.UnityThemeInversed;
        public override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Default.UnityThemeInversed;

        private List<HomeSection> homeSections { get; set; }

        public DashboardHomeWindowLayout()
        {
            AddHeader("General", "Overview", animatedIconTextures);
            content
                .ResetLayout()
                .SetStylePadding(DesignUtils.k_Spacing)
                .SetStyleBackgroundImage(EditorTextures.Dashboard.Backgrounds.DashboardBackground)
                .SetStyleBackgroundScaleMode(ScaleMode.ScaleAndCrop)
                .SetStyleBackgroundImageTintColor(EditorColors.Default.MenuBackgroundLevel0);

            sideMenu.Dispose();

            Initialize();
            Compose();
        }

        private void Initialize()
        {
            //find all derived classed of HomeSection
            IEnumerable<Type> sectionTypes = ReflectionUtils.GetDerivedTypes(typeof(HomeSection));
            homeSections =
                sectionTypes
                    .Select(s => (HomeSection)Activator.CreateInstance(s))
                    .Where(s => s.isValid)
                    .OrderBy(s => s.sectionOrder)
                    .ThenBy(s => s.sectionName)
                    .ToList();
        }

        private void Compose()
        {
            //add sections
            VisualElement sectionContainer =
                DesignUtils.row
                    .SetStyleFlexGrow(0);

            if (homeSections.Count > 0)
            {
                foreach (HomeSection section in homeSections)
                    sectionContainer.AddChild(section);

                sectionContainer.AddFlexibleSpace();
                content.AddChild(sectionContainer);
            }

            content
                .AddFlexibleSpace();
        }

        private static VisualElement newSection =>
            new VisualElement()
                .SetStyleFlexShrink(0)
                .SetStylePadding(DesignUtils.k_Spacing2X)
                .SetStyleMargins(DesignUtils.k_Spacing / 2f)
                .SetStyleBackgroundColor(EditorColors.Default.BoxBackground.WithAlpha(0.9f))
                .SetStyleBorderRadius(DesignUtils.k_Spacing2X);


    }


}
