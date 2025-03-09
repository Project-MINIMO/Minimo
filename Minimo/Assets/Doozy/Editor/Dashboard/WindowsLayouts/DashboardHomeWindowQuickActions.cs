// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms


using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.UIElements.Extensions;

namespace Doozy.Editor.Dashboard.WindowsLayouts
{
    public class DashboardHomeWindowQuickActions : DashboardHomeWindowLayout.HomeSection
    {
        public override bool isValid => quickActionSections.Count > 0;
        public override int sectionOrder => 0;
        public override string sectionName => "Quick Actions";

        private List<DashboardHomeWindowLayout.QuickActionSection> quickActionSections { get; set; }

        public DashboardHomeWindowQuickActions()
        {
            //find all derived classed of QuickActionSection
            IEnumerable<Type> sectionTypes = ReflectionUtils.GetDerivedTypes(typeof(DashboardHomeWindowLayout.QuickActionSection));
            quickActionSections =
                sectionTypes
                    .Select(s => (DashboardHomeWindowLayout.QuickActionSection)Activator.CreateInstance(s))
                    .OrderBy(s => s.sectionOrder)
                    .ThenBy(s => s.sectionName)
                    .ToList();

            foreach (DashboardHomeWindowLayout.QuickActionSection section in quickActionSections)
                this.AddChild(section);
        }
    }
}
