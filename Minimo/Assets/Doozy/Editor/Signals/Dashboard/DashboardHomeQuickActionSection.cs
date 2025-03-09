// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Dashboard.WindowsLayouts;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Signals.Windows;
using Doozy.Runtime.UIElements.Extensions;

namespace Doozy.Editor.Signals.Dashboard
{
    public class DashboardHomeQuickActionSection : DashboardHomeWindowLayout.QuickActionSection
    {
        public override int sectionOrder => 200;
        public override string sectionName => "Signals Quick Actions";

        public DashboardHomeQuickActionSection()
        {
            FluidButton signalsConsoleButton =
                NormalButton
                (
                    "Open the Signals Console Window",
                    EditorSpriteSheets.Signals.Icons.Signal,
                    EditorSelectableColors.Signals.Signal,
                    SignalsConsoleWindow.Open
                );

            FluidButton streamsConsoleButton =
                NormalButton
                (
                    "Open the Streams Console Window",
                    EditorSpriteSheets.Signals.Icons.SignalStream,
                    EditorSelectableColors.Signals.Stream,
                    StreamsConsoleWindow.Open
                );
            
            FluidButton refreshButton =
                TinyButton
                (
                    "Refresh Signals providers by searching for all provider types in the project and adding them to the system",
                    refreshIcon,
                    EditorSelectableColors.Signals.Signal,
                    SignalsWindow.RefreshProviders
                );
            
            
            this
                .AddChild(signalsConsoleButton)
                .AddSpaceBlock()
                .AddChild(streamsConsoleButton)
                .AddFlexibleSpace()
                .AddSpaceBlock()
                .AddChild(DesignUtils.dividerVertical)
                .AddSpaceBlock()
                .AddChild(refreshButton);

        }
    }
}
