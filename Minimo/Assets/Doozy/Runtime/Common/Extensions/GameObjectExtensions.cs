// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;

namespace Doozy.Runtime.Common.Extensions
{
    /// <summary> Extension methods for the GameObject class </summary>
    public static class GameObjectExtensions
    {
        /// <summary> Gets or adds a component of type T to the target GameObject. </summary>
        /// <param name="target"> Target GameObject </param>
        /// <typeparam name="T"> Type of component to get or add </typeparam>
        /// <returns> The component of type T </returns>
        public static T GetOrAddComponent<T>(this GameObject target) where T : MonoBehaviour =>
            target.GetComponent<T>() ?? target.AddComponent<T>();

        /// <summary> Checks if the target GameObject has a component of type T attached to it. </summary>
        /// <param name="target"> Target GameObject </param>
        /// <typeparam name="T"> Type of component to check for </typeparam>
        /// <returns> True if the target GameObject has a component of type T attached to it </returns>
        public static bool HasComponent<T>(this GameObject target) where T : MonoBehaviour =>
            target.GetComponent<T>() != null;
        
        
    }
}
