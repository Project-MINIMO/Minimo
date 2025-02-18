// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.UIManager.Editors.Components.Internal;
using Doozy.Runtime.UIManager.Animators;
using Doozy.Runtime.UIManager.Components;
using UnityEditor;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.Editors.Components
{
    [CustomEditor(typeof(UISelectable), true)]
    [CanEditMultipleObjects]
    public sealed class UISelectableEditor : UISelectableBaseEditor
    {
        public override Color accentColor => EditorColors.Default.UIComponent;
        public override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Default.UIComponent;
        
        public static IEnumerable<Texture2D> selectableIconTextures => EditorSpriteSheets.UIManager.Icons.UISelectable;
        
        public UISelectable castedTarget => (UISelectable)target;
        public IEnumerable<UISelectable> castedTargets => targets.Cast<UISelectable>();

        protected override void SearchForAnimators()
        {
            selectableAnimators ??= new List<BaseUISelectableAnimator>();
            selectableAnimators.Clear();
            
            //check if prefab was selected
            if (castedTargets.Any(s => s.gameObject.scene.name == null)) 
            {
                selectableAnimators.AddRange(castedSelectable.GetComponentsInChildren<BaseUISelectableAnimator>());
                return;
            }
            
            //not prefab
            #if UNITY_2023_1_OR_NEWER
            selectableAnimators.AddRange(FindObjectsByType<BaseUISelectableAnimator>(FindObjectsSortMode.None));
            #else
            selectableAnimators.AddRange(FindObjectsOfType<BaseUISelectableAnimator>());
            #endif
        }
        
        protected override void InitializeEditor()
        {
            base.InitializeEditor();
            
            componentHeader
                .SetAccentColor(accentColor)
                .SetComponentNameText("UISelectable")
                .SetIcon(selectableIconTextures.ToList())
                .AddManualButton()
                .AddApiButton("https://api.doozyui.com/api/Doozy.Runtime.UIManager.Components.UISelectable.html")
                .AddYouTubeButton();
        }

        protected override void Compose()
        {
            if (castedTarget == null)
                return;
            
            base.Compose();
        }
    }
}
