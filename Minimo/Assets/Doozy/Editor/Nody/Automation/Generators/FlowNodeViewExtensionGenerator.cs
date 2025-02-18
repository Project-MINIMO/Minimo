// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Doozy.Editor.Common.Utils;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Nody;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Nody.Automation.Generators
{
    public static class FlowNodeViewExtensionGenerator
    {
        private static string templateName => nameof(FlowNodeViewExtensionGenerator).Replace("Generator", "");
        private static string templateNameWithExtension => $"{templateName}.cst";
        private static string templateFilePath => $"{EditorPath.path}/Nody/Automation/Templates/{templateNameWithExtension}";

        private static string targetFileNameWithExtension => $"{templateName}.cs";
        private static string targetFilePath => $"{EditorPath.path}/Nody/{targetFileNameWithExtension}";

        /// <summary>
        /// Generates the FlowNodeViewExtension class
        /// </summary>
        /// <param name="includeOnlyNativeNodes"> If TRUE, only native Doozy UI Manager nodes will be included in the search window </param>
        /// <param name="saveAssets"> If TRUE, the assets will be saved </param>
        /// <param name="refreshAssetDatabase"> If TRUE, the asset database will be refreshed </param>
        /// <param name="silent"> If TRUE, no log messages will be printed </param>
        /// <returns> TRUE if the operation was successful </returns>
        public static bool Run(bool includeOnlyNativeNodes, bool saveAssets = true, bool refreshAssetDatabase = false, bool silent = false)
        {
            string data = FileGenerator.GetFile(templateFilePath);
            data = InjectContent(includeOnlyNativeNodes, data);
            bool result = FileGenerator.WriteFile(targetFilePath, data, silent);
            if (!result) return false;
            if (saveAssets) AssetDatabase.SaveAssets();
            if (refreshAssetDatabase) AssetDatabase.Refresh();
            return true;
        }

        /// <summary>
        /// Injects the content into the template
        /// </summary>
        /// <param name="includeOnlyNativeNodes"> If TRUE, only native Doozy UI Manager nodes will be included in the search window </param>
        /// <param name="data"> Template data </param>
        /// <returns> Injected template data </returns>
        private static string InjectContent(bool includeOnlyNativeNodes, string data)
        {
            var nodesStringBuilder = new StringBuilder();
            IEnumerable<Type> nodeTypeCollection = TypeCache.GetTypesDerivedFrom<FlowNode>().Where(t => !t.IsAbstract).OrderBy(t => t.FullName);
            IEnumerable<Type> nodeViewTypeCollection = TypeCache.GetTypesDerivedFrom<FlowNodeView>().Where(t => !t.IsAbstract);

            if (includeOnlyNativeNodes)
            {
                nodeTypeCollection =
                    nodeTypeCollection
                        .Where
                        (
                            item =>
                                item.FullName != null &&
                                (
                                    item.FullName.StartsWith("Doozy.Runtime.Nody") ||
                                    item.FullName.StartsWith("Doozy.Runtime.UIManager") ||
                                    item.FullName.StartsWith("Doozy.Runtime.SceneManagement")
                                )
                        )
                        .ToList();
            }

            foreach (Type nodeType in nodeTypeCollection)
            {
                string nodeTypeFullName = nodeType.FullName;
                string nodeViewTypeFullName = string.Empty;

                foreach (Type nodeViewType in nodeViewTypeCollection)
                {
                    if (nodeViewType.Name.Equals($"{nodeType.Name}View"))
                        nodeViewTypeFullName = nodeViewType.FullName;
                }

                if (nodeViewTypeFullName.IsNullOrEmpty())
                {
                    Debug.LogWarning
                    (
                        $"Could not find the '{nameof(FlowNodeView)}' node view for the '{nodeType.Name}' node. " +
                        $"Searching for '{nodeType.Name}View' failed so the node type was not added to Nody." +
                        $"To fix this, create a node view for the '{nodeType.Name}' node and name it '{nodeType.Name}View'"
                    );
                    continue;
                }



                nodesStringBuilder.AppendLine($"                {nodeTypeFullName} _ => new {nodeViewTypeFullName}(graphView, node),");
            }
            data = data.Replace("//NODES//", nodesStringBuilder.ToString().RemoveLast(Environment.NewLine.Length));

            data += Environment.NewLine;
            return data;
        }
    }
}
