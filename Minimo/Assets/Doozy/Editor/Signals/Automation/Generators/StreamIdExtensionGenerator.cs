// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Doozy.Editor.Common.Utils;
using Doozy.Editor.Signals.ScriptableObjects;
using Doozy.Runtime;
using Doozy.Runtime.Common;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Signals;
using UnityEditor;

namespace Doozy.Editor.Signals.Automation.Generators
{
    public static class StreamIdExtensionGenerator
    {
        private static string templateName => nameof(StreamIdExtensionGenerator).Replace("Generator", "");
        private static string templateNameWithExtension => $"{templateName}.cst";
        private static string templateFilePath => $"{EditorPath.path}/Signals/Automation/Templates/{templateNameWithExtension}";

        private static string targetFileNameWithExtension => $"{templateName}.cs";
        private static string targetFilePath => $"{RuntimePath.path}/Signals/{targetFileNameWithExtension}";

        public static bool Run(bool saveAssets = true, bool refreshAssetDatabase = false, bool silent = false)
        {
            string data = FileGenerator.GetFile(templateFilePath);
            if (data.IsNullOrEmpty()) return false;
            StreamIdDataGroup dataGroup = StreamIdDatabase.instance.database;
            if (!StreamIdDatabase.instance.database.isEmpty)
                data = InjectContent(data, dataGroup.GetCategories, category => dataGroup.GetNames(category));
            bool result = FileGenerator.WriteFile(targetFilePath, data, silent);
            if (!result) return false;
            if (saveAssets) AssetDatabase.SaveAssets();
            if (refreshAssetDatabase) AssetDatabase.Refresh();
            return true;
        }

        private static string InjectContent(string data, Func<IEnumerable<string>> getCategories, Func<string, IEnumerable<string>> getNames)
        {
            var serviceAccessorStringBuilder = new StringBuilder();
            var signalAccessorStringBuilder = new StringBuilder();
            var streamAccessorStringBuilder = new StringBuilder();
            var dataStringBuilder = new StringBuilder();
            var categories = getCategories.Invoke().ToList();
            int categoriesCount = categories.Count;
            for (int categoryIndex = 0; categoryIndex < categories.Count; categoryIndex++)
            {
                string category = categories[categoryIndex];
                if (category.Equals(CategoryNameItem.k_DefaultCategory)) continue;
                var names = getNames.Invoke(category).ToList();

                //SERVICE_ACCESSOR//
                {
                    serviceAccessorStringBuilder.AppendLine($"        public static {nameof(SignalStream)} GetStream({nameof(StreamId)}.{category} id) => {nameof(SignalsService.GetStream)}(nameof({nameof(StreamId)}.{category}), id.ToString());");
                    if (categoryIndex < categoriesCount - 1) serviceAccessorStringBuilder.AppendLine();
                }
                
                //SIGNAL_ACCESSOR//
                {
                    signalAccessorStringBuilder.AppendLine($"        public static bool Send({nameof(StreamId)}.{category} id, string message = \"\") => {nameof(SignalsService)}.{nameof(SignalsService.SendSignal)}(nameof({nameof(StreamId)}.{category}), id.ToString(), message);");
                    signalAccessorStringBuilder.AppendLine($"        public static bool Send({nameof(StreamId)}.{category} id, GameObject signalSource, string message = \"\") => {nameof(SignalsService)}.{nameof(SignalsService.SendSignal)}(nameof({nameof(StreamId)}.{category}), id.ToString(), signalSource, message);");
                    signalAccessorStringBuilder.AppendLine($"        public static bool Send({nameof(StreamId)}.{category} id, SignalProvider signalProvider, string message = \"\") => {nameof(SignalsService)}.{nameof(SignalsService.SendSignal)}(nameof({nameof(StreamId)}.{category}), id.ToString(), signalProvider, message);");
                    signalAccessorStringBuilder.AppendLine($"        public static bool Send({nameof(StreamId)}.{category} id, Object signalSender, string message = \"\") => {nameof(SignalsService)}.{nameof(SignalsService.SendSignal)}(nameof({nameof(StreamId)}.{category}), id.ToString(), signalSender, message);");
                    signalAccessorStringBuilder.AppendLine($"        public static bool Send<T>({nameof(StreamId)}.{category} id, T signalValue, string message = \"\") => {nameof(SignalsService)}.{nameof(SignalsService.SendSignal)}(nameof({nameof(StreamId)}.{category}), id.ToString(), signalValue, message);");
                    signalAccessorStringBuilder.AppendLine($"        public static bool Send<T>({nameof(StreamId)}.{category} id, T signalValue, GameObject signalSource, string message = \"\") => {nameof(SignalsService)}.{nameof(SignalsService.SendSignal)}(nameof({nameof(StreamId)}.{category}), id.ToString(), signalValue, signalSource, message);");
                    signalAccessorStringBuilder.AppendLine($"        public static bool Send<T>({nameof(StreamId)}.{category} id, T signalValue, SignalProvider signalProvider, string message = \"\") => {nameof(SignalsService)}.{nameof(SignalsService.SendSignal)}(nameof({nameof(StreamId)}.{category}), id.ToString(), signalValue, signalProvider, message);");
                    signalAccessorStringBuilder.AppendLine($"        public static bool Send<T>({nameof(StreamId)}.{category} id, T signalValue, Object signalSender, string message = \"\") => {nameof(SignalsService)}.{nameof(SignalsService.SendSignal)}(nameof({nameof(StreamId)}.{category}), id.ToString(), signalValue, signalSender, message);");
                    if (categoryIndex < categoriesCount - 1) signalAccessorStringBuilder.AppendLine();
                }
                
                //STREAM_ACCESSOR//
                {
                    streamAccessorStringBuilder.AppendLine($"        public static {nameof(SignalStream)} GetStream({nameof(StreamId)}.{category} id) => {nameof(SignalsService)}.{nameof(SignalsService.GetStream)}(id);");
                    if (categoryIndex < categoriesCount - 1) streamAccessorStringBuilder.AppendLine();
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

            data = data.Replace("//SERVICE_ACCESSOR//", serviceAccessorStringBuilder.ToString().RemoveLast(Environment.NewLine.Length));
            data = data.Replace("//SIGNAL_ACCESSOR//", signalAccessorStringBuilder.ToString().RemoveLast(Environment.NewLine.Length));
            data = data.Replace("//STREAM_ACCESSOR//", streamAccessorStringBuilder.ToString().RemoveLast(Environment.NewLine.Length));
            data = data.Replace("//DATA//", dataStringBuilder.ToString().RemoveLast(Environment.NewLine.Length));
            
            data += Environment.NewLine;
            return data;
        }
    }
}
