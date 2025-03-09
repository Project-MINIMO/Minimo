// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Doozy.Editor.Common.Utils;
using Doozy.Editor.UIManager.ScriptableObjects;
using Doozy.Runtime;
using Doozy.Runtime.Common;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.UIManager;
using Doozy.Runtime.UIManager.Components;
using UnityEditor;

namespace Doozy.Editor.UIManager.Automation.Generators
{
    public static class UIButtonIdExtensionGenerator
    {
        private static string templateName => nameof(UIButtonIdExtensionGenerator).Replace("Generator", "");
        private static string templateNameWithExtension => $"{templateName}.cst";
        private static string templateFilePath => $"{EditorPath.path}/UIManager/Automation/Templates/{templateNameWithExtension}";

        private static string targetFileNameWithExtension => $"{templateName}.cs";
        private static string targetFilePath => $"{RuntimePath.path}/UIManager/Ids/{targetFileNameWithExtension}";

        public static bool Run(bool saveAssets = true, bool refreshAssetDatabase = false, bool silent = false)
        {
            string data = FileGenerator.GetFile(templateFilePath);
            if (data.IsNullOrEmpty()) return false;
            UIButtonIdDataGroup dataGroup = UIButtonIdDatabase.instance.database;
            if (!UIButtonIdDatabase.instance.database.isEmpty)
                data = InjectContent(data, dataGroup.GetCategories, category => dataGroup.GetNames(category));
            bool result = FileGenerator.WriteFile(targetFilePath, data, silent);
            if (!result) return false;
            if (saveAssets) AssetDatabase.SaveAssets();
            if (refreshAssetDatabase) AssetDatabase.Refresh();
            return true;
        }

        private static string InjectContent(string data, Func<IEnumerable<string>> getCategories, Func<string, IEnumerable<string>> getNames)
        {
            var accessorStringBuilder = new StringBuilder();
            var dataStringBuilder = new StringBuilder();
            var categories = getCategories.Invoke().ToList();
            int categoriesCount = categories.Count;
            for (int categoryIndex = 0; categoryIndex < categories.Count; categoryIndex++)
            {
                string category = categories[categoryIndex];
                if (category.Equals(CategoryNameItem.k_DefaultCategory)) continue;
                var names = getNames.Invoke(category).ToList();

                //ACCESSOR//
                {
                    accessorStringBuilder.AppendLine($"        public static IEnumerable<{nameof(UIButton)}> GetButtons({nameof(UIButtonId)}.{category} id) => GetButtons(nameof({nameof(UIButtonId)}.{category}), id.ToString());");
                    accessorStringBuilder.AppendLine($"        public static bool SelectButton({nameof(UIButtonId)}.{category} id) => SelectButton(nameof({nameof(UIButtonId)}.{category}), id.ToString());");
                    if (categoryIndex < categoriesCount - 2) accessorStringBuilder.AppendLine();
                }

                //DATA//
                {
                    dataStringBuilder.AppendLine($"        public enum {category}");
                    dataStringBuilder.AppendLine("        {");
                    for (int nameIndex = 0; nameIndex < names.Count; nameIndex++)
                    {
                        string name = names[nameIndex];
                        dataStringBuilder.AppendLine($"            {name}{(nameIndex < names.Count - 1 ? "," : "")}");
                    }
                    dataStringBuilder.AppendLine("        }");
                    if (categoryIndex < categoriesCount - 2) dataStringBuilder.AppendLine();
                }
            }

            data = data.Replace("//ACCESSOR//", accessorStringBuilder.ToString().RemoveLast(Environment.NewLine.Length));
            data = data.Replace("//DATA//", dataStringBuilder.ToString().RemoveLast(Environment.NewLine.Length));
            
            data += Environment.NewLine;
            return data;
        }
    }
}
