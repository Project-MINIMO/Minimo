// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Common.Utils;
using Doozy.Runtime.UIManager.ScriptableObjects;
using UnityEditor;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable CommentTypo

namespace Doozy.Editor.UIManager
{
    /// <summary>
    /// Specialized class that checks if the UIManager symbol is defined and if not, it adds it to the Scripting Define Symbols
    /// </summary>
    public static class UIManagerSymbol
    {
        [InitializeOnLoadMethod]
        public static void Initialize()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                DelayedCall.Run(2f, Initialize);
                return;
            }
            Run();
        }
        
        public static void Run() 
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                DelayedCall.Run(2f, Run);
                return;
            }
            DefineSymbolsUtils.AddGlobalDefine(UIManagerSettings.k_UIManagerSymbol);
        }
    }
    
    /// <summary>
    /// Specialized class that gets called by Unity whenever an asset is deleted from the project
    /// This checks if the asset being deleted is UIManager and if so, it removes the DOOZY_UIMANAGER symbol from the Scripting Define Symbols
    /// </summary>
    public class UIManagerAssetModificationProcessor : UnityEditor.AssetModificationProcessor
    {
        private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            bool deletingSoundy = assetPath.Contains($"{EditorPath.path}/UIManager");
            if (deletingSoundy) DefineSymbolsUtils.RemoveGlobalDefine(UIManagerSettings.k_UIManagerSymbol);
            return AssetDeleteResult.DidNotDelete;
        }
    }
}
