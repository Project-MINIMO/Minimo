// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
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
    /// Attribute used to mark methods that restore data
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class RestoreDataAttribute : Attribute
    {
        /// <summary>
        /// Name of the data that is restored
        /// </summary>
        public string name { get; }

        /// <summary>
        /// Attribute used to mark methods that restore data
        /// </summary>
        public RestoreDataAttribute() : this("")
        {
        }

        /// <summary>
        /// Attribute used to mark methods that restore data
        /// </summary>
        /// <param name="name"> Name of the data that is restored </param>
        public RestoreDataAttribute(string name)
        {
            this.name = name;
        }
    }

    public static class RestoreDataUtils
    {
        public static void RestoreAll(bool silent = false)
        {
            #if UNITY_EDITOR
            {
                if (!silent) EditorUtility.DisplayProgressBar("Restoring data...", "Getting editor targets...", 0.1f);

                MethodInfo[] editorRestoreMethods =
                    ReflectionUtils
                        .doozyEditorAssembly
                        .GetTypes()
                        .SelectMany(t => t.GetMethods())
                        .Where(m => m.GetCustomAttributes(typeof(RestoreDataAttribute), false).Length > 0)
                        .ToArray();

                if (!silent) EditorUtility.DisplayProgressBar("Restoring data...", "Getting runtime targets...", 0.2f);

                MethodInfo[] runtimeRestoreMethods =
                    ReflectionUtils
                        .doozyRuntimeAssembly
                        .GetTypes()
                        .SelectMany(t => t.GetMethods())
                        .Where(m => m.GetCustomAttributes(typeof(RestoreDataAttribute), false).Length > 0)
                        .ToArray();

                int editorRestoreMethodsCount = editorRestoreMethods.Length;
                for (int i = 0; i < editorRestoreMethodsCount; i++)
                {
                    MethodInfo method = editorRestoreMethods[i];
                    if (!silent)
                    {
                        float progress = 0.2f + 0.4f * i / editorRestoreMethodsCount;
                        EditorUtility.DisplayProgressBar("Restoring editor data...", $"{progress.Round(2) * 100}%", progress);
                    }
                    method.Invoke(null, null);
                }

                int runtimeRestoreMethodsCount = runtimeRestoreMethods.Length;
                for (int i = 0; i < runtimeRestoreMethodsCount; i++)
                {
                    MethodInfo method = runtimeRestoreMethods[i];
                    if (!silent)
                    {
                        float progress = 0.6f + 0.4f * i / runtimeRestoreMethodsCount;
                        EditorUtility.DisplayProgressBar("Restoring runtime data...", $"{progress.Round(2) * 100}%", progress);
                    }
                    method.Invoke(null, null);
                }

                if (!silent) EditorUtility.ClearProgressBar();
            }
            #endif
        }
    }
}
