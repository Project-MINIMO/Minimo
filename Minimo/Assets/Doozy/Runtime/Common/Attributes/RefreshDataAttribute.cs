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
    /// Attribute used to mark methods that refresh data
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class RefreshDataAttribute : Attribute
    {
        /// <summary>
        /// Name of the data that is refreshed
        /// </summary>
        public string name { get; }

        /// <summary>
        /// Attribute used to mark methods that refresh data
        /// </summary>
        public RefreshDataAttribute() : this("")
        {
        }

        /// <summary>
        /// Attribute used to mark methods that refresh data
        /// </summary>
        /// <param name="name"> Name of the data that is refreshed </param>
        public RefreshDataAttribute(string name)
        {
            this.name = name;
        }
    }

    public static class RefreshDataUtils
    {
        public static void RefreshAll(bool silent = false)
        {
            #if UNITY_EDITOR
            {
                if (!silent) EditorUtility.DisplayProgressBar("Refreshing data...", "Getting editor targets...", 0.1f);

                MethodInfo[] editorRefreshMethods =
                    ReflectionUtils
                        .doozyEditorAssembly
                        .GetTypes()
                        .SelectMany(t => t.GetMethods())
                        .Where(m => m.GetCustomAttributes(typeof(RefreshDataAttribute), false).Length > 0)
                        .ToArray();

                if (!silent) EditorUtility.DisplayProgressBar("Refreshing data...", "Getting runtime targets...", 0.2f);

                MethodInfo[] runtimeRefreshMethods =
                    ReflectionUtils
                        .doozyRuntimeAssembly
                        .GetTypes()
                        .SelectMany(t => t.GetMethods())
                        .Where(m => m.GetCustomAttributes(typeof(RefreshDataAttribute), false).Length > 0)
                        .ToArray();

                int editorRefreshMethodsCount = editorRefreshMethods.Length;
                for (int i = 0; i < editorRefreshMethodsCount; i++)
                {

                    MethodInfo method = editorRefreshMethods[i];
                    if (!silent)
                    {
                        float progress = 0.2f + (0.8f * i / editorRefreshMethodsCount);
                        EditorUtility.DisplayProgressBar("Refreshing data...", $"{progress.Round(2) * 100}%", progress);
                    }
                    method.Invoke(null, null);
                }

                int runtimeRefreshMethodsCount = runtimeRefreshMethods.Length;
                for (int i = 0; i < runtimeRefreshMethodsCount; i++)
                {
                    MethodInfo method = runtimeRefreshMethods[i];
                    if (!silent)
                    {
                        float progress = 0.2f + (0.8f * i / runtimeRefreshMethodsCount);
                        EditorUtility.DisplayProgressBar("Refreshing data...", $"{progress.Round(2) * 100}%", progress);
                    }
                    method.Invoke(null, null);
                }

                if (!silent) EditorUtility.ClearProgressBar();
            }
            #endif
        }
    }
}
