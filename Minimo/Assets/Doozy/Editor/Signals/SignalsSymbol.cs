// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Common.Utils;
using UnityEditor;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Signals
{
    /// <summary>
    /// Specialized class that checks if the DOOZY_SIGNALS symbol is defined and if not, it adds it to the Scripting Define Symbols
    /// </summary>
    public static class SignalsSymbol
    {
        public const string k_Symbol = "DOOZY_SIGNALS";

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
            DefineSymbolsUtils.AddGlobalDefine(k_Symbol);
        }
    }

    /// <summary>
    /// Specialized class that gets called by Unity whenever an asset is deleted from the project
    /// This checks if the asset being deleted is Signals and if so, it removes the DOOZY_SIGNALS symbol from the Scripting Define Symbols
    /// </summary>
    public class SoundyAssetModificationProcessor : UnityEditor.AssetModificationProcessor
    {
        private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            bool deletingSoundy = assetPath.Contains($"{EditorPath.path}/Signals");
            if (deletingSoundy) DefineSymbolsUtils.RemoveGlobalDefine(SignalsSymbol.k_Symbol);
            return AssetDeleteResult.DidNotDelete;
        }
    }
}
