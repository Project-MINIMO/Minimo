// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Dashboard.WindowsLayouts;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.UIElements.Extensions;

namespace Doozy.Editor.UIManager.UIMenu.Dashboard
{
    public class DashboardHomeQuickActionSection : DashboardHomeWindowLayout.QuickActionSection
    {
        public override int sectionOrder => 300;
        public override string sectionName => "UI Menu Quick Actions";

        public DashboardHomeQuickActionSection()
        {
            FluidButton uiMenuButton =
                NormalButton
                (
                    "Open the UIMenu window",
                    EditorSpriteSheets.EditorUI.Icons.UIMenu,
                    EditorSelectableColors.Default.UnityThemeInversed,
                    UIMenuWindow.Open
                );

            FluidButton refreshButton =
                TinyButton
                (
                    "Refresh the UIMenu by searching for all UIMenuItem assets in the project and updating the database",
                    refreshIcon,
                    EditorSelectableColors.Default.UnityThemeInversed,
                    UIMenuWindow.RefreshDatabase
                );

            this
                .AddChild(uiMenuButton)
                .AddFlexibleSpace()
                .AddSpaceBlock()
                .AddChild(DesignUtils.dividerVertical)
                .AddSpaceBlock()
                .AddChild(refreshButton);
        }
    }
}
