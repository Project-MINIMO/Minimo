// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Common.Attributes;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable CheckNamespace

namespace Doozy.Runtime.Common
{
    /// <summary> Base MonoBehaviour clas that implements the singleton pattern </summary>
    /// <typeparam name="T"> Class type </typeparam>
    public abstract class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        /// <summary> Flag that returns TRUE if the application is quitting </summary>
        [ClearOnReload(false)]
        // ReSharper disable once StaticMemberInGenericType
        public static bool IsQuitting;
        
        /// <summary> Lock object used to make the singleton thread safe </summary>
        [ClearOnReload(false)]
        // ReSharper disable once StaticMemberInGenericType
        private static readonly object Lock = new object();

        [ClearOnReload]
        private static T s_instance;

        /// <summary> Get singleton instance </summary>
        public static T instance
        {
            get
            {
                if (IsQuitting) return null;
                lock (Lock)
                {
                    if (s_instance != null) return s_instance;
                    #if UNITY_2023_1_OR_NEWER
                    T[] instances = FindObjectsByType<T>(FindObjectsSortMode.None);
                    #else
                    T[] instances = FindObjectsOfType<T>();
                    #endif
                    int count = instances.Length;
                    if (count == 1) return s_instance = instances[0];
                    if (count > 0)
                    {
                        Debug.LogWarning($"There are {count} instances of the singleton behaviour of type {typeof(T).Name}. There should only be one. Keeping the first one and destroying the rest.");
                        for (int i = 1; i < count; i++) Destroy(instances[i]);
                        return s_instance = instances[0];    
                    }
                    s_instance = new GameObject(typeof(T).Name).AddComponent<T>();
                    return s_instance;
                }
            }
        }

        protected virtual void Awake()
        {
            if (s_instance != null && s_instance != this)
            {
                Debug.Log($"There cannot be two '{typeof(T).Name}' active at the same time. Destroying the '{gameObject.name}' GameObject!");
                Destroy(gameObject);
                return;
            }

            s_instance = GetComponent<T>();
            DontDestroyOnLoad(gameObject);
        }

        protected virtual void OnDestroy()
        {
            if (s_instance == this)
                s_instance = null;
        }

        private void OnApplicationQuit()
        {
            IsQuitting = true;
        }
    }
}
