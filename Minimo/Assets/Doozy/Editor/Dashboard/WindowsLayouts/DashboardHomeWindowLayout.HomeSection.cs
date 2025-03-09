// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Colors;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.Dashboard.WindowsLayouts
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public partial class DashboardHomeWindowLayout
    {
        public class HomeSection : VisualElement
        {
            public virtual bool isValid => true;
            public virtual int sectionOrder => 0;
            public virtual string sectionName => "Unnamed Home Section";
            public Label title { get; private set; }
            private VisualElement titleDivider { get; set; }

            public HomeSection()
            {
                this
                    .SetStyleFlexShrink(0)
                    .SetStyleMinWidth(160)
                    .SetStylePadding(DesignUtils.k_Spacing2X)
                    .SetStyleMargins(DesignUtils.k_Spacing / 2f)
                    .SetStyleBackgroundColor(EditorColors.Default.BoxBackground.WithAlpha(0.9f))
                    .SetStyleBorderRadius(DesignUtils.k_Spacing2X);

                title =
                    DesignUtils.NewLabel()
                        .SetStyleFontSize(12)
                        .SetStyleUnityFontStyleAndWeight(FontStyle.Bold)
                        .SetStyleAlignSelf(Align.Center)
                        .SetStyleMarginBottom(DesignUtils.k_Spacing)
                        .SetStyleDisplay(DisplayStyle.None);

                titleDivider =
                    DesignUtils.dividerHorizontal
                        .SetStyleMarginBottom(DesignUtils.k_Spacing3X)
                        .SetStyleDisplay(DisplayStyle.None);
                
                this
                    .AddChild(title)
                    .AddChild(titleDivider);

                // ReSharper disable once VirtualMemberCallInConstructor
                this.SetTitle(sectionName);
            }

            public HomeSection SetTitle(string text = "")
            {
                var display = text.IsNullOrEmpty() ? DisplayStyle.None : DisplayStyle.Flex;
                
                title
                    .SetText(text)
                    .SetStyleDisplay(display);

                titleDivider
                    .SetStyleDisplay(display);
                    
                return this;
            }

            public HomeSection ClearTitle() =>
                SetTitle();
            
            public static Label TitleLabel(string text) =>
                DesignUtils.fieldLabel
                    .SetText(text)
                    .SetStyleColor(EditorColors.Default.TextTitle)
                    .SetStyleFontSize(10)
                    .SetStyleUnityFontStyleAndWeight(FontStyle.Bold)
                    .SetStyleMarginBottom(2);

            public static Label ValueLabel(string text) =>
                DesignUtils.fieldLabel
                    .SetText(text)
                    .SetStyleColor(EditorColors.Default.TextSubtitle)
                    .SetStyleFontSize(11);

        }
    }
}
