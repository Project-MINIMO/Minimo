// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Dashboard.WindowsLayouts;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.ScriptableObjects;

namespace Doozy.Editor.UIManager.Dashboard
{
    public sealed class DashboardHomeSectionInputSettings : DashboardHomeWindowLayout.HomeSection
    {
        public override int sectionOrder => 100;
        public override string sectionName => "Input Settings";
        
        public DashboardHomeSectionInputSettings()
        {
            this
                .AddChild(TitleLabel("Input Handling"))
                .AddChild(ValueLabel(ObjectNames.NicifyVariableName(UIManagerInputSettings.k_InputHandling.ToString())))
                .AddSpaceBlock(3)
                .AddChild(TitleLabel("Multiplayer Mode"))
                .AddChild(ValueLabel(UIManagerInputSettings.instance.multiplayerMode ? "Enabled" : "Disabled"))
                .AddSpaceBlock(3)
                .AddChild(TitleLabel("'Back' Button Cooldown"))
                .AddChild(ValueLabel(UIManagerInputSettings.instance.backButtonCooldown + " seconds"))
                ;
        }
    }
}
