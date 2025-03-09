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
using Doozy.Runtime.UIManager.Components;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.UIManager.Editors.Audio
{
    [CustomEditor(typeof(UIToggleSoundyAudio), true)]
    [CanEditMultipleObjects]
    public class UIToggleSoundyAudioEditor : BaseTargetComponentAnimatorEditor
    {
        public UIToggleSoundyAudio castedTarget => (UIToggleSoundyAudio)target;
        public IEnumerable<UIToggleSoundyAudio> castedTargets => targets.Cast<UIToggleSoundyAudio>();
        
        protected override Color accentColor => EditorColors.Soundy.Color;
        protected override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Soundy.Color;
        
        private SerializedProperty propertyOnSoundId { get; set; }
        private SerializedProperty propertyOffSoundId { get; set; }
        
        private const float k_FieldLabelWidth = 20;
        private FluidField onSoundIdFluidField { get; set; }
        private FluidField offSoundIdFluidField { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            onSoundIdFluidField?.Dispose();
            offSoundIdFluidField?.Dispose();
        }
        
        protected override void FindProperties()
        {
            base.FindProperties();
            
            propertyOnSoundId = serializedObject.FindProperty("OnSoundId");
            propertyOffSoundId = serializedObject.FindProperty("OffSoundId");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(UIToggle)))
                .SetIcon(EditorSpriteSheets.EditorUI.Icons.Sound)
                .SetSecondaryIcon(EditorSpriteSheets.Soundy.Icons.Soundy)
                .SetComponentTypeText("Soundy Audio")
                .AddManualButton()
                .AddApiButton()
                .AddYouTubeButton();
            
            onSoundIdFluidField = 
                FluidField.Get()
                    .SetLabelText(" On")
                    .SetElementSize(ElementSize.Tiny)
                    .AddFieldContent(DesignUtils.NewPropertyField(propertyOnSoundId))
                    .SetTooltip("SoundId played when the toggle transitions to the ON state");
            
            offSoundIdFluidField = 
                FluidField.Get()
                    .SetLabelText(" Off")
                    .SetElementSize(ElementSize.Tiny)
                    .AddFieldContent(DesignUtils.NewPropertyField(propertyOffSoundId))
                    .SetTooltip("SoundId played when the toggle transitions to the OFF state");
            
            onSoundIdFluidField.fieldLabel.SetStyleWidth(k_FieldLabelWidth);
            offSoundIdFluidField.fieldLabel.SetStyleWidth(k_FieldLabelWidth);
        }
        
        protected override void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddSpaceBlock()
                .AddChild(BaseUIContainerAnimatorEditor.GetController(propertyController))
                .AddSpaceBlock(2)
                .AddChild(onSoundIdFluidField)
                .AddSpaceBlock()
                .AddChild(offSoundIdFluidField)
                .AddEndOfLineSpace()
                ;
        }
    }
}

#endif