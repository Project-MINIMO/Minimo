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
// ReSharper disable MemberCanBePrivate.Local

namespace Doozy.Editor.Nody.Automation.Generators
{
    public static class NodyNodeSearchWindowGenerator
    {
        private static string templateName => nameof(NodyNodeSearchWindowGenerator).Replace("Generator", "");
        private static string templateNameWithExtension => $"{templateName}.cst";
        private static string templateFilePath => $"{EditorPath.path}/Nody/Automation/Templates/{templateNameWithExtension}";

        private static string targetFileNameWithExtension => $"{templateName}.cs";
        private static string targetFilePath => $"{EditorPath.path}/Nody/{targetFileNameWithExtension}";

        /// <summary>
        /// Generates the NodyNodeSearchWindow class
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
            data += Environment.NewLine;
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
        /// <param name="templateData"> Template data </param>
        /// <returns> Injected template data </returns>
        private static string InjectContent(bool includeOnlyNativeNodes, string templateData)
        {
            var searchTreeEntryStringBuilder = new StringBuilder();
            var createNodeStringBuilder = new StringBuilder();

            IEnumerable<Type> nodeTypeCollection = TypeCache.GetTypesWithAttribute<NodyMenuPathAttribute>().Where(t => !t.IsAbstract);

            var nodeCache =
                (
                    from nodeType in nodeTypeCollection
                    let attribute = (NodyMenuPathAttribute)nodeType.GetCustomAttributes(typeof(NodyMenuPathAttribute), false).First()
                    select new NodeSearchItem(nodeType, attribute)
                )
                .OrderBy(item => item.nodeType.FullName)
                .ToList();


            if (includeOnlyNativeNodes)
            {
                nodeCache =
                    nodeCache
                        .Where
                        (
                            item =>
                                item.nodeType.FullName != null &&
                                (
                                    item.nodeType.FullName.StartsWith("Doozy.Runtime.Nody") ||
                                    item.nodeType.FullName.StartsWith("Doozy.Runtime.UIManager") ||
                                    item.nodeType.FullName.StartsWith("Doozy.Runtime.SceneManagement")
                                )
                        )
                        .ToList();
            }

            var categories = nodeCache.Select(item => item.attribute.category).Distinct().ToList();
            categories.Sort();

            foreach (string category in categories)
            {
                searchTreeEntryStringBuilder.AppendLine($"                new SearchTreeGroupEntry(new GUIContent(\"{category}\"), 1),");
                var items = nodeCache.Where(item => item.attribute.category.Equals(category)).OrderBy(item => item.attribute.name).ToList();
                foreach (NodeSearchItem item in items)
                {
                    searchTreeEntryStringBuilder.AppendLine($"                new SearchTreeEntry(new GUIContent(\"{item.attribute.name}\", transparentIcon)) {{ userData = new NodeTypeInfo(typeof({item.nodeType.FullName})), level = 2 }},");
                    createNodeStringBuilder.AppendLine($"            if(nodeInfo.type == typeof({item.nodeType.FullName})) {{ graphView.CreateNode(typeof({item.nodeType.FullName}), true); return true;}}");
                }
            }

            templateData = templateData.Replace("//SearchTreeEntry//", searchTreeEntryStringBuilder.ToString().RemoveLast(Environment.NewLine.Length));
            templateData = templateData.Replace("//CreateNode//", createNodeStringBuilder.ToString().RemoveLast(Environment.NewLine.Length));
            return templateData;
        }

        private class NodeSearchItem
        {
            public Type nodeType { get; }
            public NodyMenuPathAttribute attribute { get; }

            public NodeSearchItem(Type nodeType, NodyMenuPathAttribute attribute)
            {
                this.nodeType = nodeType;
                this.attribute = attribute;
            }
        }
    }
}
