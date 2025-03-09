// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Common.ScriptableObjects;
using Doozy.Runtime.Common.Attributes;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.UIMenu
{
    public class UIMenuSettings : SingletonEditorScriptableObject<UIMenuSettings>
    {
        public bool SelectNewlyCreatedObjects;
        public PrefabInstantiateModeSetting InstantiateMode;

        public const int MIN_ITEM_SIZE = 64;
        public const int MAX_ITEM_SIZE = 256;
        public const int DEFAULT_ITEM_SIZE = 96;
        
        [SerializeField] private int ItemSize = DEFAULT_ITEM_SIZE;
        public int itemSize
        {
            get => ItemSize;
            set => ItemSize = Mathf.Clamp(value, MIN_ITEM_SIZE, MAX_ITEM_SIZE);
        }
        
        [RestoreData(nameof(UIMenuSettings))]
        public static UIMenuSettings RestoreData() =>
            instance;
    }
}
