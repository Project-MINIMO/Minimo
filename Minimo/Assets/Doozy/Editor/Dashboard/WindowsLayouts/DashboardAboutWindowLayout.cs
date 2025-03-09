// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.Common;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Interfaces;
using Doozy.Editor.UIElements;
using Doozy.Runtime.Colors;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.Dashboard.WindowsLayouts
{
    public class DashboardAboutWindowLayout : FluidWindowLayout, IDashboardWindowLayout
    {
        public bool isValid => true;

        public int order => 1110;

        public override string layoutName => "About";
        public sealed override List<Texture2D> animatedIconTextures => EditorSpriteSheets.EditorUI.Icons.Info;

        public override Color accentColor => EditorColors.Default.UnityThemeInversed;
        public override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Default.UnityThemeInversed;

        private ScrollView contentScrollView { get; set; }

        public DashboardAboutWindowLayout()
        {
            AddHeader("About", "Version Info", animatedIconTextures);
            content
                .ResetLayout()
                .SetStylePadding(DesignUtils.k_Spacing)
                .SetStyleBackgroundImage(EditorTextures.Dashboard.Backgrounds.DashboardBackground)
                .SetStyleBackgroundScaleMode(ScaleMode.ScaleAndCrop)
                .SetStyleBackgroundImageTintColor(EditorColors.Default.MenuBackgroundLevel0);

            sideMenu.Dispose();
            Initialize();
            Compose();
        }

        private void Initialize()
        {
            InitializeVersions();
        }

        private void InitializeVersions()
        {
            //get all the ProductInfo assets from the project
            ProductInfo[] infoArray = AssetDatabase.FindAssets("t:ProductInfo")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<ProductInfo>)
                .Where(info => info != null && !info.Name.IsNullOrEmpty()) //exclude null and empty names
                .ToArray();

            //look for UI Manager and UI Manager PRO, if they are both present, remove the non PRO version from the list
            ProductInfo uiManager = infoArray.FirstOrDefault(info => info.Name.RemoveWhitespaces().Equals("UIManager"));
            ProductInfo uiManagerPro = infoArray.FirstOrDefault(info => info.Name.RemoveWhitespaces().Equals("UIManagerPRO"));
            if (uiManager != null && uiManagerPro != null)
            {
                var list = infoArray.ToList();
                list.Remove(uiManager);
                infoArray = list.ToArray();
            }
            
            contentScrollView ??= new ScrollView();
            contentScrollView
                .contentContainer
                .SetStyleFlexDirection(FlexDirection.Row)
                .SetStyleAlignItems(Align.FlexStart)
                .SetStyleFlexWrap(Wrap.Wrap)
                .RecycleAndClear();

            foreach (ProductInfo info in infoArray)
                contentScrollView.contentContainer.AddChild(new ProductInfoElement(info));
        }

        private void Compose()
        {
            content
                .AddChild(contentScrollView)
                .AddFlexibleSpace()
                ;
        }

        private static string GetVersion(ProductInfo productInfo) =>
            $"{productInfo.Major}.{productInfo.Minor}.{productInfo.RevisionVersion}";

        private class ProductInfoElement : VisualElement
        {
            private readonly ProductInfo m_Info;
            private Label productNameLabel { get; set; }
            private Label productVersionLabel { get; set; }
            private Image productIconImage { get; set; }

            private Image proBadgeImage { get; set; }

            public ProductInfoElement(ProductInfo info)
            {
                m_Info = info;
                Initialize();
                Compose();
            }

            private void Initialize()
            {
                productNameLabel =
                    DesignUtils.fieldLabel
                        .SetText(m_Info.Name.Replace(" PRO", ""))
                        .SetStyleFontSize(13)
                        .SetStyleColor(EditorColors.Default.TextTitle);

                productVersionLabel =
                    DesignUtils.fieldLabel
                        .SetText(GetVersion(m_Info));

                productIconImage =
                    new Image()
                        .SetStyleSize(32)
                        .SetStyleAlignSelf(Align.Center)
                        .SetStyleMarginRight(DesignUtils.k_Spacing2X)
                        .SetStyleDisplay(DisplayStyle.None);

                proBadgeImage = new Image().SetStyleDisplay(DisplayStyle.None);
                if (m_Info.Name.RemoveWhitespaces().Equals("UIManagerPRO"))
                {
                    proBadgeImage
                        .SetStyleSize(24)
                        .SetStyleMarginTop(-6)
                        .SetStyleMarginBottom(-6)
                        .SetStyleAlignSelf(Align.Center)
                        .SetStyleMarginLeft(DesignUtils.k_Spacing2X)
                        .SetStyleBackgroundImage(EditorTextures.EditorUI.Icons.Pro)
                        .SetStyleBackgroundScaleMode(ScaleMode.ScaleAndCrop)
                        .SetStyleBackgroundImageTintColor(EditorColors.Default.UnityThemeInversed)
                        .SetStyleDisplay(DisplayStyle.Flex);
                }

                //search for the product name in the logos textures
                foreach (string textureName in Enum.GetNames(typeof(EditorTextures.EditorUI.Logos.TextureName)))
                {
                    if (!textureName.Equals(m_Info.Name.Replace(" PRO", "").RemoveWhitespaces()))
                        continue;
                    var enumValue = (EditorTextures.EditorUI.Logos.TextureName)Enum.Parse(typeof(EditorTextures.EditorUI.Logos.TextureName), textureName);
                    Texture2D iconTexture = EditorTextures.EditorUI.Logos.GetTexture2D(enumValue);
                    productIconImage.SetStyleBackgroundImage(iconTexture);
                    productIconImage.SetStyleDisplay(iconTexture == null ? DisplayStyle.None : DisplayStyle.Flex);
                }
            }

            private void Compose()
            {
                this
                    .SetStyleFlexDirection(FlexDirection.Column)
                    .SetStyleFlexShrink(0)
                    .SetStyleFlexGrow(1)
                    .SetStyleMinWidth(200)
                    .SetStylePadding(DesignUtils.k_Spacing2X)
                    .SetStyleMargins(DesignUtils.k_Spacing / 2f)
                    .SetStyleBackgroundColor(EditorColors.Default.BoxBackground.WithAlpha(0.9f))
                    .SetStyleBorderRadius(DesignUtils.k_Spacing2X);

                this
                    .AddChild
                    (
                        DesignUtils.row
                            .SetStyleFlexShrink(0)
                            .SetStyleFlexGrow(0)
                            .SetStyleAlignItems(Align.Center)
                            .AddChild(productIconImage)
                            .AddChild
                            (
                                DesignUtils.column
                                    .SetStyleFlexShrink(0)
                                    .SetStyleFlexGrow(0)
                                    .AddChild
                                    (
                                        DesignUtils.row
                                            .SetStyleAlignItems(Align.Center)
                                            .AddChild(productNameLabel)
                                            .AddChild(proBadgeImage)
                                    )
                                    .AddSpaceBlock()
                                    .AddChild(productVersionLabel)
                            )
                    );

            }
        }
    }
}
