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
    [CustomEditor(typeof(UISelectableSoundyAudio), true)]
    [CanEditMultipleObjects]
    public class UISelectableSoundyAudioEditor : BaseTargetComponentAnimatorEditor
    {
        public UISelectableSoundyAudio castedTarget => (UISelectableSoundyAudio)target;
        public IEnumerable<UISelectableSoundyAudio> castedTargets => targets.Cast<UISelectableSoundyAudio>();

        protected override Color accentColor => EditorColors.Soundy.Color;
        protected override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Soundy.Color;

        private SerializedProperty propertyToggleCommand { get; set; }
        
        private SerializedProperty propertyNormalSoundId { get; set; }
        private SerializedProperty propertyHighlightedSoundId { get; set; }
        private SerializedProperty propertyPressedSoundId { get; set; }
        private SerializedProperty propertySelectedSoundId { get; set; }
        private SerializedProperty propertyDisabledSoundId { get; set; }
        
        private const float k_FieldLabelWidth = 66;
        private FluidField normalSoundIdFluidField { get; set; }
        private FluidField highlightedSoundIdFluidField { get; set; }
        private FluidField pressedSoundIdFluidField { get; set; }
        private FluidField selectedSoundIdFluidField { get; set; }
        private FluidField disabledSoundIdFluidField { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            normalSoundIdFluidField?.Dispose();
            highlightedSoundIdFluidField?.Dispose();
            pressedSoundIdFluidField?.Dispose();
            selectedSoundIdFluidField?.Dispose();
            disabledSoundIdFluidField?.Dispose();
        }

        protected override void FindProperties()
        {
            base.FindProperties();
            
            propertyToggleCommand = serializedObject.FindProperty("ToggleCommand");
            
            propertyNormalSoundId = serializedObject.FindProperty("NormalSoundId");
            propertyHighlightedSoundId = serializedObject.FindProperty("HighlightedSoundId");
            propertyPressedSoundId = serializedObject.FindProperty("PressedSoundId");
            propertySelectedSoundId = serializedObject.FindProperty("SelectedSoundId");
            propertyDisabledSoundId = serializedObject.FindProperty("DisabledSoundId");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(UISelectable)))
                .SetIcon(EditorSpriteSheets.EditorUI.Icons.Sound)
                .SetSecondaryIcon(EditorSpriteSheets.Soundy.Icons.Soundy)
                .SetComponentTypeText("Soundy Audio")
                .AddManualButton()
                .AddApiButton()
                .AddYouTubeButton();

            normalSoundIdFluidField = FluidField.Get().SetLabelText(" Normal").SetElementSize(ElementSize.Tiny).AddFieldContent(DesignUtils.NewPropertyField(propertyNormalSoundId));
            highlightedSoundIdFluidField = FluidField.Get().SetLabelText(" Highlighted").SetElementSize(ElementSize.Tiny).AddFieldContent(DesignUtils.NewPropertyField(propertyHighlightedSoundId));
            pressedSoundIdFluidField = FluidField.Get().SetLabelText(" Pressed").SetElementSize(ElementSize.Tiny).AddFieldContent(DesignUtils.NewPropertyField(propertyPressedSoundId));
            selectedSoundIdFluidField = FluidField.Get().SetLabelText(" Selected").SetElementSize(ElementSize.Tiny).AddFieldContent(DesignUtils.NewPropertyField(propertySelectedSoundId));
            disabledSoundIdFluidField = FluidField.Get().SetLabelText(" Disabled").SetElementSize(ElementSize.Tiny).AddFieldContent(DesignUtils.NewPropertyField(propertyDisabledSoundId));
            
            normalSoundIdFluidField.fieldLabel.SetStyleWidth(k_FieldLabelWidth);
            highlightedSoundIdFluidField.fieldLabel.SetStyleWidth(k_FieldLabelWidth);
            pressedSoundIdFluidField.fieldLabel.SetStyleWidth(k_FieldLabelWidth);
            selectedSoundIdFluidField.fieldLabel.SetStyleWidth(k_FieldLabelWidth);
            disabledSoundIdFluidField.fieldLabel.SetStyleWidth(k_FieldLabelWidth);
        }

        protected override void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddSpaceBlock()
                .AddChild(BaseUISelectableAnimatorEditor.GetController(propertyController, propertyToggleCommand))
                .AddSpaceBlock(2)
                .AddChild(normalSoundIdFluidField)
                .AddSpaceBlock()
                .AddChild(highlightedSoundIdFluidField)
                .AddSpaceBlock()
                .AddChild(pressedSoundIdFluidField)
                .AddSpaceBlock()
                .AddChild(selectedSoundIdFluidField)
                .AddSpaceBlock()
                .AddChild(disabledSoundIdFluidField)
                .AddEndOfLineSpace()
                ;
        }
    }
}

#endif