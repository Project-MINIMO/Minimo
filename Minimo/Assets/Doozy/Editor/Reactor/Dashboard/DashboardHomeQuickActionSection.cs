// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Dashboard.WindowsLayouts;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Reactor.Windows;
using Doozy.Runtime.UIElements.Extensions;

namespace Doozy.Editor.Reactor.Dashboard
{
    public class DashboardHomeQuickActionSection : DashboardHomeWindowLayout.QuickActionSection
    {
        public override int sectionOrder => 400;
        public override string sectionName => "Reactor Quick Actions";

        public DashboardHomeQuickActionSection()
        {
            FluidButton editorHeartbeatButton =
                NormalButton
                (
                    "Editor Heartbeat",
                    EditorSpriteSheets.Reactor.Icons.EditorHeartbeat,
                    EditorSelectableColors.Reactor.Red,
                    ReactorEditorTickerWindow.Open
                );

            FluidButton runtimeHeartbeatButton =
                NormalButton
                (
                    "Runtime Heartbeat",
                    EditorSpriteSheets.Reactor.Icons.Heartbeat,
                    EditorSelectableColors.Reactor.Red,
                    ReactorRuntimeTickerWindow.Open
                );

            FluidButton refreshButton =
                TinyButton
                (
                    "Refresh Reactor by searching for all animation presets in the project and adding them to the database",
                    refreshIcon,
                    EditorSelectableColors.Reactor.Red,
                    ReactorWindow.RefreshDatabase
                );

            this
                .AddChild(editorHeartbeatButton)
                .AddSpaceBlock()
                .AddChild(runtimeHeartbeatButton)
                .AddFlexibleSpace()
                .AddSpaceBlock()
                .AddChild(DesignUtils.dividerVertical)
                .AddSpaceBlock()
                .AddChild(refreshButton);
        }
    }
}
