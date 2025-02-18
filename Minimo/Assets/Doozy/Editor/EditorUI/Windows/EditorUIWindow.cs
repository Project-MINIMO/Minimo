// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI.Windows.Internal;

namespace Doozy.Editor.EditorUI.Windows
{
    public class EditorUIWindow : FluidWindow<EditorUIWindow>
    {
        public const string k_WindowTitle = "EditorUI";
        public const string k_WindowMenuPath = "Tools/Doozy/Refresh/";

        public static void Open() => InternalOpenWindow(k_WindowTitle);
        protected override void CreateGUI()
        {
            //REMOVED    
        }
    }
}
