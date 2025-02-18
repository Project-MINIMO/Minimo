// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Dashboard.WindowsLayouts;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.UIElements.Extensions;

namespace Doozy.Editor.Nody.Dashboard
{
    public sealed class DashboardHomeQuickActionSection : DashboardHomeWindowLayout.QuickActionSection
    {
        public override int sectionOrder => 100;
        public override string sectionName => "Nody Quick Actions";

        public DashboardHomeQuickActionSection()
        {
            FluidButton nodyButton =
                NormalButton
                (
                    "Open the Nody window",
                    EditorSpriteSheets.Nody.Icons.Nody,
                    EditorSelectableColors.Nody.Color,
                    NodyWindow.Open
                );

            FluidButton newNodeButton =
                SmallButton
                (
                    "Open the Nody Create Node window",
                    EditorSpriteSheets.EditorUI.Icons.Plus,
                    EditorSelectableColors.Nody.Color,
                    NodyCreateNodeWindow.Open
                );

            FluidButton refreshButton =
                TinyButton
                (
                    "Refresh Nody by searching for all Nody node types in the project and updating the Nody window search menu",
                    refreshIcon,
                    EditorSelectableColors.Nody.Color,
                    NodyWindow.Refresh
                );

            this
                .AddChild(nodyButton)
                .AddSpaceBlock()
                .AddChild(newNodeButton)
                .AddFlexibleSpace()
                .AddSpaceBlock()
                .AddChild(DesignUtils.dividerVertical)
                .AddSpaceBlock()
                .AddChild(refreshButton);
        }
    }
}
