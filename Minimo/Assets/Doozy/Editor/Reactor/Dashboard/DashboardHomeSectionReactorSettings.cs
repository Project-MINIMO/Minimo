// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Dashboard.WindowsLayouts;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.Reactor.ScriptableObjects;
using Doozy.Runtime.UIElements.Extensions;

namespace Doozy.Editor.Reactor.Dashboard
{
    // ReSharper disable once UnusedType.Global
    public sealed class DashboardHomeSectionReactorSettings : DashboardHomeWindowLayout.HomeSection
    {
        public override int sectionOrder => 150;
        public override string sectionName => "Reactor Settings";

        public DashboardHomeSectionReactorSettings()
        {
            this
                .AddChild(TitleLabel("Editor Heartbeat"))
                .AddChild(ValueLabel(ObjectNames.NicifyVariableName(ReactorSettings.editorFPS.ToString()) + " FPS"))
                .AddSpaceBlock(3)
                .AddChild(TitleLabel("Runtime Heartbeat"))
                .AddChild(ValueLabel(ObjectNames.NicifyVariableName(ReactorSettings.runtimeFPS.ToString()) + " FPS"))
                ;
        }
    }
}
