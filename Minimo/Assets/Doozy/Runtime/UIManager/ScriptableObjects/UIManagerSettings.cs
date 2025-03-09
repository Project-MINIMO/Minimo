// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Common.ScriptableObjects;

namespace Doozy.Runtime.UIManager.ScriptableObjects
{
    public class UIManagerSettings : SingletonRuntimeScriptableObject<UIManagerSettings>
    {
        [RestoreData(nameof(UIManagerSettings))]
        public static UIManagerSettings Get() =>
            instance;

        public const string k_UIManagerSymbol = "DOOZY_UIMANAGER";
        
        /// <summary> Enable orientation change support for the UIManager system </summary>
        public bool UseOrientationDetection;
    }
}