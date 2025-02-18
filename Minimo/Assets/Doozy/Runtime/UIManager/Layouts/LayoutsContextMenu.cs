// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Common.Utils;
using UnityEngine.UI;

namespace Doozy.Runtime.UIManager.Layouts
{
    public static class LayoutsContextMenu
    {
        #if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/UI/Layouts/HorizontalLayout", false, 8)]
        private static void HorizontalLayout(UnityEditor.MenuCommand menuCommand)
        {
            GameObjectUtils.AddToScene<HorizontalLayoutGroup>("HorizontalLayout", false, true);
        }
        
        [UnityEditor.MenuItem("GameObject/UI/Layouts/VerticalLayout", false, 8)]
        private static void VerticalLayout(UnityEditor.MenuCommand menuCommand)
        {
            GameObjectUtils.AddToScene<VerticalLayoutGroup>("VerticalLayout", false, true);
        }
        
        [UnityEditor.MenuItem("GameObject/UI/Layouts/GridLayout", false, 8)]
        private static void GridLayout(UnityEditor.MenuCommand menuCommand)
        {
            GameObjectUtils.AddToScene<GridLayoutGroup>("GridLayout", false, true);
        }
        #endif
    }
}
