// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Listeners;
using UnityEditor;

namespace Doozy.Editor.UIManager.Editors.Listeners
{
    [CustomEditor(typeof(FloatSignalListener), true)]
    public class FloatSignalListenerEditor : SignalListenerEditor
    {
        private FloatSignalListener castedTarget => (FloatSignalListener)target;
        private IEnumerable<FloatSignalListener> castedTargets => targets.Cast<FloatSignalListener>();

        private FluidField onFloatSignalFluidField { get; set; }
        
        private SerializedProperty propertyOnFloatSignal { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            onFloatSignalFluidField?.Recycle();
        }

        protected override void FindProperties()
        {
            base.FindProperties();
            propertyOnFloatSignal = serializedObject.FindProperty("OnFloatSignal");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText("Float")
                .SetComponentTypeText("Signal Listener");

            onFloatSignalFluidField =
                FluidField.Get()
                    .AddFieldContent(DesignUtils.UnityEventField("UnityEvent with a float parameter", propertyOnFloatSignal));
        }

        protected override void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddSpaceBlock()
                .AddChild(idFluidField)
                .AddSpaceBlock()
                .AddChild(onFloatSignalFluidField)
                // .AddSpaceBlock()
                // .AddChild(callbackFluidField)
                // .AddSpaceBlock()
                // .AddChild(onSignalFluidField)
                .AddEndOfLineSpace()
                ;
        }
    }
}
