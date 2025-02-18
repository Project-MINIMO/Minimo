// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Linq;
using System.Reflection;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Common.Utils;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Doozy.Runtime.Common.Attributes
{
    /// <summary>
    /// Attribute used to define symbols in the project
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class DefineSymbolsAttribute : Attribute
    {
        /// <summary>
        /// Name of the define symbol method
        /// </summary>
        public string name { get; }

        /// <summary>
        /// Attribute used to define symbols in the project
        /// </summary>
        public DefineSymbolsAttribute() : this("")
        {
        }

        /// <summary>
        /// Attribute used to define symbols in the project
        /// </summary>
        /// <param name="name"> Name of the define symbol method </param>
        public DefineSymbolsAttribute(string name)
        {
            this.name = name;
        }
    }

    public static class DefineSymbolsUtils
    {
        public static void DefineAll(bool silent = false)
        {
            #if UNITY_EDITOR
            {
                if (!silent) EditorUtility.DisplayProgressBar("Defining symbols...", "Getting editor targets...", 0.1f);

                MethodInfo[] editorDefineMethods =
                    ReflectionUtils
                        .doozyEditorAssembly
                        .GetTypes()
                        .SelectMany(t => t.GetMethods())
                        .Where(m => m.GetCustomAttributes(typeof(DefineSymbolsAttribute), false).Length > 0)
                        .ToArray();

                if (!silent) EditorUtility.DisplayProgressBar("Defining symbols...", "Getting runtime targets...", 0.2f);

                MethodInfo[] runtimeDefineMethods =
                    ReflectionUtils
                        .doozyRuntimeAssembly
                        .GetTypes()
                        .SelectMany(t => t.GetMethods())
                        .Where(m => m.GetCustomAttributes(typeof(DefineSymbolsAttribute), false).Length > 0)
                        .ToArray();

                int editorDefineMethodsCount = editorDefineMethods.Length;
                for (int i = 0; i < editorDefineMethods.Length; i++)
                {
                    MethodInfo method = editorDefineMethods[i];
                    if (!silent)
                    {
                        float progress = 0.2f + 0.4f * i / editorDefineMethodsCount;
                        EditorUtility.DisplayProgressBar("Defining symbols...", $"{progress.Round(2) * 100}%", progress);
                    }
                    method.Invoke(null, null);
                }

                int runtimeDefineMethodsCount = runtimeDefineMethods.Length;
                for (int i = 0; i < runtimeDefineMethods.Length; i++)
                {
                    MethodInfo method = runtimeDefineMethods[i];
                    if (!silent)
                    {
                        float progress = 0.6f + 0.4f * i / runtimeDefineMethodsCount;
                        EditorUtility.DisplayProgressBar("Defining symbols...", $"{progress.Round(2) * 100}%", progress);
                    }
                    method.Invoke(null, null);
                }

                if (!silent) EditorUtility.ClearProgressBar();
            }
            #endif
        }
    }
}
