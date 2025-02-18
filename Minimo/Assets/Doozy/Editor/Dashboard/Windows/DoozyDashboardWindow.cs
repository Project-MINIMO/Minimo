// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.EditorUI.Windows.Internal;
using Doozy.Editor.Interfaces;
using Doozy.Editor.Reactor.Internal;
using Doozy.Editor.UIElements;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor.Extensions;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Dashboard.Windows
{
    public class DoozyDashboardWindow : FluidWindow<DoozyDashboardWindow>
    {
        private const string k_WindowTitle = "Dashboard";
        public const string WINDOW_MENU_PATH = "Tools/Doozy/Dashboard";

        [MenuItem(WINDOW_MENU_PATH, priority = -2000)]
        public static void Open() => InternalOpenWindow(k_WindowTitle);

        private TemplateContainer templateContainer { get; set; }
        private VisualElement layoutContainer { get; set; }
        private VisualElement sideMenuContainer { get; set; }
        private VisualElement contentContainer { get; set; }

        private const float k_MaxIconSize = 64f;
        private const float k_MinIconSize = 32f;
        private const int k_TitleFontSize = 18;
        private const int k_SubtitleFontSize = 14;

        internal string selectedTab { get; set; }
        private string selectedTabKey => EditorPrefsKey($"{nameof(selectedTab)}");

        private static IEnumerable<Texture2D> dashboardIconTextures => EditorSpriteSheets.EditorUI.Icons.Dashboard;
        private Image dashboardIconImage { get; set; }
        private VisualElement dashboardTitleContainer { get; set; }
        private Label dashboardTitleLabel { get; set; }
        private Label dashboardSubtitleLabel { get; set; }

        private ScrollView contentScrollView { get; set; }
        private VisualElement itemsContainer { get; set; }

        private FluidSideMenu sideMenu { get; set; }
        private string sideMenuWidthKey => EditorPrefsKey($"{nameof(sideMenu)}.{nameof(sideMenu.customWidth)}");
        private string sideMenuIsCollapsedKey => EditorPrefsKey($"{nameof(sideMenu)}.{nameof(sideMenu.isCollapsed)}");
        private FluidResizer sideMenuResizer { get; set; }

        private VisualElement footerContainer { get; set; }
        private FluidPlaceholder emptyPlaceholder { get; set; }

        // protected override void OnEnable()
        // {
        //     base.OnEnable();
        //     minSize = new Vector2(1200, 673);
        // }

        protected override void CreateGUI()
        {
            Initialize();
            Compose();
        }

        private void Initialize()
        {
            root
                .RecycleAndClear()
                .Add(templateContainer = EditorLayouts.Dashboard.DashboardWindow.CloneTree());

            templateContainer
                .SetStyleFlexGrow(1)
                .AddStyle(EditorUI.EditorStyles.Dashboard.DashboardWindow);

            layoutContainer = templateContainer.Q<VisualElement>(nameof(layoutContainer));
            sideMenuContainer = layoutContainer.Q<VisualElement>(nameof(sideMenuContainer));
            contentContainer = layoutContainer.Q<VisualElement>(nameof(contentContainer));

            InitializeDoozyIcon();
            InitializeSideMenu();
            InitializeContent();
            InitializeFooter();
        }

        private void InitializeDoozyIcon()
        {
            dashboardTitleLabel =
                DesignUtils.NewLabel("Dashboard")
                    .SetStyleColor(EditorColors.Default.TextTitle)
                    .SetStyleFontSize(k_TitleFontSize);

            dashboardSubtitleLabel =
                DesignUtils.NewLabel("Hub")
                    .SetStyleColor(EditorColors.Default.TextSubtitle)
                    .SetStyleFontSize(k_SubtitleFontSize);

            dashboardIconImage =
                new Image()
                    .SetStyleFlexShrink(0)
                    .SetStyleMargins(DesignUtils.k_Spacing)
                    .SetStyleAlignSelf(Align.Center);

            Texture2DReaction reaction =
                dashboardIconImage
                    .GetTexture2DReaction(dashboardIconTextures)
                    .SetEditorHeartbeat();

            root.schedule.Execute(() =>
            {
                UpdateDoozyIconSize(EditorPrefs.GetBool(sideMenuIsCollapsedKey, false) ? k_MinIconSize : sideMenu.customWidth);
                
            });

            dashboardIconImage.RegisterCallback<PointerEnterEvent>(evt => reaction?.Play());
            dashboardIconImage.AddManipulator(new Clickable(() => reaction?.Play()));
        }

        private void UpdateDoozyIconSize(float size)
        {
            size = Mathf.Max(k_MinIconSize, size);
            size = Mathf.Min(k_MaxIconSize, size);
            dashboardIconImage.SetStyleSize(size);
        }

        private void InitializeSideMenu()
        {
            //setup side menu
            sideMenu =
                new FluidSideMenu()
                    .SetMenuLevel(FluidSideMenu.MenuLevel.Level_0)
                    .IsCollapsable(true)
                    .SetCustomWidth(EditorPrefs.GetInt(sideMenuWidthKey, 200));

            //link expand/collapse reaction to affect the doozy icon size
            FloatReaction titleFontSizeReaction =
                Reaction.Get<FloatReaction>()
                    .SetEditorHeartbeat()
                    .SetDuration(FluidSideMenu.EXPAND_COLLAPSE_DURATION * 0.5f)
                    .SetEase(FluidSideMenu.EXPAND_COLLAPSE_EASE);

            sideMenu.expandCollapseReaction.AddOnUpdateCallback(() =>
            {
                float value = sideMenu.expandCollapseReaction.currentValue;
                dashboardTitleContainer.SetStyleOpacity(value);
                dashboardIconImage.SetStyleLeft(DesignUtils.k_Spacing * (1 - value));
            });

            sideMenu.OnCollapse += () => EditorPrefs.SetBool(sideMenuIsCollapsedKey, true);
            sideMenu.OnExpand += () => EditorPrefs.SetBool(sideMenuIsCollapsedKey, false);

            bool sideMenuIsCollapsed = EditorPrefs.GetBool(sideMenuIsCollapsedKey, false);
            //update side menu collapse state
            if (sideMenu.isCollapsable) sideMenu.ToggleMenu(!sideMenuIsCollapsed, false);

            //add doozy icon to the side menu header
            dashboardTitleContainer =
                DesignUtils.column
                    .AddChild(dashboardTitleLabel)
                    .AddChild(dashboardSubtitleLabel);

            if (sideMenuIsCollapsed)
            {
                dashboardTitleContainer.SetStyleOpacity(0f);
                dashboardIconImage.SetStyleLeft(DesignUtils.k_Spacing);
            }
            
            sideMenu.headerContainer
                .SetStyleDisplay(DisplayStyle.Flex)
                .AddChild
                (
                    DesignUtils.row
                        .SetStyleOverflow(Overflow.Hidden)
                        .SetStyleAlignSelf(Align.Center)
                        .SetStyleAlignItems(Align.Center)
                        .AddChild(dashboardIconImage)
                        .AddChild(dashboardTitleContainer)
                );

            //connect the doozy icon size to the side menu expand/collapse reaction
            sideMenu.expandCollapseReaction.AddOnUpdateCallback(() =>
            {
                float currentValue = sideMenu.expandCollapseReaction.currentValue;
                float size = Mathf.LerpUnclamped(k_MinIconSize, sideMenu.customWidth, currentValue);
                UpdateDoozyIconSize(size);
            });

            //setup side menu resizer
            sideMenuResizer = new FluidResizer(FluidResizer.Position.Right);
            sideMenuResizer.onPointerMoveEvent += evt =>
            {
                if (sideMenu.isCollapsed) return;
                int delta = (int)(sideMenu.customWidth + evt.deltaPosition.x);
                sideMenu.SetCustomWidth(delta);
                UpdateDoozyIconSize(delta);
            };
            sideMenuResizer.onPointerUp += evt =>
            {
                if (sideMenu.isCollapsed) return;
                EditorPrefs.SetInt(sideMenuWidthKey, sideMenu.customWidth);
            };

            //add the menu and the resizer to the side menu container
            sideMenuContainer.Add
            (
                DesignUtils.row
                    .AddChild(sideMenu)
                    .AddChild(sideMenuResizer)
            );

            //get all the types that implement the IDashboardWindowLayout interface
            //they are used to generate the side menu buttons and to get/display the corresponding content
            IEnumerable<IDashboardWindowLayout> layouts =
                TypeCache.GetTypesDerivedFrom(typeof(IDashboardWindowLayout))               //get all the types that derive from IDashboardWindowLayout
                    .Select(type => (IDashboardWindowLayout)Activator.CreateInstance(type)) //create an instance of the type
                    .OrderBy(layout => layout.order)                                        //sort the layouts by order (set in each layout's class)
                    .ThenBy(layout => layout.layoutName);                                   //sort the layouts by name (set in each layout's class)


            //get the previously selected layout name
            string previouslySelectedLayoutName = EditorPrefs.GetString(selectedTabKey, string.Empty);
            //the previously selected tab reference
            FluidToggleButtonTab previouslySelectedTab = null;
            //order indicator used to add spacing between the tabs, when the difference is greater or equal to 50
            int previousOrder = -1;

            //add buttons to side menu
            foreach (IDashboardWindowLayout l in layouts)
            {
                //VALIDATE LAYOUT - check if the layout is valid and can be added to the side menu
                if (!l.isValid) continue; //if the layout is not valid, skip it

                //INJECT SPACE
                if (l.order > 0 && l.order - previousOrder >= 50) //if the layout order difference is greater or equal than 50
                    sideMenu.AddSpaceBetweenButtons();            //add a vertical space between side menu buttons
                previousOrder = l.order;                          //keep track of the previous layout order

                //SIDE MENU BUTTON
                FluidToggleButtonTab sideMenuButton = sideMenu.AddButton(l.layoutName, l.selectableAccentColor);

                if (!previouslySelectedLayoutName.IsNullOrEmpty() &&   //if a layout was previously selected
                    l.layoutName.Equals(previouslySelectedLayoutName)) //and the current layout is the same as the previously selected layout
                    previouslySelectedTab = sideMenuButton;            //set the previously selected tab to the current one

                //ADD SIDE MENU BUTTON ICON (animated or static)
                if (l.animatedIconTextures?.Count > 0)
                    sideMenuButton.SetIcon(l.animatedIconTextures); // <<< ANIMATED ICON
                else if (l.staticIconTexture != null)
                    sideMenuButton.SetIcon(l.staticIconTexture); // <<< STATIC ICON

                //WINDOW LAYOUT (added to the content container when the button is pressed)                
                VisualElement customWindowLayout = ((VisualElement)l).SetStyleFlexGrow(1);

                sideMenuButton.SetToggleAccentColor(((IDashboardWindowLayout)customWindowLayout).selectableAccentColor);

                sideMenuButton.name = l.layoutName;
                
                //SIDE MENU BUTTON - ON VALUE CHANGED
                //show the appropriate window layout when the value of the side menu button changes
                sideMenuButton.OnValueChanged += evt =>
                {
                    if (!evt.newValue) return;
                    contentContainer.Clear();
                    contentContainer.Add(customWindowLayout);
                    EditorPrefs.SetString(selectedTabKey, l.layoutName);
                    selectedTab = l.layoutName;
                };
            }

            //select the previously selected tab (if any)
            root.schedule.Execute(() => previouslySelectedTab?.SetIsOn(true));
        }

        private void InitializeContent()
        {
            contentScrollView = new ScrollView().SetStyleFlexGrow(1);
            contentContainer
                .AddChild(contentScrollView);
        }

        private void InitializeFooter()
        {
            footerContainer =
                DesignUtils.row
                    .SetStyleFlexShrink(0)
                    .SetStyleFlexGrow(0)
                    .SetStyleAlignItems(Align.Center)
                    .SetStyleBackgroundColor(EditorColors.Default.BoxBackground)
                    .SetStylePaddingLeft(DesignUtils.k_Spacing2X)
                    .SetStylePaddingRight(DesignUtils.k_Spacing2X)
                    .SetStylePaddingTop(DesignUtils.k_Spacing)
                    .SetStylePaddingBottom(DesignUtils.k_Spacing)
                ;

            FluidButton GetSocialButton
            (
                List<Texture2D> iconTextures,
                EditorSelectableColorInfo selectableColorInfo,
                string tooltip,
                string url
            )
                => FluidButton.Get()
                    .SetStyleFlexShrink(0)
                    .SetIcon(iconTextures)
                    .SetAccentColor(selectableColorInfo)
                    .SetTooltip(tooltip)
                    .SetOnClick(() => Application.OpenURL(url));

            var youtubeButton =
                GetSocialButton
                (
                    EditorSpriteSheets.EditorUI.Icons.Youtube,
                    EditorSelectableColors.Brands.YouTube,
                    "YouTube",
                    "https://youtube.doozyui.com"
                );

            var twitterButton =
                GetSocialButton
                (
                    EditorSpriteSheets.EditorUI.Icons.Twitter,
                    EditorSelectableColors.Brands.Twitter,
                    "Twitter",
                    "https://twitter.doozyui.com"
                );

            var facebookButton =
                GetSocialButton
                (
                    EditorSpriteSheets.EditorUI.Icons.Facebook,
                    EditorSelectableColors.Brands.Facebook,
                    "Facebook",
                    "https://facebook.doozyui.com"
                );

            var discordButton =
                GetSocialButton
                (
                    EditorSpriteSheets.EditorUI.Icons.Discord,
                    EditorSelectableColors.Brands.Discord,
                    "Discord",
                    "https://discord.doozyui.com"
                );

            var doozyWebsiteButton =
                FluidButton.Get("doozyui.com")
                    .SetStyleFlexShrink(0)
                    .SetOnClick(() => Application.OpenURL("https://doozyui.com"));

            bool proVersionExists = Directory.Exists($"{EditorPath.path}/UIManager/Pro");
            if (proVersionExists)
            {
                Image proImage =
                    new Image()
                        .SetStyleBackgroundImage(EditorTextures.EditorUI.Icons.Pro)
                        .SetStyleBackgroundImageTintColor(EditorColors.Default.Icon)
                        .SetStyleBackgroundScaleMode(ScaleMode.ScaleAndCrop)
                        .SetStyleSize(32);

                VisualElement proContainer =
                    new VisualElement()
                        .SetStylePosition(Position.Absolute)
                        .SetStyleLeft(0)
                        .SetStyleRight(0)
                        .SetStyleTop(0)
                        .SetStyleBottom(0)
                        .SetStyleAlignItems(Align.Center)
                        .SetStyleJustifyContent(Justify.Center)
                        .AddChild(proImage);
                
                footerContainer
                    .AddChild(proContainer);
            }

            footerContainer
                .AddChild(youtubeButton)
                .AddChild(twitterButton)
                .AddChild(facebookButton)
                .AddChild(discordButton)
                .AddFlexibleSpace()
                .AddChild(doozyWebsiteButton);
        }

        private void Compose()
        {
            root.AddChild(footerContainer);
        }
    }
}
