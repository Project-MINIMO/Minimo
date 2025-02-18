// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Linq;
using Doozy.Editor.Common.Automation.Generators;
using Doozy.Editor.Common.Utils;
using Doozy.Runtime.Common.Attributes;
using UnityEditor;
using UnityEngine;
using DefineSymbolsUtils = Doozy.Runtime.Common.Attributes.DefineSymbolsUtils;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Common.ScriptableObjects
{
    public class UpdateBot : SingletonEditorScriptableObject<UpdateBot>
    {
        public bool Update;

        [InitializeOnLoadMethod]
        public static void Initialize()
        {
            if (!instance.Update)
                return;

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
            
            if(instance == null)
            {
                // Debug.Log($"[{nameof(UpdateBot)}] Update Bot instance is null");
                return;
            }
            
            #if DOOZY_42
            Debug.Log($"[{nameof(UpdateBot)}] Update Bot cannot automatically execute if 42 is enabled");
            return;
            #endif

            #pragma warning disable CS0162
            RemoveOldFiles();
            GlobalRefresh();
            #pragma warning restore CS0162
        }

        [MenuItem("Tools/Doozy/Refresh/Global Refresh", priority = -400)]
        public static void GlobalRefresh()
        {
            if (!EditorUtility.DisplayDialog
                (
                    $"Refresh Doozy UI Manager?",
                    "Validate the folder structure." +
                    "\n\n" +
                    "Re-write all the helper classes for Id databases." +
                    "\n\n" +
                    "Validate and create all the settings scriptable objects. (does not change existing settings)" +
                    "\n\n" +
                    "Regenerate all the databases with the latest registered items, from the source files." +
                    "\n\n" +
                    "Takes a few minutes, depending on the number of source files and your computer's performance." +
                    "\n\n" +
                    "This operation cannot be undone!",
                    "Yes",
                    "No"
                )
               )
                return;

            Execute();
        }

        private static void Execute()
        {
            //Create Folders
            //../Doozy/Runtime/Data/Resources
            if (!AssetDatabase.IsValidFolder($"{Runtime.RuntimePath.path}/Data/Resources"))
                AssetDatabase.CreateFolder($"{Runtime.RuntimePath.path}/Data", "Resources");

            StopAutomatedUpdate();
            RestoreDataUtils.RestoreAll();
            RefreshDataUtils.RefreshAll();
            DefineSymbolsUtils.DefineAll();
            AssemblyDefinitionsGenerator.Run(true, true);
        }

        private static void RemoveOldFiles()
        {
            //Delete the old ProductInfo for Doozy UI Manager
            ProductInfo[] infoArray = AssetDatabase.FindAssets("t:ProductInfo")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => path.Contains("Doozy/Editor/Data/ProductInfo"))
                .Select(AssetDatabase.LoadAssetAtPath<ProductInfo>)
                .ToArray();

            if (infoArray.Length <= 0)
                return;

            foreach (ProductInfo info in infoArray)
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(info));
        }

        public class Postprocessor : AssetPostprocessor
        {
            private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
            {
                //look if the UpdateBot was deleted
                if (deletedAssets.Any(asset => asset.Contains(nameof(UpdateBot))))
                {
                    // Debug.Log($"[{nameof(UpdateBot)}] Update Bot was deleted");
                    return;
                }
                
                #if DOOZY_42
                // Debug.Log($"[{nameof(UpdateBot)}] Update Bot cannot automatically execute if 42 is enabled");
                return;
                #endif

                #pragma warning disable CS0162 //Unreachable code detected
                if (!instance.Update)
                    return;

                StopAutomatedUpdate();
                RemoveOldFiles();
                Execute();
                #pragma warning restore CS0162 //Unreachable code detected
            }
        }

        public static void StopAutomatedUpdate()
        {
            //Stop the automated update
            instance.Update = false;
            EditorUtility.SetDirty(instance);
            AssetDatabase.SaveAssetIfDirty(instance);
        }
    }
}
