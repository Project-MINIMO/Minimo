// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Dashboard.WindowsLayouts;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.UIDesigner.Windows;
using Doozy.Runtime.UIElements.Extensions;

namespace Doozy.Editor.UIDesigner.Dashboard
{
    public class DashboardHomeQuickActionSection : DashboardHomeWindowLayout.QuickActionSection
    {
        public override int sectionOrder => 500;
        public override string sectionName => "UI Designer Quick Actions";

        public DashboardHomeQuickActionSection()
        {
            FluidButton alignButton =
                NormalButton
                (
                    "Open the UIDesigner Align Window",
                    EditorSpriteSheets.UIDesigner.Icons.HorizontalAlignLeft,
                    EditorSelectableColors.UIDesigner.Color,
                    AlignWindow.Open
                );

            FluidButton rotateButton =
                NormalButton
                (
                    "Open the UIDesigner Rotate Window",
                    EditorSpriteSheets.UIDesigner.Icons.Rotate,
                    EditorSelectableColors.UIDesigner.Color,
                    RotateWindow.Open
                );

            FluidButton scaleButton =
                NormalButton
                (
                    "Open the UIDesigner Scale Window",
                    EditorSpriteSheets.UIDesigner.Icons.ScaleIncrease,
                    EditorSelectableColors.UIDesigner.Color,
                    ScaleWindow.Open
                );

            FluidButton sizeButton =
                NormalButton
                (
                    "Open the UIDesigner Size Window",
                    EditorSpriteSheets.UIDesigner.Icons.SizeIncrease,
                    EditorSelectableColors.UIDesigner.Color,
                    SizeWindow.Open
                );

            this
                .AddChild(alignButton)
                .AddSpaceBlock()
                .AddChild(rotateButton)
                .AddSpaceBlock()
                .AddChild(scaleButton)
                .AddSpaceBlock()
                .AddChild(sizeButton);
        }
    }
}
