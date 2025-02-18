// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

#if DOOZY_SOUNDY

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.UIManager.Editors.Animators.Internal;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Audio;
using Doozy.Runtime.UIManager.Containers;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.UIManager.Editors.Audio
{
    [CustomEditor(typeof(UIContainerSoundyAudio), true)]
    [CanEditMultipleObjects]
    public class UIContainerSoundyAudioEditor : BaseTargetComponentAnimatorEditor
    {
        public UIContainerSoundyAudio castedTarget => (UIContainerSoundyAudio)target;
        public IEnumerable<UIContainerSoundyAudio> castedTargets => targets.Cast<UIContainerSoundyAudio>();

        protected override Color accentColor => EditorColors.Soundy.Color;
        protected override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Soundy.Color;
        
        private SerializedProperty propertyShowSoundId { get; set; }
        private SerializedProperty propertyHideSoundId { get; set; }

        private const float k_FieldLabelWidth = 40;
        private FluidField showSoundIdFluidField { get; set; }
        private FluidField hideSoundIdFluidField { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            showSoundIdFluidField?.Dispose();
            hideSoundIdFluidField?.Dispose();
        }

        protected override void FindProperties()
        {
            base.FindProperties();
            
            propertyShowSoundId = serializedObject.FindProperty("ShowSoundId");
            propertyHideSoundId = serializedObject.FindProperty("HideSoundId");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(UIContainer)))
                .SetIcon(EditorSpriteSheets.EditorUI.Icons.Sound)
                .SetSecondaryIcon(EditorSpriteSheets.Soundy.Icons.Soundy)
                .SetComponentTypeText("Soundy Audio")
                .AddManualButton()
                .AddApiButton()
                .AddYouTubeButton();
            
            showSoundIdFluidField = FluidField.Get().SetLabelText(" Show").SetElementSize(ElementSize.Tiny).AddFieldContent(DesignUtils.NewPropertyField(propertyShowSoundId));
            hideSoundIdFluidField = FluidField.Get().SetLabelText(" Hide").SetElementSize(ElementSize.Tiny).AddFieldContent(DesignUtils.NewPropertyField(propertyHideSoundId));
            
            showSoundIdFluidField.fieldLabel.SetStyleWidth(k_FieldLabelWidth);
            hideSoundIdFluidField.fieldLabel.SetStyleWidth(k_FieldLabelWidth);
        }
        
        protected override void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddSpaceBlock()
                .AddChild(BaseUIContainerAnimatorEditor.GetController(propertyController))
                .AddSpaceBlock(2)
                .AddChild(showSoundIdFluidField)
                .AddSpaceBlock()
                .AddChild(hideSoundIdFluidField)
                .AddEndOfLineSpace()
                ;
        }
    }
}

#endif
